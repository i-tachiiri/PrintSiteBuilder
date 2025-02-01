using ImageMagick;
using PrintSiteBuilder.Utilities;
using System;
using System.Collections.Concurrent;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using PrintSiteBuilder.Interfaces;
using PrintSiteBuilder.Models.General;
using PrintSiteBuilder.Archive;

namespace PrintSiteBuilder.SiteItem
{
    public class Item
    {
        public List<string> GetItemNames(string directory, string extention)
        {
            List<string> FileNames = new List<string>();
            string[] FullPaths = Directory.GetFiles(directory, $"*.{extention}");
            foreach (string path in FullPaths)
            {
                FileNames.Add(Path.GetFileNameWithoutExtension(path));
            }
            return FileNames;
        }
        public void SyncAllItems()
        {
            foreach (KeyValuePair<string, long> entry in GlobalConfig.EmptyFileSizes)
            {
                var folderName = entry.Key;
                var extention = folderName.Split("-")[0];
                var directory = $@"{GlobalConfig.ItemDir}\{folderName}";
                var AllItems = Directory.GetFiles(directory, $"*.{extention}");
                var items = AllItems.Where(path => !HasAllItems(Path.GetFileNameWithoutExtension(path))).ToList();
                var dialog = MessageBox.Show($"{items.Count()}/{AllItems.Length}件の{extention}が削除対象です。削除しますか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dialog == DialogResult.OK)
                {
                    foreach (string path in items)
                    {
                        File.Delete(path);
                    }
                }
            }
        }
        public void SyncAllItemsWithoutDialog()
        {
            foreach (KeyValuePair<string, long> entry in GlobalConfig.EmptyFileSizes)
            {
                var folderName = entry.Key;
                var extention = folderName.Split("-")[0];
                var directory = $@"{GlobalConfig.ItemDir}\{folderName}";
                var AllItems = Directory.GetFiles(directory, $"*.{extention}");
                var items = AllItems.Where(path => !HasAllItems(Path.GetFileNameWithoutExtension(path))).ToList();
                if (AllItems.Length - items.Count() > 100)
                {
                    foreach (string path in items)
                    {
                        File.Delete(path);
                    }
                }
            }
        }
        public bool HasAllItems(string itemName)
        {
            var slideExists = File.Exists($@"{GlobalConfig.SlideDir}\{itemName}.gslides");
            var svgExists = File.Exists($@"{GlobalConfig.SvgDir}\{itemName}.svg");
            var pngExists = File.Exists($@"{GlobalConfig.PngDir}\{itemName}.png");
            var jpegExists = File.Exists($@"{GlobalConfig.JpegDir}\{itemName}.jpeg");
            var pdfExists = File.Exists($@"{GlobalConfig.PdfDir}\{itemName}.pdf");
            var tiffExists = File.Exists($@"{GlobalConfig.TiffDir}\{itemName}.tiff");
            var webpExists = File.Exists($@"{GlobalConfig.WebpDir}\{itemName}.webp");
            var webpSmallExists = File.Exists($@"{GlobalConfig.WebpSmallDir}\{itemName}.webp");
            var webpMobileExists = File.Exists($@"{GlobalConfig.WebpMobileDir}\{itemName}.webp");
            //return slideExists && svgExists && pngExists && jpegExists && pdfExists && tiffExists && webpExists && webpSmallExists && webpMobileExists;
            return svgExists && pdfExists && webpExists && webpSmallExists && webpMobileExists;
        }
        public ItemsConfig GeRelatedtItemsConfig(string categoryName)
        {
            var itemsConfig = new ItemsConfig();
            var SingleItemPaths = Directory.GetFiles(GlobalConfig.SvgDir, $"*{categoryName}*.svg");
            var GroupItemPaths = Directory.GetFiles(GlobalConfig.SvgGroupDir, $"*{categoryName}*.svg");
            var ItemPaths = SingleItemPaths.Concat(GroupItemPaths).OrderBy(_ => _).ToList();
            var ItemConfigList = new List<ItemConfig>();
            var KeysList = new HashSet<string>();
            var TagsList = new HashSet<string>();
            foreach (var ItemPath in ItemPaths)
            {
                var itemConfig = GetItemConfig(ItemPath);
                ItemConfigList.Add(itemConfig);
                KeysList.Add(itemConfig.ItemKey);
                foreach (var tag in itemConfig.Tags)
                {
                    TagsList.Add(tag);
                }
            }
            itemsConfig.itemPaths = ItemPaths;
            //ItemKeyが重複する場合(問題と回答)は重複を削除して1つだけ抽出
            itemsConfig.itemConfigList = ItemConfigList.GroupBy(config => config.ItemKey).Select(g => g.First()).ToList();
            //itemsConfig.singleItemConfigList = ItemConfigList.Where(item => !item.IsGroup).ToList();
            //itemsConfig.groupItemConfigList = ItemConfigList.Where(item => item.IsGroup).ToList();
            itemsConfig.Keys = KeysList;
            itemsConfig.Tags = TagsList;
            return itemsConfig;
        }


    public ItemsConfig GetItemsConfig(bool IsUpdateAll)
        {
            var itemsConfig = new ItemsConfig();
            var singleItemPaths = Directory.GetFiles(GlobalConfig.SvgDir, $"*.svg").ToList();
            var groupItemPaths = Directory.GetFiles(GlobalConfig.SvgGroupDir, $"*.svg").ToList();
            var itemPaths = singleItemPaths.Concat(groupItemPaths).ToList();
            var oldItemConfigs = new Json().DeserializeItemsConfig();
            var itemConfigList = new ConcurrentBag<ItemConfig>(); // 並列処理のためにConcurrentBagを使用
            var keysList = new ConcurrentDictionary<string, byte>(); // ConcurrentDictionaryをSetの代わりに使用
            var tagsList = new ConcurrentDictionary<string, byte>(); // ConcurrentDictionaryをSetの代わりに使用
            var i = 0;

            Parallel.ForEach(itemPaths, (itemPath) =>
            {
                Interlocked.Increment(ref i);
                Console.WriteLine($@"[{DateTime.Now:hh:mm:ss}][{i}/{itemPaths.Count}] GetItemsConfig : Get {Path.GetFileNameWithoutExtension(itemPath)} config...");

                ItemConfig itemConfig;
                if (!IsUpdateAll && oldItemConfigs.itemConfigList.Any(item => item.ItemName == Path.GetFileNameWithoutExtension(itemPath)))
                {
                    itemConfig = oldItemConfigs.itemConfigList.First(item => item.ItemName == Path.GetFileNameWithoutExtension(itemPath));
                }
                else
                {
                    itemConfig = GetItemConfig(itemPath);
                }

                itemConfigList.Add(itemConfig);
                keysList[itemConfig.ItemKey] = 0; // Setに相当する操作
                foreach (var tag in itemConfig.Tags)
                {
                    tagsList[tag] = 0; // Setに相当する操作
                }
            });

            itemsConfig.itemPaths = itemPaths;
            itemsConfig.itemConfigList = itemConfigList.ToList();
            itemsConfig.singleItemConfigList = itemConfigList.Where(item => !item.IsGroup).ToList();
            itemsConfig.groupItemConfigList = itemConfigList.Where(item => item.IsGroup).ToList();
            itemsConfig.Keys = keysList.Keys.ToHashSet();
            itemsConfig.Tags = tagsList.Keys.ToHashSet();

            return itemsConfig;
        }

        public ItemConfig GetItemConfig(string itemPath)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss:fff")}]start");
            var itemConfig = new ItemConfig
            {
                SlidePath = itemPath,
                SvgPath = itemPath,
                ItemName = itemPath.Contains(@"C:\") ? Path.GetFileNameWithoutExtension(itemPath) : itemPath,
                CategoryName = Path.GetFileNameWithoutExtension(itemPath).Split("-")[0]
            };

            var itemTags = itemConfig.ItemName.Split("-");
            var tags = new HashSet<string>();
            var types = new HashSet<string>();
            foreach (var itemTag in itemTags)
            {
                if (!IsNumeric(itemTag) && !IsIgnoredTag(itemTag))
                {
                    tags.Add(itemTag);
                }
                if (!IsIgnoredTag(itemTag))
                {
                    types.Add(itemTag);
                }
            }

            itemConfig.Tags = tags.ToList();
            itemConfig.ItemType = string.Join("-", tags);
            itemConfig.ItemKey = string.Join("-", types);
            itemConfig.QrPath = Path.Combine(GlobalConfig.QrDir, $"{itemConfig.ItemKey}.png");
            itemConfig.Title = $"「{itemConfig.ItemKey}」のプリント";
            itemConfig.Description = $"無料で印刷できる{itemConfig.ItemKey}のプリントです。";
            itemConfig.IsGroup = itemPath.Contains("svg-group");
            itemConfig.IsHtmlUpdated = IsHtmlUpdated(itemConfig);
            itemConfig.IsInvalidSvg = IsInvalidSvg(itemConfig);
            itemConfig.IsVertical = IsSvgVertical(itemConfig);
            itemConfig.IsInstagramImageUpdated = IsInstagramImageUpdated(itemConfig);
            itemConfig.IsWebpUpdated = IsWebpUpdated(itemConfig);
            itemConfig.IsWebpMobileUpdated = IsWebpMobileUpdated(itemConfig);
            itemConfig.IsWebpSmallUpdated = IsWebpSmallUpdated(itemConfig);
            itemConfig.IsPngUpdated = IsPngUpdated(itemConfig);
            itemConfig.IsQrUpdated = IsQrUpdated(itemConfig);
            itemConfig.IsPdfUpdated = IsPdfUpdated(itemConfig);

            if (itemConfig.CategoryName != "index")
            {
                var parentFolder = GetParentFolder(itemConfig.CategoryName);
                itemConfig.GroupName = GetGroupName(parentFolder);
            }

            return itemConfig;
        }

        private bool IsNumeric(string input)
        {
            return int.TryParse(input, out _);
        }

        private bool IsIgnoredTag(string tag)
        {
            return tag.Contains("問題") || tag.Contains("回答") || tag.Contains("表面") || tag.Contains("裏面");
        }

        private string GetParentFolder(string categoryName)
        {
            var filePath = Directory.GetFiles(GlobalConfig.GroupDir, $"{categoryName}.csv", SearchOption.AllDirectories).FirstOrDefault();
            return filePath != null ? Directory.GetParent(filePath).Name : "新着";
        }

        private string GetGroupName(string parentFolderName)
        {
            var splited = parentFolderName.Split("_");
            return splited.Length < 3 ? "新着" : splited[2];
        }
        public bool IsInstagramImageUpdated(ItemConfig itemConfig)
        {
            var NewFilePath = $@"{GlobalConfig.InstaDir}\{itemConfig.ItemName}.png";
            var SvgFilePath = itemConfig.SvgPath;
            if (!File.Exists(NewFilePath))
            {
                return false;
            }
            var SvgFileInfo = new FileInfo(SvgFilePath);
            var NewFileInfo = new FileInfo(NewFilePath);
            if (SvgFileInfo.LastWriteTime > NewFileInfo.LastWriteTime)
            {
                return false;
            }
            return true;
        }
        public bool IsWebpUpdated(ItemConfig itemConfig)
        {
            var NewFilePath = $@"{GlobalConfig.WebpDir}\{itemConfig.ItemName}.webp";
            var SvgFilePath = itemConfig.SvgPath;
            if (!File.Exists(NewFilePath))
            {
                return false;
            }
            var SvgFileInfo = new FileInfo(SvgFilePath);
            var NewFileInfo = new FileInfo(NewFilePath);
            if (SvgFileInfo.LastWriteTime > NewFileInfo.LastWriteTime)
            {
                return false;
            }
            return true;
        }
        public bool IsWebpMobileUpdated(ItemConfig itemConfig)
        {
            var NewFilePath = $@"{GlobalConfig.WebpMobileDir}\{itemConfig.ItemName}.webp";
            var SvgFilePath = itemConfig.SvgPath;
            if (!File.Exists(NewFilePath))
            {
                return false;
            }
            var SvgFileInfo = new FileInfo(SvgFilePath);
            var NewFileInfo = new FileInfo(NewFilePath);
            if (SvgFileInfo.LastWriteTime > NewFileInfo.LastWriteTime)
            {
                return false;
            }
            return true;
        }
        public bool IsWebpSmallUpdated(ItemConfig itemConfig)
        {
            var NewFilePath = $@"{GlobalConfig.WebpSmallDir}\{itemConfig.ItemName}.webp";
            var SvgFilePath = itemConfig.SvgPath;
            if (!File.Exists(NewFilePath))
            {
                return false;
            }
            var SvgFileInfo = new FileInfo(SvgFilePath);
            var NewFileInfo = new FileInfo(NewFilePath);
            if (SvgFileInfo.LastWriteTime > NewFileInfo.LastWriteTime)
            {
                return false;
            }
            return true;
        }
        public bool IsPngUpdated(ItemConfig itemConfig)
        {
            var NewFilePath = $@"{GlobalConfig.PngDir}\{itemConfig.ItemName}.png";
            var SvgFilePath = itemConfig.SvgPath;
            if (!File.Exists(NewFilePath))
            {
                return false;
            }
            var SvgFileInfo = new FileInfo(SvgFilePath);
            var NewFileInfo = new FileInfo(NewFilePath);
            if (SvgFileInfo.LastWriteTime > NewFileInfo.LastWriteTime)
            {
                return false;
            }
            return true;
        }
        private bool IsHtmlUpdated(ItemConfig itemConfig)
        {
            var ReleaseHtml = $@"{GlobalConfig.HtmlDir}\{itemConfig.ItemKey}.html"; // PDFファイルの出力先ディレクトリとファイル名を設定
            var DebugHtml = $@"{GlobalConfig.HtmlTestDir}\{itemConfig.ItemKey}.html";
            if (File.Exists(ReleaseHtml) && File.Exists(DebugHtml))
            {
                var SvgFileInfo = new FileInfo(itemConfig.SvgPath);
                var DebugHtmlFileInfo = new FileInfo(DebugHtml);
                var ReleaseHtmlFileInfo = new FileInfo(ReleaseHtml);
                if (SvgFileInfo.LastWriteTime < ReleaseHtmlFileInfo.LastWriteTime && SvgFileInfo.LastWriteTime < DebugHtmlFileInfo.LastWriteTime)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsQrUpdated(ItemConfig itemConfig)
        {
            var QrPath = $@"{GlobalConfig.QrDir}\{itemConfig.ItemKey}.png";
            if (File.Exists(QrPath))
            {
                var SvgFileInfo = new FileInfo(itemConfig.SvgPath);
                var QrFileInfo = new FileInfo(QrPath);
                if (SvgFileInfo.LastWriteTime < QrFileInfo.LastWriteTime)
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsPdfUpdated(ItemConfig itemConfig)
        {
            var PdfPath = $@"{GlobalConfig.PdfDir}\{itemConfig.ItemKey}.pdf";
            if (File.Exists(PdfPath))
            {
                var SvgFileInfo = new FileInfo(itemConfig.SvgPath);
                var PdfFileInfo = new FileInfo(PdfPath);
                if (SvgFileInfo.LastWriteTime < PdfFileInfo.LastWriteTime)
                {
                    return true;
                }
            }
            return false;
        }
        //SVG判定、ChatGPTにキャッシュ利用で書いてもらった
        private readonly Dictionary<string, (bool isInvalid, bool isVertical)> svgCache = new Dictionary<string, (bool, bool)>();
        private bool IsInvalidSvg(ItemConfig itemConfig)
        {
            var cacheKey = itemConfig.IsGroup
                ? $"{GlobalConfig.SvgGroupDir}\\{itemConfig.ItemName}*.svg"
                : $"{GlobalConfig.SvgDir}\\{itemConfig.ItemName}.svg";

            if (svgCache.TryGetValue(cacheKey, out var cachedValue))
            {
                return cachedValue.isInvalid;
            }

            try
            {
                var sampleSvg = itemConfig.IsGroup
                    ? Directory.GetFiles($@"{GlobalConfig.SvgGroupDir}", $"{itemConfig.ItemName}*.svg").FirstOrDefault()
                    : $@"{GlobalConfig.SvgDir}\{itemConfig.ItemName}.svg";

                if (sampleSvg == null)
                {
                    svgCache[cacheKey] = (true, false);
                    return true;
                }

                using (var image = new MagickImage(sampleSvg))
                {
                    svgCache[cacheKey] = (false, image.Height > image.Width);
                    return false;
                }
            }
            catch
            {
                svgCache[cacheKey] = (true, false);
                return true;
            }
        }

        private bool IsSvgVertical(ItemConfig itemConfig)
        {
            if (itemConfig.IsInvalidSvg)
            {
                return false;  // InvalidなSVGの時点で使わない
            }

            var cacheKey = itemConfig.IsGroup
                ? $"{GlobalConfig.SvgGroupDir}\\{itemConfig.ItemName}*.svg"
                : $"{GlobalConfig.SvgDir}\\{itemConfig.ItemName}.svg";

            if (svgCache.TryGetValue(cacheKey, out var cachedValue))
            {
                return cachedValue.isVertical;
            }

            var sampleSvg = itemConfig.IsGroup
                ? Directory.GetFiles($@"{GlobalConfig.SvgGroupDir}", $"{itemConfig.ItemName}*.svg").FirstOrDefault()
                : $@"{GlobalConfig.SvgDir}\{itemConfig.ItemName}.svg";

            if (sampleSvg == null)
            {
                svgCache[cacheKey] = (true, false);
                return false;
            }

            using (var image = new MagickImage(sampleSvg))
            {
                var isVertical = image.Height > image.Width;
                svgCache[cacheKey] = (false, isVertical);
                return isVertical;
            }
        }




        public void RemoveEmptySvg()
        {
            var svgs = Directory.GetFiles(GlobalConfig.SvgDir, "*.svg");
            foreach (string svg in svgs)
            {
                var fileInfo = new FileInfo(svg);
                var itemName = Path.GetFileNameWithoutExtension(svg);
                var gslidePath = $@"{GlobalConfig.SlideDir}\{itemName}.gslides";
                if (fileInfo.Length < GlobalConfig.EmptySvgSize)
                {
                    File.Delete(svg);
                }
                if (File.Exists(gslidePath))
                {
                    File.Move(gslidePath, $@"{GlobalConfig.SlideTempDir}\@{itemName}.gslides");
                }
            }
        }
        public void RemoveEmptyItem()
        {
            foreach (KeyValuePair<string, long> entry in GlobalConfig.EmptyFileSizes)
            {
                Console.WriteLine($"Remove empty {entry.Key}...");
                var folderName = entry.Key;
                var extention = folderName.Split("-")[0];
                var itemSourceExtention = folderName == "svg" ? "gslides" : "svg";
                var itemSourceFolder = itemSourceExtention == "gslides" ? "slide" : "svg";
                var preExportFolder = itemSourceFolder == "slide" ? "slide-temp" : "svg-source";
                var TargetDirectory = $@"{GlobalConfig.ItemDir}\{folderName}";
                var items = Directory.GetFiles(TargetDirectory, $"*.{extention}");
                foreach (string item in items)
                {
                    var fileInfo = new FileInfo(item);
                    var itemName = Path.GetFileNameWithoutExtension(item);
                    if (fileInfo.Length < entry.Value)
                    {
                        File.Delete(item);
                        var SourceDir = $@"{GlobalConfig.ItemDir}\{itemSourceFolder}\{itemName}\{itemSourceExtention}";
                        var DestDir = $@"{GlobalConfig.ItemDir}\{preExportFolder}\@{itemName}\{itemSourceExtention}";
                        if (File.Exists(SourceDir))
                        {
                            File.Move(SourceDir, DestDir);
                        }
                    }
                }

            }
        }
        public void RemoveInvalidItem(ItemsConfig itemsConfig)
        {
            foreach(ItemConfig itemConfig in itemsConfig.itemConfigList) 
            {
                if(itemConfig.IsInvalidSvg)
                {
                    File.Delete($@"{GlobalConfig.SvgDir}\{itemConfig.ItemName}.svg");
                    File.Delete($@"{GlobalConfig.SvgGroupDir}\{itemConfig.ItemName}.svg");
                    File.Delete($@"{GlobalConfig.PngDir}\{itemConfig.ItemName}.png");
                    File.Delete($@"{GlobalConfig.WebpDir}\{itemConfig.ItemName}.webp");
                    File.Delete($@"{GlobalConfig.WebpMobileDir}\{itemConfig.ItemName}.webp");
                    File.Delete($@"{GlobalConfig.WebpSmallDir}\{itemConfig.ItemName}.webp");
                    File.Delete($@"{GlobalConfig.HtmlDir}\{itemConfig.ItemName}.html");
                    File.Delete($@"{GlobalConfig.HtmlTestDir}\{itemConfig.ItemName}.html");
                    Console.WriteLine($"Delete invaliid item : {itemConfig.ItemName}");
                }
            }
        }
        public void RemoveTestItem(string ItemName, bool IsCreateImage, bool IsCreatePdf, bool IsCreateHtml)
        {
            if (string.IsNullOrEmpty(ItemName)) { return; }
            var items = Directory.GetFiles(GlobalConfig.ItemDir, $"{ItemName}*", SearchOption.AllDirectories);
            foreach (string item in items)
            {
                string extension = Path.GetExtension(item).ToLower(); // 小文字に変換して大文字小文字を区別しないようにします
                if (extension == ".svg" && extension == ".csv" && extension == ".gslides")
                {
                    continue;//
                }
                else if (extension == ".png" && IsCreateImage)
                {
                    File.Delete(item);
                }
                else if (extension == ".webp" && IsCreateImage)
                {
                    File.Delete(item);
                }
                else if (extension == ".pdf" && IsCreatePdf)
                {
                    File.Delete(item);
                }
                else if (extension == ".html" && IsCreateHtml)
                {
                    File.Delete(item);
                }
                else { }
            }
        }

    }
}
