using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Windows.Forms.LinkLabel;
using PrintSiteBuilder.SiteItem;
using PrintSiteBuilder.Utilities;
using PrintSiteBuilder.Interfaces;
using PrintSiteBuilder.Models.General;
using PrintSiteBuilder.Archive;


namespace PrintSiteBuilder.SiteItem
{
    public class Php
    {
        private IPrint2 iPrint;
        private ItemsConfig itemsConfig;
        public Php(ItemsConfig itemsConfig,IPrint2 iPrint)
        {
            this.iPrint = iPrint;
            this.itemsConfig = itemsConfig;
        }
        public void CreatePhps(bool UpdateAll) //SVG検索してHasAllItemsからのCreateHtml呼び出してファイル書き込み
        {
            var i = 0;
            var KeysHashSet = new HashSet<string>();
            foreach (var itemConfig in itemsConfig.itemConfigList)
            {
                i++;
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}][{i}/{itemsConfig.itemConfigList.Count()}]CreatePhps : Create {itemConfig.ItemName}.php ...");

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
                var Content = CreatePhp(itemConfig);
                File.WriteAllLines($@"{iPrint.path.PrintPhpDir}\{itemConfig.ItemKey}.php", new[] { Content });
            }
            //CreateIndexPhp();
        }
        public void CreateIndexPhp()
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


            File.WriteAllLines($@"{iPrint.path.PrintDir}\index.php", new[] { html });
        }
        public string CreateIndexRelatedPost()
        {
            var html = "";
            var tagNumber = 0;
            var json = new Json();
            var itemsConfig = json.DeserializeItemsConfig();
            html += $@"<div id=""main"">";
            html += $@"<div id=""search-result-window""></div>";
            html += $@"<div id='search-form-placeholder'>";
            html += $@"<form id=""search-form"" action=""javascript:void(0);"" class=""search-form"">";
            html += $@"<button class=""search-button"" type=""submit"" aria-label=""検索""></button>";
            html += $@"<input class=""search-form-input"" type=""text"" placeholder=""こちらでプリントを検索できます。"" aria-label=""検索"">";
            html += $@"</form>";
            html += $@"</div>";
            foreach (var categoryName in iPrint.PrintId_CategoryName.Values)
            {
                var item = new Item();
                var relatedItemsConfigList = itemsConfig.itemConfigList.Where(itemConfig => itemConfig.CategoryName == categoryName).ToList();//item.GeRelatedtItemsConfig(categoryName);
                if (relatedItemsConfigList.Count() == 0)
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
                tagNumber++;
            }
            html += $@"</div>";
            html += $@"<div id = 'header-placeholder'></div>";
            return html;
        }

        public string CreatePhp(ItemConfig itemConfig)
        {
            var item = new Item();
            var html = "";
            html += CreateHeaderScript();
            html += CreateHeadTag(itemConfig);
            html += CreateHeader();
            html += CreateSidebar();
            html += CreateMainContent(itemConfig);
            html += CreateRelatedPost(itemConfig, itemsConfig);
            html += CreateMenu(itemConfig, false);
            html += CreateEnd();
            return html;
        }
        public string CreateHeaderScript()
        {
            var html = "";
            html += $@"<?php ";
            html += $@"$pass_txt = '../uuid/1pass.txt';";
            html += $@"$session_id = trim(file_get_contents($pass_txt));";
            html += $@"ini_set('session.gc_maxlifetime', 2592000);";
            html += $@"session_start();";
            html += $@"if (isset($_COOKIE[session_name()])) {{";
            html += $@"setcookie(session_name(), $_COOKIE[session_name()], time() + 2592000, ""/"");";
            html += $@"}}";

            html += $@"ini_set('display_errors', 1);";
            html += $@"ini_set('display_startup_errors', 1);";
            html += $@"error_reporting(E_ALL);";
            html += $@"include '../auth/auth.php';";
            html += $@"$message = '';";
            html += $@"$auth_successful = false;";
            html += $@"if (isset($_GET['uuid'])) {{";
            html += $@"$uuid = $_GET['uuid'];";
            html += $@"$file_path = __DIR__ . '/../uuid/uuid.txt';";
            //html += $@"$index_path = __DIR__ . '/../index.php';";
            //html += $@"$folder_name = basename(dirname($index_path));";
            html += $@"list($auth_successful, $message) = authenticate($uuid, $file_path);";
            html += $@"if ($auth_successful) {{";
            html += $@"if (!isset($_SESSION['uuids'])) {{";
            html += $@"$_SESSION['uuids'] = [];";
            html += $@"}}";
            html += $@"if (!in_array($uuid, $_SESSION['uuids'])) {{";
            html += $@"$_SESSION['uuids'][] = $uuid;";
            html += $@"}}";
            html += $@"$_SESSION['authenticated'] = true;";
            html += $@"$redirect_url = strtok($_SERVER[""REQUEST_URI""], '?');";
            html += $@"header(""Location: $redirect_url"");";
            html += $@"exit;";
            html += $@"}}";
            html += $@"}} else {{";
            html += $@"if (isset($_SESSION['authenticated']) && $_SESSION['authenticated'] === true) {{";
            html += $@"$auth_successful = true;";
            html += $@"}} else {{";
            html += $@"$message = 'IDがありません。';";
            html += $@"debug($message);";
            html += $@"if (isset($folder_name)) {{";
            html += $@"debug($folder_name);";
            html += $@"}} else {{";
            html += $@"debug('Folder name is not set.');";
            html += $@"}}";
            html += $@"}}";
            html += $@"}}";
            html += $@"?>";
            return html;
        }
        public string CreateHeadTag(ItemConfig itemConfig)
        {
            var html = "";
            html += $@"<!DOCTYPE html>";
            html += $@"<html lang=""ja"">";
            html += $@"<head>";
            html += $@"<meta charset=""UTF-8"">";
            html += $@"<title>{iPrint.PrintName}-{iPrint.PrintId}</title>";
            html += $@"<meta property=""og:title"" content=""{iPrint.PrintName}"">";
            html += $@"<meta property=""og:image"" content=""https://tempri.tokyo/print/{iPrint.PrintId}/webp/{itemConfig.ItemKey}-q.webp"">";
            html += $@"<meta property=""og:url"" content=""https://tempri.tokyo/print/php/{itemConfig.ItemKey}.php"">";
            html += $@"<meta property=""og:type"" content=""website"">";
            html += $@"<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">";
            html += $@"<meta name=""description"" content=""{itemConfig.Description}"">";
            html += $@"<meta name=""theme-color"" content=""#000000"">";
            //html += $@"<link rel=""manifest"" href=""/manifest.json"">";
            html += $@"<link rel=""stylesheet"" href=""../css/desktop.css"" media=""only screen and (min-width: 1280px)"">";
            html += $@"<link rel=""stylesheet"" href=""../css/tablet.css"" media=""only screen and (min-width: 768px) and (max-width: 1280px)"">";
            html += $@"<link rel=""stylesheet"" href=""../css/mobile.css"" media=""only screen and (max-width: 768px)"">";
            if (itemConfig.IsVertical)
            {
                html += $@"<link rel=""stylesheet"" href=""../css/print-vertical.css"" media=""print"">";
            }
            else
            {
                html += $@"<link rel=""stylesheet"" href=""../css/print.css"" media=""print"">";
            }

            html += $@"<link rel=""icon"" href=""..//icon/favicon.svg"" type=""image/x-icon"">";
            html += $@"<style>@import url('https://fonts.googleapis.com/css2?family=Noto+Sans+JP:wght@100..900&display=swap');</style>";
            html += $@"<script type=""text/javascript"">(function (f, b) {{ if (!b.__SV) {{ var e, g, i, h; window.mixpanel = b; b._i = []; b.init = function (e, f, c) {{ function g(a, d) {{ var b = d.split("".""); 2 == b.length && ((a = a[b[0]]), (d = b[1])); a[d] = function () {{ a.push([d].concat(Array.prototype.slice.call(arguments, 0))); }}; }} var a = b; ""undefined"" !== typeof c ? (a = b[c] = []) : (c = ""mixpanel""); a.people = a.people || []; a.toString = function (a) {{ var d = ""mixpanel""; ""mixpanel"" !== c && (d += ""."" + c); a || (d += "" (stub)""); return d; }}; a.people.toString = function () {{ return a.toString(1) + "".people (stub)""; }}; i = ""disable time_event track track_pageview track_links track_forms track_with_groups add_group set_group remove_group register register_once alias unregister identify name_tag set_config reset opt_in_tracking opt_out_tracking has_opted_in_tracking has_opted_out_tracking clear_opt_in_out_tracking start_batch_senders people.set people.set_once people.unset people.increment people.append people.union people.track_charge people.clear_charges people.delete_user people.remove"".split("" ""); for (h = 0; h < i.length; h++) g(a, i[h]); var j = ""set set_once union unset remove delete"".split("" ""); a.get_group = function () {{ function b(c) {{ d[c] = function () {{ call2_args = arguments; call2 = [c].concat(Array.prototype.slice.call(call2_args, 0)); a.push([e, call2]); }}; }} for (var d = {{}}, e = [""get_group""].concat(Array.prototype.slice.call(arguments, 0)), c = 0; c < j.length; c++) b(j[c]); return d; }}; b._i.push([e, f, c]); }}; b.__SV = 1.2; e = f.createElement(""script""); e.type = ""text/javascript""; e.async = !0; e.src = ""undefined"" !== typeof MIXPANEL_CUSTOM_LIB_URL ? MIXPANEL_CUSTOM_LIB_URL : ""file:"" === f.location.protocol && ""//cdn.mxpnl.com/libs/mixpanel-2-latest.min.js"".match(/^\/\//) ? ""https://cdn.mxpnl.com/libs/mixpanel-2-latest.min.js"" : ""//cdn.mxpnl.com/libs/mixpanel-2-latest.min.js""; g = f.getElementsByTagName(""script"")[0]; g.parentNode.insertBefore(e, g); }} }})(document, window.mixpanel || []);</script>";
            html += $@"</head>";
            html += $@"<body>";
            return html;
        }
        public string CreateHeader()
        {
            var html = "";
            html += $@"<div id='header'>";
            //html += $@"<a href='https://tempri.tokyo/print/{iPrint.PrintId}/index.php' onclick=""trackLinkClick(this)"">";
            html += $@"<img id='logo' src=""../icon/logo.svg"" alt=""テンプリ"" />";
            html += $@"</a>";
            html += $@"</div>";
            html += $@"<div id='header-placeholder'></div>";
            return html;
        }
        public string CreateSidebar()
        {
            /*var GroupCodes = Directory.GetDirectories(GlobalConfig.GroupDir)
                                                    .Select(path => Path.GetFileName(path))
                                                    .Where(groupName => groupName.Contains("_"))
                                                    .Select(groupName => groupName.Split("_").ToList())
                                                    .Select(pair => pair[0]).ToHashSet().ToList();*/

            var html = "";
            html += $@"<div id='sidebar'>";
            html += $@"<ul id='sidebar-ul'>";
            html += $@"<button class=""sidebar-button-category"" onclick=""displayContent('item-{iPrint.PrintName}')"" style=""display:block"">- {iPrint.PrintName}</button>";
            html += $@"<div id=""item-{iPrint.PrintName}"" class=""sidebar-items"" style=""display:block;"">";
            foreach (var itemConfig in itemsConfig.itemConfigList.OrderBy(x => x.ItemName).GroupBy(x => x.ItemKey))
            {
                html += $@"<a class=""sidebar-link"" href=""{itemConfig.First().ItemKey}.php""onclick=""clickSidebarCategory('{itemConfig.First().ItemKey}',this)"">{itemConfig.First().ItemKey.Split("-")[1].TrimStart('0')}枚目のプリント</a>";
            }
            html += $@"</div>";
            html += $@"</ul>";
            html += $@"<div id = 'header-placeholder'></div>";
            html += $@"</div>";

            /*foreach (string groupCode in GroupCodes)
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
            }*/
            html += $@"</ul>";
            html += $@"<div id = 'header-placeholder'></div>";
            html += $@"</div>";
            return html;
        }
        public string CreateMainContent(ItemConfig itemConfig)
        {
            var html = "";
            html += $@"<div id=""main"">";
            html += "<?php if ($auth_successful): ?>";
            if(itemConfig.HasMp3)
            {
                html += $@"<audio id=""audioPlayer"" controls>";
                html += $@"<source src=""../mp3/{itemConfig.ItemKey}.mp3"" type=""audio/mpeg"">";
                html += $@"Your browser does not support the audio element.";
                html += $@"</audio>";
            }
            html += $@"<div class=""content-title"">{iPrint.PrintName}-{itemConfig.ItemKey.Split("-")[1].TrimStart('0')}枚目のプリント</div>";


                var ItemNames = Directory.GetFiles(iPrint.path.PrintWebpDir, $"{itemConfig.ItemKey}*.webp").Select(item => Path.GetFileNameWithoutExtension(item)).ToList();
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
                html += $@"<?php else: ?>";
                html += $@"<div class=""content-area-doc"">";
                html += $@"<p style=""border:solid 1px #BFC5CA;border-radius:10px;padding:20px;"">";
                html += $@"ログインに失敗しました。<br>";
                html += $@"プリントのQRコードから再アクセスしてみて下さい。<br>";
                html += $@"<br>";
                html += $@"<?php echo $debug_info; ?></p>";
                html += $@"</div>";
                html += $@"<?php endif; ?>";
                html += $@"</div>";
                html += $@"</div>";
            return html;
        }
        public string CreateRelatedPost(ItemConfig itemConfig, ItemsConfig itemsConfig)
        {
            var html = "";
            var tagNumber = 0;
            //var CategoryDirectory = Directory.GetDirectories($@"{GlobalConfig.GroupDir}", $"*{itemConfig.GroupName}", SearchOption.AllDirectories)[0];
            //var categoryNames = Directory.GetFiles(CategoryDirectory, "*.csv", SearchOption.AllDirectories).Select(path => Path.GetFileNameWithoutExtension(path)).ToList();
            

            var categoryName = iPrint.PrintName;
            var item = new Item();
            var relatedItemsConfigList = itemsConfig.itemConfigList.Where(itemConfig => itemConfig.CategoryName == categoryName).ToList();
            /*if (relatedItemsConfigList.Count() == 0)
            {
                continue;
            }*/



            var i = 0;
            var ItemKeyHashSet = new HashSet<string>();
            foreach (var relatedItemConfig in relatedItemsConfigList)
            {
                if (ItemKeyHashSet.Contains(relatedItemConfig.ItemKey))
                {
                    continue;
                }
                html += $@"<div class=""related-post"">";
                html += $@"{categoryName}";
                html += $@"</div>";
                html += $@"<div class=""content-area-group"">";
                ItemKeyHashSet.Add(relatedItemConfig.ItemKey);
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
                /*if (i > 14)
                {
                    break;
                }*/
            }
            html += $@"</div>";
            tagNumber++;
            
            html += $@"<div id = 'header-placeholder'></div>";
            return html;
        }
        public string CreateMenu(ItemConfig itemConfig, bool IsIndex)
        {
            var html = "";
            html += $@"<div id = 'menu-placeholder'>";
            if (!IsIndex)
            {
                html += $@"<div class= ""footer-menu-title"">{iPrint.PrintName}-{itemConfig.ItemKey.Split("-")[1].TrimStart('0')}枚目のプリント</div>";
            }
            html += $@"<div id = ""focus-icons"">";
            html += $@"<img class=""focus-icon mobile-only"" src=""../icon/share.svg"" alt=""共有"" onclick=""sharePage(this)"" />";
            html += $@"<img class=""focus-icon desktop-only"" src=""../icon/share-desktop.svg"" alt=""共有"" onclick=""sharePage(this)"" />";
            if (!IsIndex)
            {
                html += $@"<img class= ""focus-icon desktop-only without-apple"" src = ""../icon/print.svg"" alt = ""プリントを印刷""  onclick=""printAttempt(this);""/>";
                html += $@"<img class= ""focus-icon mobile-only without-apple"" src = ""../icon/print-mobile.svg"" alt = ""プリントを印刷""  onclick=""printAttempt(this);""/>";

                html += $@"<a class= ""icons-link desktop-only only-apple"" href = ""../pdf/{itemConfig.ItemKey}.pdf"" target=""_blank"" onclick=""trackPdfDownload(this)"">";
                html += $@"<img class= ""focus-icon only-apple"" src = ""../icon/pdf.svg"" alt = ""PDFをダウンロード"" />";
                html += $@"</a>";
                html += $@"<a class= ""icons-link mobile-only only-apple"" href = ""../pdf/{itemConfig.ItemKey}.pdf"" target=""_blank"" onclick=""trackPdfDownload(this)"">";
                html += $@"<img class= ""focus-icon only-apple"" src = ""../icon/pdf-mobile.svg"" alt = ""PDFをダウンロード"" />";
                html += $@"</a>";
            }
            html += $@"<div id=""customPopup"" class=""custom-popup"" style=""display: none;""></div>";
            html += $@"<img class= ""focus-icon icon-search"" src = ""../icon/search.svg"" alt = ""プリントを検索""  onclick = ""switchSidebar()"" />";
            /*html += $@"<a class= ""icons-link mobile-only"" href=""https://lin.ee/DkJsNzJ"" target=""_blank"" onclick=""trackLineClick(this)"">";
            html += $@"<img  class= ""focus-icon"" src=""../icon/line-mobile.svg"" alt=""LINE"">";
            html += $@"</a>";
            html += $@"<a class= ""icons-link desktop-only"" href=""https://lin.ee/DkJsNzJ"" target=""_blank""  onclick=""trackLineClick(this)"">";
            html += $@"<img  class= ""focus-icon"" src=""../icon/line-desktop.svg"" alt=""LINE"" >";
            html += $@"</a>";*/
            html += $@"</div>";
            html += $@"</div>";
            html += $@"</div>";
            return html;
        }
        public string CreateEnd()
        {
            var html = "";
            html += $@"<script>const sessionId = '<?php echo $session_id; ?>';</script>";
            html += $@"<script src=""../js/keys.js"" defer></script>";
            html += $@"<script src=""../js/_script.js"" defer></script>";
            html += $@"<script src=""../js/tracking.js""></script>";
            html += $@"</body>";
            html += $@"</html>";
            return html;
        }
    }
}
