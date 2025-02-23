using ExCSS;
using ExplorerLibrary.Services;
using iTextSharp.text.pdf;
using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using TempriDomain.Entity;
using TempriDomain.Interfaces;
using TempriDomain.ValueObject;

namespace PrintGenerater.Services
{
    public class PdfGenerator
    {
        IPrint print;
        FolderPathValue folderPathValue;
        public PdfGenerator(IPrint print, FolderPathValue folderPathValue)
        {
            this.print = print;
            this.folderPathValue = folderPathValue;
        }
        /// <summary>
        /// SVGからベクター形式のPDFを作成し、1ページ4問の回答PDFの作成後、カバー・問題・回答を結合したPDFを作成します。
        /// </summary>
        public void CreatePdf()
        {
            CreateVectorPdf();
            CreateGroupedAnswerPdf();
            CreateGoodsPdf();
        }
        /// <summary>
        /// ベクター形式のPDFを作成するためにinkscapeでのSVG-PDF変換を行います。
        /// SVGのファイル名を見て、問題と回答を識別し、別々のフォルダへ出力しています。
        /// </summary>
        private void CreateVectorPdf()  
        {
            var bat = new BatService();
            bat.GenerateAndExecuteBat("svg-to-pdf-template.txt",  print.PrintId);
        }
        /// <summary>
        /// 回答PDFを1ページあたり4問のレイアウトにして出力します。
        /// </summary>
        private void CreateGroupedAnswerPdf()
        {
            try
            {
                var pdfFiles = Directory.GetFiles(folderPathValue.PdfaDir, "*-a.pdf").ToList();

                var i = 0;
                var chunks = GroupByChunk(pdfFiles, 4);
                foreach (var chunk in chunks)
                {
                    i++;
                    var PdfPaths = chunk.ToList();
                    var inputPdf1 = chunk.Count() >= 1 ? PdfSharp.Pdf.IO.PdfReader.Open(PdfPaths[0], PdfDocumentOpenMode.Import) : new PdfSharp.Pdf.PdfDocument();
                    var inputPdf2 = chunk.Count() >= 2 ? PdfSharp.Pdf.IO.PdfReader.Open(PdfPaths[1], PdfDocumentOpenMode.Import) : new PdfSharp.Pdf.PdfDocument();
                    var inputPdf3 = chunk.Count() >= 3 ? PdfSharp.Pdf.IO.PdfReader.Open(PdfPaths[2], PdfDocumentOpenMode.Import) : new PdfSharp.Pdf.PdfDocument();
                    var inputPdf4 = chunk.Count() >= 4 ? PdfSharp.Pdf.IO.PdfReader.Open(PdfPaths[3], PdfDocumentOpenMode.Import) : new PdfSharp.Pdf.PdfDocument();

                    // 新しいPDFを作成
                    var outputPdf = new PdfSharp.Pdf.PdfDocument();
                    var page = outputPdf.AddPage();
                    XGraphics gfx = XGraphics.FromPdfPage(page);

                    // ページのサイズを取得
                    double width = page.Width;
                    double height = page.Height;

                    // 各PDFのサイズを計算（ページを4分割するために2で割る）
                    double subWidth = width / 2;
                    double subHeight = height / 2;

                    // 各PDFページを描画
                    DrawPdfPage(gfx, inputPdf1.FullPath, 0, 0, subWidth, subHeight);
                    DrawPdfPage(gfx, inputPdf2.FullPath, subWidth, 0, subWidth, subHeight);
                    DrawPdfPage(gfx, inputPdf3.FullPath, 0, subHeight, subWidth, subHeight);
                    DrawPdfPage(gfx, inputPdf4.FullPath, subWidth, subHeight, subWidth, subHeight);

                    // PDFを保存
                    outputPdf.Save(Path.Combine(folderPathValue.Pdf4Dir, $@"{pdfFiles.Count()}-{i.ToString("D3")}-answer.pdf"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        /// <summary>
        /// カバー画像・問題・回答をくっつけて1つのプリントにします。
        /// </summary>
        private void CreateGoodsPdf()
        {
            var cover = new List<string> { Path.Combine(folderPathValue.CoverDir, $"{print.PrintId}-cover.pdf") };
            var questions = Directory.GetFiles(folderPathValue.PdfqDir, "*.pdf").ToList();
            var answers = Directory.GetFiles(folderPathValue.Pdf4Dir, "*.pdf").ToList();
            var paths = cover.Concat(questions.Concat(answers)).ToList();
            CombinePDFs(paths, Path.Combine(folderPathValue.GoodsDir, "goods.pdf"));
        }

        private void CombinePDFs(List<string> PdfPaths, string ExportPath)
        {
            try
            {
                using (FileStream stream = new FileStream(ExportPath, FileMode.Create))
                {
                    var document = new iTextSharp.text.Document();
                    PdfCopy pdfCopy = new PdfCopy(document, stream);
                    document.Open();

                    foreach (string file in PdfPaths)
                    {
                        var reader = new iTextSharp.text.pdf.PdfReader(file);
                        for (int i = 1; i <= reader.NumberOfPages; i++)
                        {
                            pdfCopy.AddPage(pdfCopy.GetImportedPage(reader, i));
                        }
                        reader.Close();
                    }

                    document.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
        }
        /*public void CreatePdfForApple()
        {
            var AnswerPdfs = Directory.GetFiles(tempriConstants.PdfaDir);
            var QuestionPdfs = Directory.GetFiles(tempriConstants.PdfqDir);
            var PdfPaths = AnswerPdfs.Concat(QuestionPdfs);
            for (var i = 0; i < AnswerPdfs.Length; i++)
            {
                var FileName = $@"{printEntity.PrintId}-{(i + 1).ToString("D3")}";
                var paths = PdfPaths.Where(path => path.Contains(FileName)).ToList();
                CombinePDFs(paths, $@"{tempriConstants.PdfDir}\{FileName}.pdf");
            }
        }*/

        private IEnumerable<IEnumerable<T>> GroupByChunk<T>(IEnumerable<T> source, int chunkSize)
        {
            while (source.Any())
            {
                yield return source.Take(chunkSize);
                source = source.Skip(chunkSize);
            }
        }
        private void DrawPdfPage(XGraphics gfx, string filePath, double x, double y, double width, double height)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                // 空のPDFの場合、白い矩形を描画
                gfx.DrawRectangle(XBrushes.White, x, y, width, height);
            }
            else
            {
                // PDFページを読み込む
                XPdfForm form = XPdfForm.FromFile(filePath);
                gfx.DrawImage(form, x, y, width, height);
            }
        }

    }
}
