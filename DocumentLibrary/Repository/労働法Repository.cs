using Google.Apis.Docs.v1;
using DocumentLibrary.Config;
using LawTextDomain.Entity;
using Google.Apis.Docs.v1.Data;


namespace DocumentLibrary.Repository
{
    public class 労働法Repository
    {
        private readonly DocsService docsService;
        private Dictionary<string, string> dictionary = new Dictionary<string, string>()
        {
            { "1juUkUUVUAv9AEvbTcJh4vv2fzORSp52AyEHxN43MxY8","労働安全衛生法" } ,
            {"1AF9vf14r1H8PaG3jEmRIm7k0Em-OLN8OqN8BLUzSSeg", "労働法"}
        };

        public 労働法Repository(DocumentConnector docsConnecter)
        {
            docsService = docsConnecter.GetDocsService();
        }
        public void ExportHtml()
        {
            File.WriteAllText(@"C:\drive\work\web\Release\exam\労働法(条文付・ランダム).html", ConvertTextToHtml(GetWholeRandomHtml(), "労働法(条文付・ランダム)"));
            File.WriteAllText(@"C:\drive\work\web\Release\exam\労働法(条文抜・ランダム).html", ConvertTextToHtml(GetSummaryRandomHtml(), "労働法(条文抜・ランダム)"));
            File.WriteAllText(@"C:\drive\work\web\Release\exam\労働法(条文抜・順序).html", ConvertTextToHtml(GetSummaryOrderedHtml(), "労働法(条文抜・順序)"));
            File.WriteAllText(@"C:\drive\work\web\Release\exam\労働法(条文付・順序).html", ConvertTextToHtml(GetWholeOrderedHtml(), "労働法(条文付・順序)"));

        }
        private string ConvertTextToHtml(string text, string title = "Document")
        {
            // HTML テンプレート
            return $@"<!DOCTYPE html>
                <html lang=""ja"">
                <head>
                    <link rel=""stylesheet"" href=""style.css"">
                    <meta charset=""utf-8"">
                    <title>{title}</title>
                </head>
                <body>
                {text}
                </body>
                </html>";
        }
        /// <summary>
        /// ドキュメントの内容を取得し、条件に従ってエンティティに代入して処理します。
        /// </summary>
        public List<PointEntity> FormatDoc2PointEntity()
        {
            List<PointEntity> blocks = new();
            string currentCaption = null;
            string currentRawId = null;
            foreach (var documentId in dictionary.Keys)
            {
                var content = GetDocumentText(documentId);
                foreach (var element in content.Skip(1)) // 最初の行をスキップ
                {
                    if (element.Paragraph?.Elements == null) continue;

                    // 段落内のすべてのテキストを結合
                    var text = string.Join("", element.Paragraph.Elements
                        .Where(e => e.TextRun?.Content != null)
                        .Select(e => e.TextRun.Content.Replace("\n", "")));

                    if (string.IsNullOrEmpty(text)) continue;
                    if (text.Contains("附　則")) break;

                    // （〇〇）をCaptionとして検出し、保持
                    if (text.StartsWith("（") && text.EndsWith("）"))
                    {
                        currentCaption = text;
                        continue;
                    }

                    // 第〇条をRawIdとして検出し、保持
                    if (text.StartsWith("第") && text.Contains("条") && text.Length < 20)
                    {
                        currentRawId = text;
                        continue;
                    }
                    // 章の始まりではCaptionとRawIdをリセット
                    if (text.StartsWith("第") && text.Contains("章") && text.Length < 30)
                    {
                        currentCaption = "";
                        currentRawId = "";
                        continue;
                    }

                    // [第〇条]形式の検出
                    string textType = text.StartsWith("[第") && text.Contains("]") ? "解釈" : "条文";

                    if (textType == "解釈")
                    {
                        // [第〇条]形式のテキストを処理
                        int startIndex = text.IndexOf('[');
                        int endIndex = text.IndexOf(']');

                        if (startIndex >= 0 && endIndex > startIndex)
                        {
                            text = text.Remove(startIndex, endIndex - startIndex + 1);
                        }
                    }

                    // エンティティを作成
                    blocks.Add(new PointEntity
                    {
                        LawName = dictionary[documentId],
                        Caption = currentCaption,
                        LawId = currentRawId,
                        TextType = textType,
                        Content = text
                    });
                }
            }
            return blocks;
        }
        public string GetWholeRandomHtml()
        {
            // LawIdごとにグループ化
            var blocks = FormatDoc2PointEntity();
            var groupedBlocks = blocks
                .GroupBy(b => new { b.LawId, b.LawName }) // LawIdでグループ化
                .Where(group => group.Any(b => b.TextType == "解釈")) // 解釈が1つもないグループを除外
                .OrderBy(_ => Guid.NewGuid())
                .ToList();

            // フォーマット結果を生成
            List<string> result = new();
            List<string> sectionContent = new(); // 現在のセクションの内容
            int currentLineCount = 0; // 現在のセクションの行数

            foreach (var group in groupedBlocks)
            {
                // グループ内の全ブロックを処理
                List<string> groupContent = new();
                int groupLineCount = 0;

                // グループごとにCaptionとLawIdを一度だけ出力
                var firstBlock = group.First();
                if (!string.IsNullOrEmpty(firstBlock.LawId) && !string.IsNullOrEmpty(firstBlock.Caption))
                {
                    var header = $"<h2>{firstBlock.LawName}　{firstBlock.LawId} {firstBlock.Caption}</h2>";
                    groupContent.Add(header);
                    groupLineCount += CalculateLines(header, 60);
                }

                // 条文を全て取得し、まとめて <pre> タグで囲む
                var articles = group
                    .Where(b => b.TextType == "条文")
                    .Select(b => b.Content)
                    .ToList();

                if (articles.Any())
                {
                    var articleText = $"<pre>{string.Join("\n", articles)}</pre>";
                    groupContent.Add(articleText);
                    groupLineCount += articles.Sum(a => CalculateLines(a, 60));
                }

                // 解釈をすべて出力
                foreach (var block in group.Where(b => b.TextType == "解釈"))
                {
                    var paragraph = $"<p>{block.Content.Trim()}</p>";
                    groupContent.Add(paragraph);
                    groupLineCount += CalculateLines(block.Content.Trim(), 60);
                }

                // グループを追加する前に行数を確認
                if (currentLineCount + groupLineCount > 40)
                {
                    // 現在のセクションを閉じて新しいセクションを開始
                    result.Add($"<section>{string.Join("\n", sectionContent)}</section>");
                    sectionContent.Clear();
                    currentLineCount = 0;
                }

                // グループ内容を現在のセクションに追加
                sectionContent.AddRange(groupContent);
                currentLineCount += groupLineCount;
            }

            // 最後のセクションを閉じる
            if (sectionContent.Any())
            {
                result.Add($"<section>{string.Join("\n", sectionContent)}</section>");
            }

            return string.Join("\n\n", result.Where(r => !string.IsNullOrEmpty(r))); // 空行を整理して出力
        }

        public string GetSummaryRandomHtml()
        {
            // LawIdごとにグループ化
            var blocks = FormatDoc2PointEntity();
            var groupedBlocks = blocks
                .Where(block => block.TextType == "解釈")
                .GroupBy(b => new { b.LawId, b.LawName }) // LawIdでグループ化
                .OrderBy(_ => Guid.NewGuid())
                .ToList();

            // フォーマット結果を生成
            List<string> result = new();
            List<string> sectionContent = new(); // 現在のセクションの内容
            int currentLineCount = 0; // 現在のセクションの行数

            foreach (var group in groupedBlocks)
            {
                // グループ内の全ブロックを処理
                List<string> groupContent = new();
                int groupLineCount = 0;

                // グループごとにCaptionとLawIdを一度だけ出力
                var firstBlock = group.First();
                if (!string.IsNullOrEmpty(firstBlock.LawId) && !string.IsNullOrEmpty(firstBlock.Caption))
                {
                    var header = $"<h2>{firstBlock.LawName} {firstBlock.LawId} {firstBlock.Caption}</h2>";
                    groupContent.Add(header);
                    groupLineCount += CalculateLines(header, 60);
                }

                // 解釈をすべて出力
                foreach (var block in group.Where(b => b.TextType == "解釈"))
                {
                    var paragraph = $"<p>{block.Content.Trim()}</p>";
                    groupContent.Add(paragraph);
                    groupLineCount += CalculateLines(block.Content.Trim(), 60);
                }

                // グループを追加する前に行数を確認
                if (currentLineCount + groupLineCount > 50)
                {
                    // 現在のセクションを閉じて新しいセクションを開始
                    result.Add($"<section>{string.Join("\n", sectionContent)}</section>");
                    sectionContent.Clear();
                    currentLineCount = 0;
                }

                // グループ内容を現在のセクションに追加
                sectionContent.AddRange(groupContent);
                currentLineCount += groupLineCount;
            }

            // 最後のセクションを閉じる
            if (sectionContent.Any())
            {
                result.Add($"<section>{string.Join("\n", sectionContent)}</section>");
            }

            return string.Join("\n\n", result.Where(r => !string.IsNullOrEmpty(r))); // 空行を整理して出力
        }
        public string GetSummaryOrderedHtml()
        {
            // LawIdごとにグループ化
            var blocks = FormatDoc2PointEntity();
            var groupedBlocks = blocks
                .Where(block => block.TextType == "解釈")
                .GroupBy(b => new { b.LawId, b.LawName }) // LawIdでグループ化
                .ToList();

            // フォーマット結果を生成
            List<string> result = new();
            List<string> sectionContent = new(); // 現在のセクションの内容
            int currentLineCount = 0; // 現在のセクションの行数

            foreach (var group in groupedBlocks)
            {
                // グループ内の全ブロックを処理
                List<string> groupContent = new();
                int groupLineCount = 0;

                // グループごとにCaptionとLawIdを一度だけ出力
                var firstBlock = group.First();
                if (!string.IsNullOrEmpty(firstBlock.LawId) && !string.IsNullOrEmpty(firstBlock.Caption))
                {
                    var header = $"<h2>{firstBlock.LawName} {firstBlock.LawId} {firstBlock.Caption}</h2>";
                    groupContent.Add(header);
                    groupLineCount += CalculateLines(header, 60);
                }

                // 解釈をすべて出力
                foreach (var block in group.Where(b => b.TextType == "解釈"))
                {
                    var paragraph = $"<p>{block.Content.Trim()}</p>";
                    groupContent.Add(paragraph);
                    groupLineCount += CalculateLines(block.Content.Trim(), 60);
                }

                // グループを追加する前に行数を確認
                if (currentLineCount + groupLineCount > 50)
                {
                    // 現在のセクションを閉じて新しいセクションを開始
                    result.Add($"<section>{string.Join("\n", sectionContent)}</section>");
                    sectionContent.Clear();
                    currentLineCount = 0;
                }

                // グループ内容を現在のセクションに追加
                sectionContent.AddRange(groupContent);
                currentLineCount += groupLineCount;
            }

            // 最後のセクションを閉じる
            if (sectionContent.Any())
            {
                result.Add($"<section>{string.Join("\n", sectionContent)}</section>");
            }

            return string.Join("\n\n", result.Where(r => !string.IsNullOrEmpty(r))); // 空行を整理して出力
        }
        public string GetWholeOrderedHtml()
        {
            // LawIdごとにグループ化
            var blocks = FormatDoc2PointEntity();
            var groupedBlocks = blocks
                .GroupBy(b => new { b.LawId, b.LawName }) // LawIdでグループ化
                .Where(group => group.Any(b => b.TextType == "解釈")) // 解釈が1つもないグループを除外
                .ToList();

            // フォーマット結果を生成
            List<string> result = new();
            List<string> sectionContent = new(); // 現在のセクションの内容
            int currentLineCount = 0; // 現在のセクションの行数

            foreach (var group in groupedBlocks)
            {
                // グループ内の全ブロックを処理
                List<string> groupContent = new();
                int groupLineCount = 0;

                // グループごとにCaptionとLawIdを一度だけ出力
                var firstBlock = group.First();
                if (!string.IsNullOrEmpty(firstBlock.LawId) && !string.IsNullOrEmpty(firstBlock.Caption))
                {
                    var header = $"<h2>{firstBlock.LawName}　{firstBlock.LawId} {firstBlock.Caption}</h2>";
                    groupContent.Add(header);
                    groupLineCount += CalculateLines(header, 60);
                }

                // 条文を全て取得し、まとめて <pre> タグで囲む
                var articles = group
                    .Where(b => b.TextType == "条文")
                    .Select(b => b.Content)
                    .ToList();

                if (articles.Any())
                {
                    var articleText = $"<pre>{string.Join("\n", articles)}</pre>";
                    groupContent.Add(articleText);
                    groupLineCount += articles.Sum(a => CalculateLines(a, 60));
                }

                // 解釈をすべて出力
                foreach (var block in group.Where(b => b.TextType == "解釈"))
                {
                    var paragraph = $"<p>{block.Content.Trim()}</p>";
                    groupContent.Add(paragraph);
                    groupLineCount += CalculateLines(block.Content.Trim(), 60);
                }

                // グループを追加する前に行数を確認
                if (currentLineCount + groupLineCount > 40)
                {
                    // 現在のセクションを閉じて新しいセクションを開始
                    result.Add($"<section>{string.Join("\n", sectionContent)}</section>");
                    sectionContent.Clear();
                    currentLineCount = 0;
                }

                // グループ内容を現在のセクションに追加
                sectionContent.AddRange(groupContent);
                currentLineCount += groupLineCount;
            }

            // 最後のセクションを閉じる
            if (sectionContent.Any())
            {
                result.Add($"<section>{string.Join("\n", sectionContent)}</section>");
            }

            return string.Join("\n\n", result.Where(r => !string.IsNullOrEmpty(r))); // 空行を整理して出力
        }

        //Googleドキュメントのデータを取得
        private IList<StructuralElement> GetDocumentText(string documentId)
        {
            var request = docsService.Documents.Get(documentId);
            var document = request.Execute();
            var content = document.Body.Content;
            return content;
        }

        // 1行に収まる文字数（lineWidth）を基準に、テキストが何行かかるか計算
        private int CalculateLines(string text, int lineWidth)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            return (text.Length + lineWidth - 1) / lineWidth; // 切り上げ計算
        }


    }
}
