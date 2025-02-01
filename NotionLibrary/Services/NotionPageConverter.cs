using BlogDomain.Config;
using BlogDomain.Entity;
using NotionLibrary.External;
using System.Text.Json;

namespace NotionLibrary.Services
{
    public class NotionPageConverter
    {
        NotionConnecter notionService;
        NotionDatabaseConverter databaseConveter;
        NotionPropertyConverter propertyConverter;
        
        public NotionPageConverter(NotionConnecter notionService,NotionDatabaseConverter databaseConveter, NotionPropertyConverter propertyConverter)
        {
            this.notionService = notionService;
            this.databaseConveter = databaseConveter;
            this.propertyConverter = propertyConverter;
        }
        public string GetFirstImageUrl(JsonElement blocks, BlogEntity entity,string BlogUrl)
        {
            foreach (var block in blocks.EnumerateArray())
            {
                var type = block.GetProperty("type").GetString();
                if (type == "image")
                {
                    var ImageBlockId = block.GetProperty("id").GetString();
                    var ImageFiles = Directory.GetFiles(entity.LocalFolder, ImageBlockId + "*");
                    if (ImageFiles.Length > 0)
                    {
                        return $"{DomainConstants.Web.BlogUrl}/{entity.Directory}/img/{Path.GetFileName(ImageFiles[0])}";
                    }
                }
            }
            return string.Empty;
        }
        public async Task<string> ConvertBlocksToHtml(JsonElement blocks, string PageId, string PageTitle, string ImageDir, string directory)
        {
            string html = string.Empty;//$@"<h1>{pageTitle}</h1>";
            bool isBulletedListOpen = false;
            bool isNumberedListOpen = false;

            foreach (var block in blocks.EnumerateArray())
            {
                var type = block.GetProperty("type").GetString();
                var blockValue = block.GetProperty(type);
                var blockId = block.GetProperty("id").GetString();
                if (type != "bulleted_list_item" && type != "numbered_list_item")
                {
                    if (isBulletedListOpen)
                    {
                        html += "</ul>";
                        isBulletedListOpen = false;
                    }
                    if (isNumberedListOpen)
                    {
                        html += "</ol>";
                        isNumberedListOpen = false;
                    }
                }
                switch (type)
                {
                    case "paragraph":

                        if (IsAffiliateLink(blockValue))
                        {
                            html += $@"<p style=""text-align:center"">{GetAffiliateLinkHtml(blockValue)}</p>";
                        }
                        else
                        {
                            var content = $"<p>　{ConvertParagraphToHtml(blockValue).Replace("　", "")}</p>";
                            if (content.StartsWith("<p>　▼")) content = content.Replace("<p>　▼", @"<p class=""reference""　▼");
                            if (content.StartsWith("<p>　参照")) content = content.Replace("<p>　参照", @"<p class=""reference"">参照");
                            html += content;
                        }
                        break;
                    case "heading_1":
                        html += $"<h2 id=\"{blockId}\">{GetRichText(blockValue.GetProperty("rich_text"))}</h2>";
                        break;
                    case "heading_2":
                        html += $"<h3 id=\"{blockId}\">{GetRichText(blockValue.GetProperty("rich_text"))}</h3>";
                        break;
                    case "heading_3":
                        html += $"<h4 id=\"{blockId}\">{GetRichText(blockValue.GetProperty("rich_text"))}</h4>";
                        break;
                    case "quote":
                        html += $"<blockquote>{GetRichText(blockValue.GetProperty("rich_text"))}</blockquote>";
                        break;
                    case "divider":
                        break;
                    case "code":
                        var codeContent = GetRichText(blockValue.GetProperty("rich_text"));
                        var normalizedCode = NormalizeCodeIndentation(codeContent);
                        html += $@"<pre class=""language-csharp""><code>{System.Web.HttpUtility.HtmlEncode(normalizedCode)}</code></pre>";
                        break;
                    case "image":
                        html = GetImageTag(ImageDir, html, blockValue, blockId);
                        break;

                    case "link_to_page":
                        html += await GetLinkToPageTag(blockValue,directory, PageTitle);
                        break;
                    case "bookmark":
                        html = await GetBookmarkTag(html, blockValue);
                        break;
                    case "table":
                        var tableRows = notionService.GetTableRows(block.GetProperty("id").GetString());
                        html = GetTableTag(html, tableRows);
                        break;
                    case "bulleted_list_item":
                        if (isNumberedListOpen)
                        {
                            html += "</ol>";
                            isNumberedListOpen = false;
                        }
                        if (!isBulletedListOpen)
                        {
                            html += @"<ul class=""content-ul"">";
                            isBulletedListOpen = true;
                        }
                        html += $"<li>{GetRichText(blockValue.GetProperty("rich_text"))}</li>";
                        break;
                    case "numbered_list_item":
                        if (isBulletedListOpen)
                        {
                            html += "</ul>";
                            isBulletedListOpen = false;
                        }
                        // 番号付きリストを開始する
                        if (!isNumberedListOpen)
                        {
                            html += "<ol>";
                            isNumberedListOpen = true;
                        }
                        html += $"<li>{GetRichText(blockValue.GetProperty("rich_text"))}</li>";
                        break;
                    default:
                        Console.WriteLine($"Unsupported block type: {type}");
                        break;
                }
            }

            // リストが閉じられないまま終了した場合に備えて
            if (isBulletedListOpen)
            {
                html += "</ul>";
            }
            if (isNumberedListOpen)
            {
                html += "</ol>";
            }

            return html;
        }

        private async Task<string> GetBookmarkTag(string html, JsonElement blockValue)
        {
            var bookmarkUrl = blockValue.GetProperty("url").GetString();
            var (Title, description) = await GetWebPageMetadataAsync(bookmarkUrl);
            html += $"<a href=\"{bookmarkUrl}\" target=\"_blank\">";
            html += $"<div class=\"bookmark\">";
            html += $@"<div class= ""bookmark-title"">{Title}</div>";
            if (!string.IsNullOrEmpty(description))
            {
                html += $@"<div class= ""bookmark-description"">{description}</div>";
            }
            html += $"</div></a>";
            return html;
        }
        private async Task<(string title, string description)> GetWebPageMetadataAsync(string url)
        {
            try
            {
                // リダイレクト対応のため HttpClientHandler を使う
                var handler = new HttpClientHandler { AllowAutoRedirect = true };
                using var httpClient = new HttpClient(handler);
                httpClient.Timeout = TimeSpan.FromSeconds(5);
                var response = await httpClient.GetStringAsync(url);

                var htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(response);

                // <title>タグを取得
                var titleNode = htmlDocument.DocumentNode.SelectSingleNode("//title");
                var title = titleNode != null ? titleNode.InnerText : "No Title";

                // <meta name="description">タグを取得
                var descriptionNode = htmlDocument.DocumentNode.SelectSingleNode("//meta[@name='description']");
                var description = descriptionNode != null ? descriptionNode.GetAttributeValue("content", "No Description") : "";

                return (title, description);
            }
            catch (Exception)
            {
                return ("No Title", "No Description");
            }
        }
        //BlgEntity限定になってしまっているので、他のエンティティが出てきたら修正
        private async Task<string> GetLinkToPageTag(JsonElement blockValue,string Directory,string Title)
        {
            string linkedPageId;
            if (blockValue.TryGetProperty("page_id", out var pageIdElement))
            {
                linkedPageId = pageIdElement.GetString();
                return $@"
                    <div class='card'>
                        <div class='card-body'>
                            <a href='../{Directory}/{linkedPageId}.html' class='card-link'>
                                <div class='card-title'>{Title}</div>                                
                            </a>
                        </div>
                    </div>";
            }
            return String.Empty;
        }

        private string GetTableTag(string html, JsonElement tableRows)
        {
            html += "<table>";
            foreach (var row in tableRows.EnumerateArray())
            {
                html += "<tr>";
                if (row.TryGetProperty("table_row", out var tableRow) && tableRow.TryGetProperty("cells", out var cells))
                {
                    foreach (var cellArray in cells.EnumerateArray())
                    {
                        foreach (var cell in cellArray.EnumerateArray())
                        {
                            if (cell.TryGetProperty("text", out var text))
                            {
                                string cellContent = text.GetProperty("content").GetString();
                                if (text.TryGetProperty("link", out var link) && link.ValueKind != JsonValueKind.Null && link.TryGetProperty("url", out var url))
                                {
                                    var linkUrl = url.GetString();
                                    cellContent = $"<a href=\"{linkUrl}\" target=\"_blank\">{cellContent}</a>";
                                }

                                html += $"<td>{cellContent}</td>";
                            }
                            else
                            {
                                html += "<td></td>";
                            }
                        }
                    }
                }

                html += "</tr>";
            }
            html += "</table>";
            return html;
        }

        public string GenerateTableOfContents(JsonElement blocks)
        {
            string tocHtml = String.Empty;//"<nav><ul>";

            foreach (var block in blocks.EnumerateArray())
            {
                var type = block.GetProperty("type").GetString();
                var blockValue = block.GetProperty(type);
                var blockId = block.GetProperty("id").GetString();

                // 見出しのタイプに応じて目次項目を追加
                switch (type)
                {
                    case "heading_1":
                        tocHtml += $"<li><a href=\"#{blockId}\">{GetRichText(blockValue.GetProperty("rich_text"))}</a></li>";
                        break;

                    case "heading_2":
                        tocHtml += $"<li style=\"margin-left: 20px;\"><a href=\"#{blockId}\">{GetRichText(blockValue.GetProperty("rich_text"))}</a></li>";
                        break;

                    case "heading_3":
                        tocHtml += $"<li style=\"margin-left: 40px;\"><a href=\"#{blockId}\">{GetRichText(blockValue.GetProperty("rich_text"))}</a></li>";
                        break;
                }
            }

            //tocHtml += "</ul></nav>";
            return tocHtml;
        }
        private string GetImageTag(string ImageDir, string html, JsonElement blockValue, string? blockId)
        {
            string imageUrl = null;

            if (blockValue.TryGetProperty("file", out JsonElement fileElement) &&
                fileElement.TryGetProperty("url", out JsonElement fileUrlElement))
            {
                // 内部ファイル
                imageUrl = fileUrlElement.GetString();
            }
            else if (blockValue.TryGetProperty("external", out JsonElement externalElement) &&
                     externalElement.TryGetProperty("url", out JsonElement externalUrlElement))
            {
                // 外部ファイル
                imageUrl = externalUrlElement.GetString();
            }

            if (!string.IsNullOrEmpty(imageUrl))
            {
                string FileName = DownloadAndSaveImage(imageUrl, blockId, $@"{ImageDir}\img");  // 保存ディレクトリを指定
                if (!string.IsNullOrEmpty(FileName))
                {
                    html += $"<div><img src=\"img/{FileName}\" alt=\"Notion Image\" /></div>";
                }
            }

            return html;
        }
        private string DownloadAndSaveImage(string imageUrl, string blockId, string saveDirectory)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // 画像をバイト配列として取得
                    var imageBytes = client.GetByteArrayAsync(imageUrl).Result;

                    // 保存ディレクトリが存在しない場合は作成
                    if (!Directory.Exists(saveDirectory))
                    {
                        Directory.CreateDirectory(saveDirectory);
                    }

                    // URLからファイル名を取得し、クエリパラメータを削除
                    string fileNameFromUrl = Path.GetFileName(new Uri(imageUrl).AbsolutePath);
                    string extension = Path.GetExtension(fileNameFromUrl);

                    // 拡張子が見つからない場合の対処
                    if (string.IsNullOrEmpty(extension))
                    {
                        extension = ".jpg"; // デフォルト拡張子を指定（適宜変更）
                    }

                    // ブロックIDを使用してファイル名を生成
                    var fileName = Path.Combine(saveDirectory, $"{blockId}{extension}");

                    // 画像をローカルに保存
                    File.WriteAllBytes(fileName, imageBytes);

                    // 相対パスとして返す（必要に応じてパスを調整）
                    return $"{blockId}{extension}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"画像のダウンロードに失敗しました: {ex.Message}");
                return string.Empty;
            }
        }

        private string NormalizeCodeIndentation(string code)
        {
            var lines = code.Split('\n');
            if (lines.Length == 0) return code;
            var firstLine = lines.FirstOrDefault(line => !string.IsNullOrWhiteSpace(line));
            if (firstLine == null) return code;
            var indent = firstLine.TakeWhile(char.IsWhiteSpace).Count();
            var normalizedLines = lines.Select(line => line.Length > indent ? line.Substring(indent) : line.TrimStart());
            return string.Join("\n", normalizedLines);
        }
        private string ConvertParagraphToHtml(JsonElement paragraph)
        {
            string html = string.Empty;
            var textArray = paragraph.GetProperty("rich_text");

            foreach (var textElement in textArray.EnumerateArray())
            {
                if (textElement.TryGetProperty("plain_text", out JsonElement plainTextElement))
                {
                    string plainText = plainTextElement.GetString();
                    if (textElement.TryGetProperty("href", out JsonElement hrefElement) && !string.IsNullOrEmpty(hrefElement.GetString()))
                    {
                        string href = hrefElement.GetString();
                        html += $"<a href=\"{href}\">{plainText}</a>";
                    }
                    else
                    {
                        html += plainText;
                    }
                }
            }
            return html;
        }
        private bool IsAffiliateLink(JsonElement paragraph)
        {
            var textArray = paragraph.GetProperty("rich_text");

            foreach (var textElement in textArray.EnumerateArray())
            {
                if (textElement.TryGetProperty("href", out JsonElement hrefElement))
                {
                    string href = hrefElement.GetString();
                    if (href != null && href.Contains("moshimo.com"))
                    {
                        return true; // アフィリエイトリンクの場合
                    }
                }
            }
            return false; // 通常のテキストの場合
        }


        private string GetAffiliateLinkHtml(JsonElement paragraph)
        {
            string html = string.Empty;
            var textArray = paragraph.GetProperty("rich_text");

            foreach (var textElement in textArray.EnumerateArray())
            {
                if (textElement.TryGetProperty("plain_text", out JsonElement plainTextElement))
                {
                    string plainText = plainTextElement.GetString();
                    html += plainText;
                }
            }
            html = html.Replace("<a ", @"<a style=""display:block""");
            return html;
        }



        private string GetRichText(JsonElement richTextArray)
        {
            string text = string.Empty;

            foreach (var richText in richTextArray.EnumerateArray())
            {
                text += richText.GetProperty("plain_text").GetString();
            }

            return text;
        }

        private string ConvertBlocksToMarkdown(JsonElement blocks)
        {
            string markdown = string.Empty;

            foreach (var block in blocks.EnumerateArray())
            {
                var type = block.GetProperty("type").GetString();
                var blockValue = block.GetProperty(type);

                switch (type)
                {
                    case "paragraph":
                        markdown += $"　{ConvertParagraphToMarkdown(blockValue)}";
                        break;
                    case "heading_1":
                        markdown += $"# {GetRichText(blockValue.GetProperty("rich_text"))}\n\n";
                        break;
                    case "heading_2":
                        markdown += $"## {GetRichText(blockValue.GetProperty("rich_text"))}\n\n";
                        break;
                    case "heading_3":
                        markdown += $"### {GetRichText(blockValue.GetProperty("rich_text"))}\n\n";
                        break;
                    case "bulleted_list_item":
                        markdown += $"- {GetRichText(blockValue.GetProperty("rich_text"))}\n";
                        break;
                    case "numbered_list_item":
                        markdown += $"1. {GetRichText(blockValue.GetProperty("rich_text"))}\n";
                        break;
                    case "quote":
                        markdown += $"> {GetRichText(blockValue.GetProperty("rich_text"))}\n\n";
                        break;
                    // 他のブロックタイプに応じた処理を追加
                    default:
                        Console.WriteLine($"Unsupported block type: {type}");
                        break;
                }
            }

            return markdown;
        }
        private string ConvertParagraphToMarkdown(JsonElement paragraph)
        {
            string markdown = string.Empty;
            var textArray = paragraph.GetProperty("rich_text");

            foreach (var textElement in textArray.EnumerateArray())
            {
                // "plain_text" を抽出
                if (textElement.TryGetProperty("plain_text", out JsonElement plainTextElement))
                {
                    string plainText = plainTextElement.GetString();

                    // "href" が存在するかチェックし、リンク形式を出力
                    if (textElement.TryGetProperty("href", out JsonElement hrefElement) && !string.IsNullOrEmpty(hrefElement.GetString()))
                    {
                        string href = hrefElement.GetString();
                        // マークダウン形式のリンクに変換
                        markdown += $"[{plainText}]({href})";
                    }
                    else
                    {
                        // 通常のテキストを追加
                        markdown += plainText;
                    }
                }
            }

            // パラグラフの末尾に改行を追加
            return markdown + "\n\n";
        }
    }
}
