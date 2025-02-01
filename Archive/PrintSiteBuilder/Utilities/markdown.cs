using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PrintSiteBuilder.Models.General;

namespace PrintSiteBuilder.Utilities
{
    public class markdown
    {
        bool IsListItem = false;
        public string GetHtmlFromMarkdown(DocConfig docConfig)
        {
            var MarkdownLines = File.ReadAllLines(docConfig.MarkdownPath);
            var html = "";
            html += $@"<div id=""main"">";
            html += $@"<div id=""search-result-window""></div>";
            html += $@"<div id='search-form-placeholder'>";
            html += $@"<form id=""search-form"" action=""javascript:void(0);"" class=""search-form"">";
            html += $@"<button class=""search-button"" type=""submit"" aria-label=""検索""></button>";
            html += $@"<input class=""search-form-input"" type=""text"" placeholder=""こちらでプリントを検索できます。"" aria-label=""検索"">";
            html += $@"</form>";
            html += $@"</div>";
            //html += $@"<div class=""content-title"">{itemConfig.ItemKey}</div>"; 
            if (MarkdownLines.Any(line => line.StartsWith("![")))
            {
                var imgLines = MarkdownLines.Where(line => line.StartsWith("![")).ToList();
                var regex = new Regex(@"!\[(.*?)\]\((.*?)\)");
                var CarouselImages = "";
                var CarouselImageNames = new List<string>();
                for (var i = 0; i < imgLines.Count(); i++)
                {
                    var match = regex.Match(imgLines[i]);
                    var altText = match.Groups[1].Value;
                    var imageUrl = match.Groups[2].Value.Replace("../doc-image", "../webp");
                    if (i == 0)
                    {
                        CarouselImages = $@"<img src=""{imageUrl.Replace("svg", "webp")}"" alt=""{altText}"" class=""content-image-single"">";
                    }
                    CarouselImageNames.Add(Path.GetFileNameWithoutExtension(imageUrl));
                }
                var ItemsListString = CarouselImageNames.Select(item => $"'{item}'").ToList();
                var ItemsString = string.Join(",", ItemsListString);
                html += $@"<div class=""content-area"">";
                html += $@"<img class=""slide-image-left-doc"" src=""../icon/slide-image-last.svg"" onclick=""updateImageDisplay([{ItemsString}],false,true)"">";
                html += $@"<div class=""content-placeholder-multi carousel-display""  data-index=""0"">";
                html += CarouselImages;
                html += $@"</div>";
                html += $@"<img class=""slide-image-right-doc"" id=""next"" src=""../icon/slide-image-next.svg"" onclick=""updateImageDisplay([{ItemsString}],true,true)"">";
                html += $@"</div>";
                html += $@"<div class=""adsense-area"">";
                html += $@"<div class=""adsense-placeholder-large""></div>";
                html += $@"</div>";
            }
            html += $@"<div class=""content-area-doc"">";
            foreach (var markdownLine in MarkdownLines)
            {
                html += ReplaceMarkdownToHtml(markdownLine);
            }
            html += $@"</div>";
            return html;
        }
        public string ReplaceMarkdownToHtml(string markdownLine)
        {
            var html = "";
            if (markdownLine.StartsWith("- "))
            {
                if (!IsListItem)
                {
                    html += "<ul>";
                    IsListItem = true;
                }
                var content = markdownLine.Substring(2).TrimStart();
                html += $"<li>{content}</li>";
                return html;
            }
            if (IsListItem && !markdownLine.StartsWith("- "))
            {
                html += "</ul>";
                IsListItem = false;
                return html;
            }
            if (markdownLine.TrimStart().StartsWith("<") && markdownLine.TrimEnd().EndsWith(">"))
            {
                return markdownLine;
            }
            else if (markdownLine.StartsWith("# "))
            {
                return "";
            }
            else if (markdownLine.StartsWith("#"))
            {
                var i = 0;
                foreach (char c in markdownLine)
                {
                    if (c == '#')
                    {
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
                i = Math.Min(i, 7);
                string headerText = markdownLine.Substring(i).TrimStart();
                html += i < 2 ? $"" : $"<h{i - 1}>{headerText}</h{i - 1}>";
                return html;
            }
            else if (markdownLine.StartsWith("!["))
            {
                // 正規表現を使用してMarkdown画像タグから情報を抽出
                var regex = new Regex(@"!\[(.*?)\]\((.*?)\)");
                var match = regex.Match(markdownLine);

                if (match.Success)
                {
                    var altText = match.Groups[1].Value;
                    var imageUrl = match.Groups[2].Value;
                    return $"<img class=\"content-image-doc-single\" alt=\"{altText}\" src=\"../webp/{Path.GetFileNameWithoutExtension(imageUrl)}.webp\">";
                }
                return "";
            }
            else if (markdownLine == "")
            {
                return "";
            }
            else
            {
                html += $@"<p>{markdownLine}</p>";
                return html;
            }

        }

    }
}
