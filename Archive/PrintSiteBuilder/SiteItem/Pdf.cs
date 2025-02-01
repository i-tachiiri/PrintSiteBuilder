using ExCSS;
using ImageMagick;
using PrintSiteBuilder.Interfaces;
using PrintSiteBuilder.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Svg;
using System.Drawing;
using Svg;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using PrintSiteBuilder.Models.General;

namespace PrintSiteBuilder.SiteItem
{
    public class Pdf
    {
        ItemsConfig itemsConfig;
        IPrint2 iPrint;
        public Pdf(ItemsConfig _itemsConfig, IPrint2 _iPrint)
        {
            itemsConfig = _itemsConfig;
            iPrint = _iPrint;
        }
        public void CreatePdf()
        {
            var bat = new Bat();
            bat.RunBat2("svg-to-pdf-template.txt", false,iPrint.PrintId);
        }
        public void CreateQuestionGoods()
        {
            var i = 0;
            var questions = Directory.GetFiles(iPrint.path.PrintPdfqDir, "*.pdf").ToList();
            var answers = Directory.GetFiles(iPrint.path.PrintPdf4Dir, "*.pdf").ToList();
            var cover = new List<string> { $@"{iPrint.path.PrintCoverDir}\{iPrint.PrintId}-cover.pdf" };
            var paths = cover.Concat(questions.Concat(answers)).ToList();
            CombinePDFs(paths, $@"{iPrint.path.PrintGoodsDir}\goods.pdf");
        }
        public void CombinePDFs(List<string> PdfPaths, string ExportPath)
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
        public void CreatePdfForApple()
        {
            var AnswerPdfs = Directory.GetFiles(iPrint.path.PrintPdfaDir);
            var QuestionPdfs = Directory.GetFiles(iPrint.path.PrintPdfqDir);
            var PdfPaths = AnswerPdfs.Concat(QuestionPdfs);
            for(var i=0;i<AnswerPdfs.Length;i++)
            {
                var FileName = $@"{iPrint.PrintId}-{(i + 1).ToString("D3")}";
                var paths = PdfPaths.Where(path => path.Contains(FileName)).ToList();
                CombinePDFs(paths, $@"{iPrint.path.PrintPdfDir}\{FileName}.pdf");
            }
        }
        public void CreateGroupedImage()
        {
            try
            {
                var AnswerPdfDir = @$"C:\drive\work\www\item\print\{iPrint.PrintId}\pdf-a";
                var pdfFiles = Directory.GetFiles(AnswerPdfDir, "*-a.pdf").ToList();

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
                    outputPdf.Save($@"{iPrint.path.PrintPdf4Dir}\{pdfFiles.Count()}-{i.ToString("D3")}-answer.pdf");
                }               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        public IEnumerable<IEnumerable<T>> GroupByChunk<T>(IEnumerable<T> source, int chunkSize)
        {
            while (source.Any())
            {
                yield return source.Take(chunkSize);
                source = source.Skip(chunkSize);
            }
        }
        public void DrawPdfPage(XGraphics gfx, string filePath, double x, double y, double width, double height)
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
