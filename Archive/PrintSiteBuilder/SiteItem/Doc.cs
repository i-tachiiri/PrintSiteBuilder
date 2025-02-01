using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PrintSiteBuilder.Models.General;
using PrintSiteBuilder.SiteItem;
using PrintSiteBuilder.Utilities;



namespace PrintSiteBuilder.SiteItem
{
    public class Doc
    {
        public DocsConfig GetDocsConfig(Label progressLabel)
        {
            var docsConfig = new DocsConfig();
            var MarkdownPaths = Directory.GetFiles(GlobalConfig.DocMdDir, $"*.md").ToList();

            var ItemConfigList = new List<DocConfig>();
            var KeysList = new HashSet<string>();
            var TagsList = new HashSet<string>();
            var i = 0;
            foreach (var ItemPath in MarkdownPaths)
            {
                i++;
                progressLabel.Text = $@"[{i}/{MarkdownPaths.Count()}]GetItemsConfig : Get {Path.GetFileNameWithoutExtension(ItemPath)} config...";
                progressLabel.Update();

                var itemConfig = GetDocConfig(ItemPath);
                ItemConfigList.Add(itemConfig);
                KeysList.Add(itemConfig.ItemKey);
                foreach (var tag in itemConfig.Tags)
                {
                    TagsList.Add(tag);
                }
            }
            docsConfig.MarkdownPaths = MarkdownPaths;
            docsConfig.DocImagePaths = Directory.GetFiles(GlobalConfig.DocImageDir, $"*.svg").ToList();
            docsConfig.itemConfigList = ItemConfigList;
            docsConfig.Keys = KeysList;
            docsConfig.Tags = TagsList;


            return docsConfig;
        }
        public DocConfig GetDocConfig(string itemPath)
        {
            var itemConfig = new DocConfig();
            itemConfig.MarkdownPath = itemPath;
            itemConfig.MarkdownName = itemPath.Contains(@"C:\") ? itemConfig.MarkdownName = Path.GetFileNameWithoutExtension(itemPath) : itemPath;
            itemConfig.CategoryName = itemConfig.MarkdownName.Split("-")[0];
            itemConfig.DocImageNames = GetWebpNamesFromLines(File.ReadAllLines(itemPath).Where(line => line.StartsWith("![")).ToList());
            itemConfig.DocImagePaths = itemConfig.DocImageNames.Select(name => $@"{GlobalConfig.DocImageDir}\{name}.svg").ToList();

            var ItemTags = Path.GetFileNameWithoutExtension(itemConfig.MarkdownName).Split("-");
            var Tags = new HashSet<string>();
            var Types = new HashSet<string>();
            foreach (var itemTag in ItemTags)
            {
                if (!Regex.IsMatch(itemTag, @"^\d+$"))
                {
                    Tags.Add(itemTag);
                }
                Types.Add(itemTag);
            }
            itemConfig.Tags = Tags.ToList();
            itemConfig.ItemType = string.Join("-", Tags);  //最大の数-5までの数. sidebar
            itemConfig.ItemKey = string.Join("-", Types);  //最大の数-5までの数-01. html,pdf
            //itemConfig.QrPath = $@"{GlobalConfig.QrDir}\{itemConfig.ItemKey}.png"; --QR作成はどこかでやりたい
            itemConfig.Title = GetTitleFromMd(itemConfig.MarkdownPath);
            itemConfig.Description = GetDescriptionFromMd(itemConfig.MarkdownPath);
            itemConfig.IsGroup = false;
            itemConfig.IsHtmlUpdated = IsHtmlUpdated(itemConfig);
            itemConfig.IsInvalidSvg = false;
            itemConfig.IsVertical = false;
            //itemConfig.IsQrUpdated = IsQrUpdated(itemConfig);
            //itemConfig.IsPdfUpdated = IsPdfUpdated(itemConfig);
            if (itemConfig.CategoryName != "index")
            {
                var ParentFolderName = Directory.GetParent(Directory.GetFiles(GlobalConfig.DocGroupDir, $"{itemConfig.CategoryName}.csv", SearchOption.AllDirectories)[0]).Name;
                itemConfig.GroupName = ParentFolderName.Contains("_") ? ParentFolderName.Split("_")[1] : "新着";
            }
            return itemConfig;
        }
        public List<string> GetWebpNamesFromLines(List<string> MarkdownLines)
        {
            var WebpNames = new List<string>();
            foreach (var line in MarkdownLines)
            {
                var imgLines = MarkdownLines.Where(line => line.StartsWith("![")).ToList();
                var regex = new Regex(@"!\[(.*?)\]\((.*?)\)");
                if (regex.IsMatch(line))
                {
                    var match = regex.Match(line);
                    var altText = match.Groups[1].Value;
                    var imageUrl = match.Groups[2].Value.Replace("../doc-image", "../webp");
                    WebpNames.Add(Path.GetFileNameWithoutExtension(imageUrl));
                }
            }
            return WebpNames;
        }
        public string GetTitleFromMd(string ItemPath)
        {
            var MdTextLines = File.ReadAllLines(ItemPath);
            foreach (var mdTextLine in MdTextLines)
            {
                if (mdTextLine.StartsWith("# "))
                {
                    return mdTextLine.Substring(2).Trim();
                }
            }
            return Path.GetFileNameWithoutExtension(ItemPath);
        }
        public string GetDescriptionFromMd(string ItemPath)
        {
            var MdTextLines = File.ReadAllLines(ItemPath);
            var Description = "";
            foreach (var mdTextLine in MdTextLines)
            {
                if (mdTextLine.StartsWith("# "))
                {
                    continue;
                }
                else if (mdTextLine.StartsWith("#"))
                {
                    return Description;
                }
                else
                {
                    Description += mdTextLine;
                }
            }
            return $"{Path.GetFileNameWithoutExtension(ItemPath)}に関する説明をしているページです。";
        }
        private bool IsHtmlUpdated(DocConfig itemConfig)
        {
            var ReleaseHtml = $@"{GlobalConfig.HtmlDir}\{itemConfig.ItemKey}.html"; // PDFファイルの出力先ディレクトリとファイル名を設定
            var DebugHtml = $@"{GlobalConfig.HtmlTestDir}\{itemConfig.ItemKey}.html";
            if (File.Exists(ReleaseHtml) && File.Exists(DebugHtml))
            {
                var MdFileInfo = new FileInfo(itemConfig.MarkdownPath);
                var DebugHtmlFileInfo = new FileInfo(DebugHtml);
                var ReleaseHtmlFileInfo = new FileInfo(ReleaseHtml);
                if (MdFileInfo.LastWriteTime < ReleaseHtmlFileInfo.LastWriteTime && MdFileInfo.LastWriteTime < DebugHtmlFileInfo.LastWriteTime)
                {
                    return true;
                }
            }
            return false;
        }
        public void CreateDocs(bool UpdateAll, bool IsOnlyTest, DocsConfig docsConfig, Label ProgressLabel)
        {
            var i = 0;
            var KeysHashSet = new HashSet<string>();
            foreach (var itemConfig in docsConfig.itemConfigList)
            {
                i++;
                ProgressLabel.Text = $"[{i}/{docsConfig.itemConfigList.Count()}]CreateDocs : Create {itemConfig.MarkdownName}.html ...";
                ProgressLabel.Update();

                if (!UpdateAll && itemConfig.IsHtmlUpdated)
                {
                    continue;
                }
                if (KeysHashSet.Contains(itemConfig.ItemKey))
                {
                    continue;
                }
                KeysHashSet.Add(itemConfig.ItemKey);
                var Content = CreateDoc(itemConfig, docsConfig);
                var TestContent = Content
                    .Replace("https://tempri.tokyo/page", "https://tempri.tokyo/testpage")
                    .Replace($@"<head>", $@"<head><meta name=""robots"" content=""noindex"">")
                    .Replace($@"<script src=""../js/tracking.js""></script>", $@"<script src=""../js/tracking-test.js""></script>")
                    .Replace($@"<div id='header'>", $@"<div id='header' style=""background-color:#b2b2b2"">");

                if (!IsOnlyTest)
                {
                    File.WriteAllLines($@"{GlobalConfig.DocDir}\{itemConfig.ItemKey}.html", new[] { Content });
                }
                File.WriteAllLines($@"{GlobalConfig.DocTestDir}\{itemConfig.ItemKey}.html", new[] { TestContent });
            }
        }
        public string CreateDoc(DocConfig docConfig, DocsConfig docsConfig)
        {
            var item = new Item();
            var htmlClass = new Html();
            var markdown = new markdown();
            var html = "";
            html += CreateHeadTag(docConfig);
            html += htmlClass.CreateHeader();
            html += htmlClass.CreateSidebar();
            html += markdown.GetHtmlFromMarkdown(docConfig);
            html += CreateRelatedPost(docConfig, docsConfig);
            html += CreateMenu(docConfig, true);
            html += htmlClass.CreateEnd();
            return html;
        }
        public string CreateHeadTag(DocConfig docConfig)
        {
            var html = "";
            html += $@"<!DOCTYPE html>";
            html += $@"<html lang=""ja"">";
            html += $@"<head>";
            html += $@"<meta charset=""UTF-8"">";
            html += $@"<title>{docConfig.Title}</title>";
            html += $@"<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">";
            html += $@"<meta name=""description"" content=""{docConfig.Description}"">";
            html += $@"<meta name=""theme-color"" content=""#000000"">";
            html += $@"<link rel=""manifest"" href=""/manifest.json"">";
            html += $@"<link rel=""stylesheet"" href=""../css/desktop.css"" media=""only screen and (min-width: 768px)"">";
            html += $@"<link rel=""stylesheet"" href=""../css/mobile.css"" media=""only screen and (max-width: 768px)"">";
            html += $@"<link rel=""icon"" href=""../icon/favicon.svg"" type=""image/x-icon"">";
            html += $@"<style>@import url('https://fonts.googleapis.com/css2?family=Noto+Sans+JP:wght@100..900&display=swap');</style>";
            html += CreateOrgDara(docConfig);
            html += $@"<script type=""text/javascript"">(function (f, b) {{ if (!b.__SV) {{ var e, g, i, h; window.mixpanel = b; b._i = []; b.init = function (e, f, c) {{ function g(a, d) {{ var b = d.split("".""); 2 == b.length && ((a = a[b[0]]), (d = b[1])); a[d] = function () {{ a.push([d].concat(Array.prototype.slice.call(arguments, 0))); }}; }} var a = b; ""undefined"" !== typeof c ? (a = b[c] = []) : (c = ""mixpanel""); a.people = a.people || []; a.toString = function (a) {{ var d = ""mixpanel""; ""mixpanel"" !== c && (d += ""."" + c); a || (d += "" (stub)""); return d; }}; a.people.toString = function () {{ return a.toString(1) + "".people (stub)""; }}; i = ""disable time_event track track_pageview track_links track_forms track_with_groups add_group set_group remove_group register register_once alias unregister identify name_tag set_config reset opt_in_tracking opt_out_tracking has_opted_in_tracking has_opted_out_tracking clear_opt_in_out_tracking start_batch_senders people.set people.set_once people.unset people.increment people.append people.union people.track_charge people.clear_charges people.delete_user people.remove"".split("" ""); for (h = 0; h < i.length; h++) g(a, i[h]); var j = ""set set_once union unset remove delete"".split("" ""); a.get_group = function () {{ function b(c) {{ d[c] = function () {{ call2_args = arguments; call2 = [c].concat(Array.prototype.slice.call(call2_args, 0)); a.push([e, call2]); }}; }} for (var d = {{}}, e = [""get_group""].concat(Array.prototype.slice.call(arguments, 0)), c = 0; c < j.length; c++) b(j[c]); return d; }}; b._i.push([e, f, c]); }}; b.__SV = 1.2; e = f.createElement(""script""); e.type = ""text/javascript""; e.async = !0; e.src = ""undefined"" !== typeof MIXPANEL_CUSTOM_LIB_URL ? MIXPANEL_CUSTOM_LIB_URL : ""file:"" === f.location.protocol && ""//cdn.mxpnl.com/libs/mixpanel-2-latest.min.js"".match(/^\/\//) ? ""https://cdn.mxpnl.com/libs/mixpanel-2-latest.min.js"" : ""//cdn.mxpnl.com/libs/mixpanel-2-latest.min.js""; g = f.getElementsByTagName(""script"")[0]; g.parentNode.insertBefore(e, g); }} }})(document, window.mixpanel || []);</script>";
            html += $@"</head>";
            html += $@"<body>";
            return html;
        }
        public string CreateOrgDara(DocConfig itemConfig)
        {
            var mdInfo = new FileInfo($@"{GlobalConfig.DocMdDir}\{itemConfig.MarkdownName}.md");
            var publishedDate = mdInfo.CreationTime.ToString("yyyy-MM-dd");
            var uploadDate = mdInfo.LastWriteTime.ToString("yyyy-MM-dd");
            var html = "";
            html += $@"<script type=""application/ld+json"">";
            html += $@"{{";
            html += $@"""@context"": ""http://schema.org"",";
            html += $@"""@type"": ""WebPage"",";
            html += $@"""name"": ""{itemConfig.Title}"",";
            html += $@"""description"": ""{itemConfig.Description}"",";
            html += $@"""url"": ""https://tempri.tokyo/doc/{itemConfig.ItemKey}.html"",";
            html += $@"""publisher"": {{";
            html += $@"""@type"": ""Person"",";
            html += $@"""name"": ""立入"",";
            html += $@"""url"": ""https://tempri.tokyo/doc/{itemConfig.ItemKey}.html""";
            html += $@"}},";
            html += $@"""datePublished"": ""{publishedDate}"",";
            html += $@"""dateModified"": ""{uploadDate}"",";
            html += $@"""breadcrumb"": {{";
            html += $@"""@type"": ""BreadcrumbList"",";
            html += $@"""itemListElement"": [{{";
            html += $@"""@type"": ""ListItem"",";
            html += $@"""position"": 1,";
            html += $@"""item"": {{";
            html += $@"""@id"": ""https://tempri.tokyo"",";
            html += $@"""name"": ""Home""";
            html += $@"}}";
            html += $@"}},{{";
            html += $@"""@type"": ""ListItem"",";
            html += $@"""position"": 2,";
            html += $@"""item"": {{";
            html += $@"""@id"": ""https://tempri.tokyo/doc"",";
            html += $@"""name"": ""doc""";
            html += $@"}}";
            html += $@"}}]}}}}";
            html += $@"</script>";

            return html;
        }
        public string CreateRelatedPost(DocConfig itemConfig, DocsConfig itemsConfig)
        {
            var html = "";
            var tagNumber = 0;
            /*var CategoryDirectory = Directory.GetDirectories($@"{GlobalConfig.DocGroupDir}", $"*{itemConfig.GroupName}", SearchOption.AllDirectories)[0];
            var categoryNames = Directory.GetFiles(CategoryDirectory, "*.csv", SearchOption.AllDirectories).Select(path => Path.GetFileNameWithoutExtension(path)).ToList();*/

            var GroupDirectoryPaths = Directory.GetDirectories(GlobalConfig.DocGroupDir);
            foreach (var groupDirectoryPath in GroupDirectoryPaths)
            {
                var item = new Item();
                var GroupName = Path.GetFileNameWithoutExtension(groupDirectoryPath).Split("_")[1];
                var relatedItemsConfigList = itemsConfig.itemConfigList.Where(itemConfig => itemConfig.GroupName == GroupName).ToList();//item.GeRelatedtItemsConfig(categoryName);
                if (relatedItemsConfigList.Count() == 0)
                {
                    continue;
                }
                html += $@"<div class=""related-post-search"">";
                html += $@"{GroupName}";
                html += $@"</div>";
                html += $@"<div class=""content-area-group"">";

                var i = 0;
                foreach (var relatedItemConfig in relatedItemsConfigList)
                {
                    //var itemConfigTemp = item.GetItemConfig(itemPath);
                    html += $@"<a href=""./{relatedItemConfig.ItemKey}.html"">";
                    html += $@"<div class=""content-placeholder-group"">";
                    html += $@"<img src=""../icon/doc-image.png"" alt=""{relatedItemConfig.Description}"" class=""content-image-group"" loading=""lazy"">";
                    html += $@"<div class=""content-text-group"">";
                    html += $@"<div class=""content-text-line"">{relatedItemConfig.Title}</div>";
                    html += $@"</div>";
                    html += $@"</div>";
                    html += $@"<div class=""content-placeholder-group-mobile"">";
                    html += $@"<img src=""../icon/doc-image.png"" alt=""{relatedItemConfig.Description}"" class=""content-image-group"" loading=""lazy"">";
                    html += $@"<div class=""content-text-group"">";
                    html += $@"<div class=""content-text-line"">{relatedItemConfig.Title}</div>";
                    html += $@"</div>";
                    html += $@"</div>";
                    html += $@"</a>";
                    i++;
                    if (i > 12)
                    {
                        break;
                    }
                }
                html += $@"</div>";
                //html += tagNumber == 0 ? CreateRakutenItems(itemConfig) : "";
                tagNumber++;
            }
            html += $@"<div id = 'header-placeholder'></div>";
            return html;
        }
        public string CreateMenu(DocConfig itemConfig, bool IsIndex)
        {
            //var itemName = itemConfig.ItemName;
            var html = "";
            html += $@"<div id = 'menu-placeholder'>";
            if (!IsIndex)
            {
                html += $@"<div class= ""footer-menu-title"">{itemConfig.ItemKey}</div>";
            }
            html += $@"<div id = ""focus-icons"">";
            //html += $@"<img class=""focus-icon mobile-only"" src=""../icon/clipboard.svg"" alt=""クリップボードへURLをコピー"" onclick=""copyCurrentUrlToClipboard(this)"" style=""cursor: pointer;"">";
            html += $@"<img class=""focus-icon mobile-only"" src=""../icon/share.svg"" alt=""共有"" onclick=""sharePage(this)"" />";
            html += $@"<img class=""focus-icon desktop-only"" src=""../icon/share-desktop.svg"" alt=""共有"" onclick=""sharePage(this)"" />";
            if (!IsIndex)
            {
                html += $@"<img class= ""focus-icon desktop-only without-apple"" src = ""../icon/print.svg"" alt = ""プリントを印刷""  onclick=""printAttempt(this);""/>";
                html += $@"<img class= ""focus-icon mobile-only without-apple"" src = ""../icon/print-mobile.svg"" alt = ""プリントを印刷""  onclick=""printAttempt(this);""/>";

                html += $@"<a class= ""icons-link desktop-only"" href = ""../pdf/{itemConfig.ItemKey}.pdf"" target=""_blank"" onclick=""trackPdfDownload(this)"">";
                html += $@"<img class= ""focus-icon"" src = ""../icon/pdf.svg"" alt = ""PDFをダウンロード"" />";
                html += $@"</a>";
                html += $@"<a class= ""icons-link mobile-only"" href = ""../pdf/{itemConfig.ItemKey}.pdf"" target=""_blank"" onclick=""trackPdfDownload(this)"">";
                html += $@"<img class= ""focus-icon"" src = ""../icon/pdf-mobile.svg"" alt = ""PDFをダウンロード"" />";
                html += $@"</a>";

                //html += $@"<img class=""focus-icon desktop-only"" src=""../icon/CopyImage.svg"" alt=""クリップボードへ画像をコピー"" onclick=""copyImageToClipboard(this)"" style=""cursor: pointer;"">";
                //html += $@"<img class=""focus-icon mobile-only"" src=""../icon/CopyImage-mobile.svg"" alt=""クリップボードへ画像をコピー"" onclick=""copyImageToClipboard(this)"" style=""cursor: pointer;"">";
                /*html += $@"<a class= ""icons-link"" href = ""../png/{itemName}.png"" download onclick=""trackPdfDownload(this)"">";
                html += $@"<img class= ""focus-icon"" src = ""../icon/png.svg"" alt = ""PNGをダウンロード"" />";
                html += $@"</a>";*/
                /*html += $@"<a class= ""icons-link desktop-only"" href = ""../jpeg/{itemName}.jpeg"" download>";
                html += $@"<img class= ""focus-icon desktop-only"" src = ""../icon/jpeg.svg"" alt = ""JPEGをダウンロード"" />";
                html += $@"</a>";*/
                /*html += $@"<a class= ""icons-link desktop-only"" href = ""../svg/{itemName}.svg"" download>";
                html += $@"<img class= ""focus-icon desktop-only"" src = ""../icon/svg.svg"" alt = ""SVGをダウンロード"" />";
                html += $@"</a>";*/
                /*html += $@"<a class= ""icons-link desktop-only"" href = ""../tiff/{itemName}.tiff"" download>";
                html += $@"<img class= ""focus-icon desktop-only"" src = ""../icon/tiff.svg"" alt = ""TIFFをダウンロード"" />";
                html += $@"</a>";*/
            }
            html += $@"<div id=""customPopup"" class=""custom-popup"" style=""display: none;""></div>";
            html += $@"<img class= ""focus-icon icon-search"" src = ""../icon/search.svg"" alt = ""プリントを検索""  onclick = ""switchSidebar()"" />";
            html += $@"</div>";
            html += $@"</div>";
            html += $@"</div>";
            return html;
        }



    }
}
