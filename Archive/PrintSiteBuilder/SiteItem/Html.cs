using PrintSiteBuilder.Models.General;
using PrintSiteBuilder.Archive;

namespace PrintSiteBuilder.SiteItem
{
    public class Html
    {
        public void CreateHtmls(bool UpdateAll, bool IsOnlyTest, ItemsConfig itemsConfig) //SVG検索してHasAllItemsからのCreateHtml呼び出してファイル書き込み
        {
            var i = 0;
            var KeysHashSet = new HashSet<string>();
            foreach (var itemConfig in itemsConfig.itemConfigList)
            {
                i++;
                Console.WriteLine($"[{i}/{itemsConfig.itemConfigList.Count()}]CreateHtmls : Create {itemConfig.ItemName}.html ...");

                if (!UpdateAll && itemConfig.IsHtmlUpdated)
                {
                    continue;
                }
                if (KeysHashSet.Contains(itemConfig.ItemKey))
                {
                    continue;
                }
                KeysHashSet.Add(itemConfig.ItemKey);
                if (itemConfig.IsInvalidSvg) continue;
                var Content = CreateHtml(itemConfig, itemsConfig);
                var TestContent = Content
                    .Replace("https://tempri.tokyo/page", "https://tempri.tokyo/testpage")
                    .Replace($@"<head>", $@"<head><meta name=""robots"" content=""noindex"">")
                    .Replace($@"<script src=""../js/tracking.js""></script>", $@"<script src=""../js/tracking-test.js""></script>")
                    .Replace($@"<div id='header'>", $@"<div id='header' style=""background-color:#b2b2b2"">");

                if (!IsOnlyTest)
                {
                    File.WriteAllLines($@"{GlobalConfig.HtmlDir}\{itemConfig.ItemKey}.html", new[] { Content });
                }
                File.WriteAllLines($@"{GlobalConfig.HtmlTestDir}\{itemConfig.ItemKey}.html", new[] { TestContent });
            }
            CreateIndexHtml();
        }

        public string CreateHtml(ItemConfig itemConfig, ItemsConfig itemsConfig)
        {
            var item = new Item();
            var html = "";
            html += CreateHeadTag(itemConfig);
            html += CreateHeader();
            html += CreateSidebar();
            html += CreateMainContent(itemConfig);
            html += CreateRelatedPost(itemConfig, itemsConfig);
            html += CreateMenu(itemConfig, false);
            html += CreateEnd();
            return html;
        }
        public void CreateIndexHtml()
        {
            var item = new Item();
            var itemConfig = item.GetItemConfig("index");
            var categories = Directory.GetFiles(GlobalConfig.HtmlDir, "*.html", SearchOption.AllDirectories)
                                            .Select(path => Path.GetFileNameWithoutExtension(path))
                                            .Where(itemName => File.Exists($@"{GlobalConfig.WebpDir}\{itemName}.webp"))
                                            .Where(category => category != "config" && category != "index" && !category.Contains("-"));

            var html = "";
            html += CreateHeadTag(itemConfig);
            html += CreateHeader();
            html += CreateSidebar();
            html += CreateIndexRelatedPost();
            html += CreateMenu(itemConfig, true);
            html += CreateEnd();
            html = html.Replace("../", "./");

            File.WriteAllLines($@"{GlobalConfig.ItemDir}\index.html", new[] { html });
            /*html += $@"<div id=""main"">";
            html += $@"<div id=""search-result-window""></div>";
            html += $@"<div id='search-form-placeholder'>";
            html += $@"<form id=""search-form"" action=""javascript:void(0);"" class=""search-form"">";
            html += $@"<button class=""search-button"" type=""submit"" aria-label=""検索""></button>";
            html += $@"<input class=""search-form-input"" type=""text"" placeholder=""こちらでプリントを検索できます。"" aria-label=""検索"">";
            html += $@"</form>";
            html += $@"</div>";
            html += $@"<div id=""index-window"" style=""display: flex;"">";
            html += CreateDocLinks();
            html += $@"<div class=""related-post-search"">";
            html += $@"プリント";
            html += $@"</div>"; 
            foreach (var category in categories)
            {                
                if(category == "test")
                {
                    continue;
                }
                html += $@"<a href=""./page/{category}.html"" onclick=""trackLinkClick(this)"">";
                html += $@"<div class=""content-placeholder-search"">";
                html += $@"<img src=""./webp/{category}.webp"" class=""content-image-group-search"">";
                html += $@"<div class=""content-text-search""><div>{category}</div></div>";
                html += $@"</div>";
                html += $@"</a>";
            }
            //html += $@"<p class=""annotion"">※アフィリエイト広告掲載あり（Amazonアソシエイト含む）</p>";
            html += "</div>";
            html += "</div>";*/

        }
        public string CreateDocLinks()
        {
            var html = "";
            var UrlWithTitle = new Dictionary<string, string>
            {
                { "../doc/guide.html", "利用ガイド" },
                { "../doc/license.html", "ライセンス" },
                { "../doc/roadmap.html", "ロードマップ" },
                { "../doc/request.html", "問い合わせ" }
            };
            html += $@"<div class=""related-post-search"">ドキュメント</div>";
            foreach (KeyValuePair<string, string> keyValue in UrlWithTitle)
            {
                html += $@"<div class=""content-area-group-doc"">";
                html += $@"<a href=""{keyValue.Key}"" onclick=""trackLinkClick(this)"" target=""_blank"">";
                html += $@"<div class=""content-placeholder-search-doc"">";
                html += $@"{keyValue.Value}";
                html += $@"</div>";
                html += $@"</a>";
                html += $@"</div>";
            }
            return html;
        }
        public string CreateHeadTag(ItemConfig itemConfig)
        {
            var html = "";
            html += $@"<!DOCTYPE html>";
            html += $@"<html lang=""ja"">";
            html += $@"<head>";
            html += $@"<meta charset=""UTF-8"">";
            html += $@"<title>テンプリ-{itemConfig.Title}</title>";
            html += $@"<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">";
            html += $@"<meta name=""description"" content=""{itemConfig.Description}"">";
            html += $@"<meta name=""theme-color"" content=""#000000"">";
            html += $@"<link rel=""manifest"" href=""/manifest.json"">";
            html += $@"<link rel=""stylesheet"" href=""../css/desktop.css"" media=""only screen and (min-width: 768px)"">";
            html += $@"<link rel=""stylesheet"" href=""../css/mobile.css"" media=""only screen and (max-width: 768px)"">";
            if (itemConfig.IsVertical)
            {
                html += $@"<link rel=""stylesheet"" href=""../css/print-vertical.css"" media=""print"">";
            }
            else
            {
                html += $@"<link rel=""stylesheet"" href=""../css/print.css"" media=""print"">";
            }

            html += $@"<link rel=""icon"" href=""../icon/favicon.svg"" type=""image/x-icon"">";
            html += $@"<style>@import url('https://fonts.googleapis.com/css2?family=Noto+Sans+JP:wght@100..900&display=swap');</style>";
            /*if (itemName.Contains("-"))
            {
                html += $@"<link rel = ""canonical"" href = ""https://tempri.tokyo/{category}.html"" />";
            }*/
            html += CreateOrgDara(itemConfig);
            html += $@"<script type=""text/javascript"">(function (f, b) {{ if (!b.__SV) {{ var e, g, i, h; window.mixpanel = b; b._i = []; b.init = function (e, f, c) {{ function g(a, d) {{ var b = d.split("".""); 2 == b.length && ((a = a[b[0]]), (d = b[1])); a[d] = function () {{ a.push([d].concat(Array.prototype.slice.call(arguments, 0))); }}; }} var a = b; ""undefined"" !== typeof c ? (a = b[c] = []) : (c = ""mixpanel""); a.people = a.people || []; a.toString = function (a) {{ var d = ""mixpanel""; ""mixpanel"" !== c && (d += ""."" + c); a || (d += "" (stub)""); return d; }}; a.people.toString = function () {{ return a.toString(1) + "".people (stub)""; }}; i = ""disable time_event track track_pageview track_links track_forms track_with_groups add_group set_group remove_group register register_once alias unregister identify name_tag set_config reset opt_in_tracking opt_out_tracking has_opted_in_tracking has_opted_out_tracking clear_opt_in_out_tracking start_batch_senders people.set people.set_once people.unset people.increment people.append people.union people.track_charge people.clear_charges people.delete_user people.remove"".split("" ""); for (h = 0; h < i.length; h++) g(a, i[h]); var j = ""set set_once union unset remove delete"".split("" ""); a.get_group = function () {{ function b(c) {{ d[c] = function () {{ call2_args = arguments; call2 = [c].concat(Array.prototype.slice.call(call2_args, 0)); a.push([e, call2]); }}; }} for (var d = {{}}, e = [""get_group""].concat(Array.prototype.slice.call(arguments, 0)), c = 0; c < j.length; c++) b(j[c]); return d; }}; b._i.push([e, f, c]); }}; b.__SV = 1.2; e = f.createElement(""script""); e.type = ""text/javascript""; e.async = !0; e.src = ""undefined"" !== typeof MIXPANEL_CUSTOM_LIB_URL ? MIXPANEL_CUSTOM_LIB_URL : ""file:"" === f.location.protocol && ""//cdn.mxpnl.com/libs/mixpanel-2-latest.min.js"".match(/^\/\//) ? ""https://cdn.mxpnl.com/libs/mixpanel-2-latest.min.js"" : ""//cdn.mxpnl.com/libs/mixpanel-2-latest.min.js""; g = f.getElementsByTagName(""script"")[0]; g.parentNode.insertBefore(e, g); }} }})(document, window.mixpanel || []);</script>";
            html += $@"</head>";
            html += $@"<body>";
            return html;
        }
        public string CreateOrgDara(ItemConfig itemConfig)
        {
            var webpInfo = new FileInfo($@"{GlobalConfig.WebpDir}\{itemConfig.ItemName}.webp");
            var uploadDate = webpInfo.LastWriteTime.ToString("yyyy-MM-dd");
            var html = "";
            html += $@"<script type=""application/ld+json"">";
            html += $@"{{""@context"":""http://schema.org/"",""@type"":""WebPage"",""name"":""{itemConfig.Title}"",""description"":""{itemConfig.Description}"",""url"":""https://tempri.tokyo/page/{itemConfig.ItemKey}.html""}}";
            html += $@"</script>";
            html += $@"<script type=""application/ld+json"">";
            html += $@"{{""@context"":""http://schema.org/"",""@type"":""ImageObject"",""url"":""https://tempri.tokyo/webp/{itemConfig.ItemName}.webp"",""caption"":""{itemConfig.Description}"",""uploadDate"":""{uploadDate}""}}";
            html += $@"</script>";
            html += $@"<script type=""application/ld+json"">";
            html += $@"{{""@context"": ""http://schema.org/"",""@type"": ""CreativeWork"",""name"": ""{itemConfig.ItemName}"",""description"": ""{itemConfig.Description}"",""creator"": {{""@type"": ""Person"",""name"": ""立入""}},""license"": ""ライセンス情報のURL"",""mainEntityOfPage"": {{""@type"": ""WebPage"",""@id"": ""https://tempri.tokyo/page/{itemConfig.ItemKey}.html""}}}}";
            html += $@"</script>";
            html += $@"<script type=""application/ld+json"">";
            html += $@"{{""@context"": ""http://schema.org/"",""@type"": ""Product"",""name"": ""{itemConfig.ItemName}"",""image"": [""https://tempri.tokyo/webp/{itemConfig.ItemName}.webp""],""description"": ""{itemConfig.Description}"",""sku"": ""{itemConfig.ItemName}"",""brand"": {{""@type"": ""Brand"",""name"": ""テンプリ""}},""offers"": {{""@type"": ""Offer"",""priceCurrency"": ""JPY"",""price"": ""0"",""availability"": ""http://schema.org/InStock"",""seller"": {{""@type"": ""Person"",""name"": ""テンプリ""}}}},""mainEntityOfPage"": {{""@type"": ""WebPage"",""@id"": ""https://tempri.tokyo/page/{itemConfig.ItemName}.html""}}}}";
            html += $@"</script>";
            return html;
        }

        public string CreateHeader()
        {
            var html = "";
            html += $@"<div id='header'>";
            html += $@"<a href='https://tempri.tokyo/index.html' onclick=""trackLinkClick(this)"">";
            html += $@"<img id='logo' src=""../icon/logo.svg"" alt=""テンプリ-プリント配布サイト"" />";
            html += $@"</a>";
            html += $@"</div>";
            html += $@"<div id='header-placeholder'></div>";
            return html;
        }
        public string CreateSidebar()
        {
            var GroupCodes = Directory.GetDirectories(GlobalConfig.GroupDir)
                                                    .Select(path => Path.GetFileName(path))
                                                    .Where(groupName => groupName.Contains("_"))
                                                    .Select(groupName => groupName.Split("_").ToList())
                                                    .Select(pair => pair[0]).ToHashSet().ToList();

            var html = "";
            html += $@"<div id='sidebar'>";
            html += $@"<ul id='sidebar-ul'>";
            foreach (string groupCode in GroupCodes)
            {
                var GroupName = GlobalConfig.GroupCodeWithGroupName.TryGetValue(groupCode, out var groupName) ? groupName : "新着";
                html += $@"<button class=""sidebar-button-group"" onclick=""displayContent('category-{GroupName}')"" style=""display:block"">{GroupName}</button>";
                html += $@"<div id=""category-{GroupName}"" class=""sidebar-items"">";
                var CategoryPaths = Directory.GetDirectories(GlobalConfig.GroupDir, $"{groupCode}*");
                var CategorieNames = CategoryPaths.Select(itempath => Path.GetFileNameWithoutExtension(itempath)).ToList();
                foreach (string categoryPath in CategoryPaths)
                {
                    var FolderName = Path.GetFileNameWithoutExtension(categoryPath).Split("_");
                    var CategoryName = FolderName.Length > 2 ? FolderName[2] : FolderName[1];

                    html += $@"<button class=""sidebar-button-category"" onclick=""displayContent('item-{CategoryName}')"" style=""display:block"">- {CategoryName}</button>";
                    html += $@"<div id=""item-{CategoryName}"" class=""sidebar-items"" style=""display:block;"">";
                    var ItemPaths = Directory.GetFiles(categoryPath, "*.csv");
                    foreach (string itemPath in ItemPaths)
                    {
                        html = html.Replace($@"<button class=""sidebar-button-group"" onclick=""displayContent('category-{GroupName}')"" style=""display:block"">", $@"<button class=""sidebar-button-group"" onclick=""displayContent('category-{GroupName}')"" style=""display:inline-flex"">");
                        html = html.Replace($@"<button class=""sidebar-button-category"" onclick=""displayContent('item-{CategoryName}')"" style=""display:block"">", $@"<button class=""sidebar-button-category"" onclick=""displayContent('item-{CategoryName}')"" style=""display:inline-flex"">");
                        var itemName = Path.GetFileNameWithoutExtension(itemPath);
                        html += $@"<a class=""sidebar-link"" onclick=""clickSidebarCategory('{itemName}',this)"">{itemName}</a>";
                    }
                    html += $@"</div>";
                }
                html += $@"</div>";
            }
            html += $@"</ul>";
            html += $@"<div id = 'header-placeholder'></div>";
            html += $@"</div>";
            return html;
        }

        public string CreateMainContent(ItemConfig itemConfig)
        {
            var html = "";
            html += $@"<div id=""main"">";
            html += $@"<div id=""search-result-window""></div>";
            html += $@"<div id='search-form-placeholder'>";
            html += $@"<form id=""search-form"" action=""javascript:void(0);"" class=""search-form"">";
            html += $@"<button class=""search-button"" type=""submit"" aria-label=""検索""></button>";
            html += $@"<input class=""search-form-input"" type=""text"" placeholder=""こちらでプリントを検索できます。"" aria-label=""検索"">";
            html += $@"</form>";
            html += $@"</div>";
            html += $@"<div class=""content-title"">{itemConfig.ItemKey}</div>";

            if (itemConfig.IsGroup)
            {
                var ItemNames = Directory.GetFiles(GlobalConfig.WebpDir, $"{itemConfig.ItemKey}*.webp").Select(item => Path.GetFileNameWithoutExtension(item)).ToList();
                var ItemsListString = ItemNames.Select(item => $"'{item}'").ToList();
                var ItemsString = string.Join(",", ItemsListString);
                if (itemConfig.IsVertical)
                {
                    html += $@"<div class=""content-area-vertical"">";
                    html += $@"<img class=""slide-image-left"" src=""../icon/slide-image-last.svg"" onclick=""updateImageDisplay([{ItemsString}],false,true)"">";
                    for (var i = 0; i < ItemNames.Count(); i++)
                    {
                        html += i == 0 ? $@"<div class=""content-placeholder-multi-vertical carousel-display""  data-index=""{i}"">" : $@"<div class=""content-placeholder-multi-vertical carousel-display desktop-only""  data-index=""{i}"">";
                        html += $@"<img src=""../webp/{ItemNames[i]}.webp"" alt=""{itemConfig.Description}"" class=""content-image-single-vertical"">";
                        html += $@"</div>";
                    }
                    html += $@"<img class=""slide-image-right"" id=""next"" src=""../icon/slide-image-next.svg"" onclick=""updateImageDisplay([{ItemsString}],true,true)"">";
                }
                else
                {
                    html += $@"<div class=""content-area"">";
                    html += $@"<img class=""slide-image-left"" src=""../icon/slide-image-last.svg"" onclick=""updateImageDisplay([{ItemsString}],false,false)"">";
                    html += $@"<div class=""content-placeholder-multi carousel-display"" data-index=""0"">";
                    html += $@"<img src=""../webp/{ItemNames[0]}.webp"" alt=""{itemConfig.Description}"" class=""content-image-single"">";
                    html += $@"</div>";
                    html += $@"<img class=""slide-image-right"" id=""next"" src=""../icon/slide-image-next.svg"" onclick=""updateImageDisplay([{ItemsString}],true,false)"">";
                }
                foreach (var item in ItemNames)
                {
                    if (itemConfig.IsVertical)
                    {
                        html += $@"<img src=""../webp/{item}.webp"" alt=""「紙飛行機の折り紙」のプリントです。印刷・ダウンロードしてお使い下さい。"" class=""content-image-single-vertical print-only"">";
                    }
                    else
                    {
                        html += $@"<img src=""../webp/{item}.webp"" alt=""「紙飛行機の折り紙」のプリントです。印刷・ダウンロードしてお使い下さい。"" class=""content-image-single print-only"">";
                    }
                }
                html += $@"<div id=""carousel-indicators"">";
                html += $@"<span class=""indicator active""></span>";
                for (var i = 0; i < ItemNames.Count() - 1; i++)
                {
                    html += $@"<span class=""indicator""></span>";
                }
                html += $@"</div>";
                html += $@"</div>";
                html += CreateAdsenceArea(itemConfig);

            }
            else
            {
                html += $@"<div class=""content-area"">";
                html += $@"<div class=""content-placeholder-single"">";
                html += $@"<img src=""../webp/{itemConfig.ItemName}.webp"" alt=""{itemConfig.Description}"" class=""content-image-single"">";
                html += $@"</div>";
                html += $@"</div>";
                html += CreateAdsenceArea(itemConfig);
            }
            //html += $@"<div class=""content-description"">{description}</div>";
            return html;

        }
        public string CreateAdsenceArea(ItemConfig itemConfig)
        {
            var html = "";
            html += $@"<div class=""adsense-area"">";
            html += $@"<div class=""adsense-placeholder-large""></div>";
            html += $@"</div>";
            return html;
        }
        public string CreateRelatedPost(ItemConfig itemConfig, ItemsConfig itemsConfig)
        {
            var html = "";
            var tagNumber = 0;
            var xxx = Directory.GetDirectories($@"{GlobalConfig.GroupDir}", $"*{itemConfig.GroupName}");
            var CategoryDirectory = Directory.GetDirectories($@"{GlobalConfig.GroupDir}", $"*{itemConfig.GroupName}", SearchOption.AllDirectories)[0];
            var categoryNames = Directory.GetFiles(CategoryDirectory, "*.csv", SearchOption.AllDirectories).Select(path => Path.GetFileNameWithoutExtension(path)).ToList();
            foreach (var categoryName in categoryNames)
            {
                var item = new Item();
                var relatedItemsConfigList = itemsConfig.itemConfigList.Where(itemConfig => itemConfig.CategoryName == categoryName).ToList();//item.GeRelatedtItemsConfig(categoryName);
                if (relatedItemsConfigList.Count() == 0)
                {
                    continue;
                }
                if (categoryName == "config" || categoryName == "index")
                {
                    continue;
                }
                html += $@"<div class=""related-post"">";
                html += $@"{categoryName}";
                html += $@"</div>";
                html += $@"<div class=""content-area-group"">";

                var i = 0;
                var ItemKeyHashSet = new HashSet<string>();
                foreach (var relatedItemConfig in relatedItemsConfigList)
                {
                    if (ItemKeyHashSet.Contains(relatedItemConfig.ItemKey))
                    {
                        continue;
                    }
                    ItemKeyHashSet.Add(relatedItemConfig.ItemKey);
                    //var itemConfigTemp = item.GetItemConfig(itemPath);
                    html += $@"<a href=""./{relatedItemConfig.ItemKey}.html"">";
                    html += $@"<div class=""content-placeholder-group"">";
                    html += $@"<img src=""../webp-small/{relatedItemConfig.ItemName}.webp"" alt=""{relatedItemConfig.Description}"" class=""content-image-group"" loading=""lazy"">";
                    html += $@"<div class=""content-text-group"">";
                    html += $@"<div class=""content-text-line"">{string.Join("-", relatedItemConfig.ItemKey.Split('-').Skip(1))}</div>";
                    html += $@"</div>";
                    html += $@"</div>";
                    html += $@"<div class=""content-placeholder-group-mobile"">";
                    html += $@"<img src=""../webp-mobile/{relatedItemConfig.ItemName}.webp"" alt=""{relatedItemConfig.Description}"" class=""content-image-group"" loading=""lazy"">";
                    html += $@"<div class=""content-text-group"">";
                    html += $@"<div class=""content-text-line"">{relatedItemConfig.ItemKey}</div>";
                    html += $@"</div>";
                    html += $@"</div>";
                    html += $@"</a>";
                    i++;
                    if (i > 14)
                    {
                        break;
                    }
                }
                html += $@"</div>";
                html += tagNumber == 0 ? CreateRakutenItems(itemConfig) : "";
                tagNumber++;
            }
            //html += $@"<p class=""annotion"">※アフィリエイト広告掲載あり（Amazonアソシエイト含む）</p>";
            html += $@"<div id = 'header-placeholder'></div>";
            return html;
        }
        public string CreateIndexRelatedPost()
        {
            var html = "";
            var tagNumber = 0;
            var json = new Json();
            var itemsConfig = json.DeserializeItemsConfig();
            var CategoryDirectories = Directory.GetDirectories($@"{GlobalConfig.GroupDir}", "", SearchOption.AllDirectories);
            html += $@"<div id=""main"">";
            html += $@"<div id=""search-result-window""></div>";
            html += $@"<div id='search-form-placeholder'>";
            html += $@"<form id=""search-form"" action=""javascript:void(0);"" class=""search-form"">";
            html += $@"<button class=""search-button"" type=""submit"" aria-label=""検索""></button>";
            html += $@"<input class=""search-form-input"" type=""text"" placeholder=""こちらでプリントを検索できます。"" aria-label=""検索"">";
            html += $@"</form>";
            html += $@"</div>";
            foreach (var CategoryDirectory in CategoryDirectories)
            {
                var categoryNames = Directory.GetFiles(CategoryDirectory, "*.csv", SearchOption.AllDirectories).Select(path => Path.GetFileNameWithoutExtension(path)).ToList();
                foreach (var categoryName in categoryNames)
                {
                    var item = new Item();
                    var relatedItemsConfigList = itemsConfig.itemConfigList.Where(itemConfig => itemConfig.CategoryName == categoryName).ToList();//item.GeRelatedtItemsConfig(categoryName);
                    if (relatedItemsConfigList.Count() == 0)
                    {
                        continue;
                    }
                    if (categoryName == "config" || categoryName == "index")
                    {
                        continue;
                    }

                    html += $@"<div class=""related-post"">";
                    html += $@"{categoryName}";
                    html += $@"</div>";
                    html += $@"<div class=""content-area-group"">";
                    var i = 0;
                    var ItemKeyHashSet = new HashSet<string>();
                    foreach (var relatedItemConfig in relatedItemsConfigList)
                    {
                        if (ItemKeyHashSet.Contains(relatedItemConfig.ItemKey))
                        {
                            continue;
                        }
                        ItemKeyHashSet.Add(relatedItemConfig.ItemKey);
                        //var itemConfigTemp = item.GetItemConfig(itemPath);
                        html += $@"<a href=""./page/{relatedItemConfig.ItemKey}.html"">";
                        html += $@"<div class=""content-placeholder-group"">";
                        html += $@"<img src=""../webp-small/{relatedItemConfig.ItemName}.webp"" alt=""{relatedItemConfig.Description}"" class=""content-image-group"" loading=""lazy"">";
                        html += $@"<div class=""content-text-group"">";
                        html += $@"<div class=""content-text-line"">{string.Join("-", relatedItemConfig.ItemKey.Split('-').Skip(1))}</div>";
                        html += $@"</div>";
                        html += $@"</div>";
                        html += $@"<div class=""content-placeholder-group-mobile"">";
                        html += $@"<img src=""../webp-mobile/{relatedItemConfig.ItemName}.webp"" alt=""{relatedItemConfig.Description}"" class=""content-image-group"" loading=""lazy"">";
                        html += $@"<div class=""content-text-group"">";
                        html += $@"<div class=""content-text-line"">{relatedItemConfig.ItemKey}</div>";
                        html += $@"</div>";
                        html += $@"</div>";
                        html += $@"</a>";
                        i++;
                        if (i > 14)
                        {
                            break;
                        }
                    }
                    html += $@"</div>";
                    html += tagNumber == 0 ? CreateRakutenItems(categoryName) : "";
                    tagNumber++;
                }

            }
            html += $@"</div>";
            //html += $@"<p class=""annotion"">※アフィリエイト広告掲載あり（Amazonアソシエイト含む）</p>";
            html += $@"<div id = 'header-placeholder'></div>";
            return html;
        }
        public string CreateRakutenItems(ItemConfig itemConfig)
        {
            var html = "";
            html += $@"<div class=""related-post"">楽天市場</div>";
            html += $@"<div class=""content-area-group"">";
            var category = new Category();
            var categoryConfig = category.GetCategoryConfig(itemConfig.CategoryName);
            Directory.CreateDirectory($@"{GlobalConfig.RakutenDir}\{categoryConfig.GroupName}");
            var itemPaths = Directory.GetFiles($@"{GlobalConfig.RakutenDir}\{categoryConfig.GroupName}", "*.txt").OrderBy(_ => Guid.NewGuid()).ToList();
            if (itemPaths.Count() == 0)
            {
                return "";
            }
            var i = 0;
            foreach (var itemPath in itemPaths)
            {
                html += File.ReadAllText(itemPaths[i]).Replace(
                    $@"<a href=""//af.moshimo.com",
                    $@"<a onclick=""trackRakutenClick(this, '{Path.GetFileNameWithoutExtension(itemPath)}');"" href=""//af.moshimo.com");
                i++; if (i > 12)
                {
                    break;
                }
            }
            html += $@"</div>";
            return html;
        }
        public string CreateRakutenItems(string CategoryName)
        {
            var html = "";
            html += $@"<div class=""related-post"">楽天市場</div>";
            html += $@"<div class=""content-area-group"">";
            var category = new Category();
            var categoryConfig = category.GetCategoryConfig(CategoryName);
            Directory.CreateDirectory($@"{GlobalConfig.RakutenDir}\{categoryConfig.GroupName}");
            var itemPaths = Directory.GetFiles($@"{GlobalConfig.RakutenDir}\{categoryConfig.GroupName}", "*.txt").OrderBy(_ => Guid.NewGuid()).ToList();
            if (itemPaths.Count() == 0)
            {
                return "";
            }
            var i = 0;
            foreach (var itemPath in itemPaths)
            {
                html += File.ReadAllText(itemPaths[i]).Replace(
                    $@"<a href=""//af.moshimo.com",
                    $@"<a onclick=""trackRakutenClick(this, '{Path.GetFileNameWithoutExtension(itemPath)}');"" href=""//af.moshimo.com");
                i++; if (i > 12)
                {
                    break;
                }
            }
            html += $@"</div>";
            return html;
        }
        public string CreateMenu(ItemConfig itemConfig, bool IsIndex)
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
            html += $@"<a class= ""icons-link mobile-only"" href=""https://lin.ee/DkJsNzJ"" target=""_blank"" onclick=""trackLineClick(this)"">";
            html += $@"<img  class= ""focus-icon"" src=""../icon/line-mobile.svg"" alt=""LINE"">";
            html += $@"</a>";
            html += $@"<a class= ""icons-link desktop-only"" href=""https://lin.ee/DkJsNzJ"" target=""_blank""  onclick=""trackLineClick(this)"">";
            html += $@"<img  class= ""focus-icon"" src=""../icon/line-desktop.svg"" alt=""LINE"" >";
            html += $@"</a>";
            html += $@"</div>";
            html += $@"</div>";
            html += $@"</div>";
            return html;
        }
        public string CreateEnd()
        {
            var html = "";
            html += $@"<script src=""../js/keys.js"" defer></script>";
            html += $@"<script src=""../js/_script.js"" defer></script>";
            html += $@"<script src=""../js/tracking.js""></script>";
            html += $@"</body>";
            html += $@"</html>";
            return html;
        }
    }
}
