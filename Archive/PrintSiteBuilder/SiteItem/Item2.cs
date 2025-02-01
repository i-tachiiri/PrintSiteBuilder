using ImageMagick;
using PrintSiteBuilder.Interfaces;
using PrintSiteBuilder.Models.General;
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
using PrintSiteBuilder.Archive;
using PrintSiteBuilder.GoogleService.Drive;

namespace PrintSiteBuilder.SiteItem
{
    public class Item2
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
        public bool HasAllItems(string itemName,IPrint2 iPrint)
        {
            var svgExists = File.Exists($@"{iPrint.path.PrintSvgDir}\{itemName}.svg");
            var pngExists = File.Exists($@"{iPrint.path.PrintPngDir}\{itemName}.png");
            var pdfExists = File.Exists($@"{iPrint.path.PrintPdfDir}\{itemName}.pdf");
            var webpExists = File.Exists($@"{iPrint.path.PrintWebpDir}\{itemName}.webp");
            var webpSmallExists = File.Exists($@"{iPrint.path.PrintThumbnailDir}\{itemName}.webp");
            return svgExists && pdfExists && webpExists && webpSmallExists;
        }
        public async Task<SlidesConfig> GetSlidesConfig(IPrint2 iPrint)
        {
            var slidesConfig = new SlidesConfig();
            var template = new TemplateFolder(iPrint);
            await template.CopyTemplate();
            var slides = await template.GetPrintSlides();

            var amazonSlide = slides.First(slide => slide.Name == $"{iPrint.PrintId}-amazon");
            var amazonConfig = new SlideConfig();
            slidesConfig.AmazonSlideConfig = new SlideConfig() { SlideId = amazonSlide.Id, SlideName = amazonSlide.Name };

            var coverSlide = slides.First(slide => slide.Name == $"{iPrint.PrintId}-cover");
            var coverConfig = new SlideConfig();
            slidesConfig.CoverSlideConfig = new SlideConfig() { SlideId = coverSlide.Id, SlideName = coverSlide.Name };

            var printSlide = slides.First(slide => slide.Name == $"{iPrint.PrintId}-template");
            var printConfig = new SlideConfig();
            slidesConfig.PrintSlideConfig = new SlideConfig() { SlideId = printSlide.Id, SlideName = printSlide.Name };

            return slidesConfig;

        }

        public ItemsConfig GetItemsConfig(bool IsUpdateAll, IPrint2 iPrint)
        {
            var itemsConfig = new ItemsConfig();
            var itemPaths = Directory.GetFiles(iPrint.path.PrintSlideDir, $"*.svg").ToList();
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
                    itemConfig = GetItemConfig(itemPath,iPrint, oldItemConfigs.itemConfigList.Any(item => item.ItemName == Path.GetFileNameWithoutExtension(itemPath)));
                }

                itemConfigList.Add(itemConfig);
                keysList[itemConfig.ItemKey] = 0; // Setに相当する操作
                foreach (var tag in itemConfig.Tags)
                {
                    tagsList[tag] = 0; // Setに相当する操作
                }
            });

            itemsConfig.itemPaths = itemPaths;
            itemsConfig.itemConfigList = itemConfigList.OrderBy(config => config.ItemName).ToList();
            itemsConfig.Keys = keysList.Keys.ToHashSet();
            itemsConfig.Tags = tagsList.Keys.ToHashSet();

            return itemsConfig;
        }

        public ItemConfig GetItemConfig(string itemPath,IPrint2 iPrint,bool HasUuid)
        {
            var itemConfig = new ItemConfig
            {
                SlidePath = itemPath,
                ItemName = itemPath.Contains(@"C:\") ? Path.GetFileNameWithoutExtension(itemPath) : itemPath,
                CategoryName = Path.GetFileNameWithoutExtension(itemPath).Split("-")[0]
            };
            itemConfig.SvgPath = $@"{iPrint.path.PrintSvgDir}\{itemConfig.ItemName}.svg";

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
            itemConfig.QrPath = Path.Combine(iPrint.path.PrintQrDir, $"{itemConfig.ItemKey}.svg");
            itemConfig.Title = $"「{itemConfig.ItemKey}」のプリント";
            itemConfig.Description = $"{itemConfig.ItemKey}のプリントです。";
            itemConfig.IsGroup = itemPath.Contains("svg-group");
            itemConfig.IsHtmlUpdated = IsHtmlUpdated(itemConfig, iPrint);
            itemConfig.IsInvalidSvg = IsInvalidSvg(itemConfig, iPrint);
            itemConfig.IsVertical = IsSvgVertical(itemConfig, iPrint);
            itemConfig.IsInstagramImageUpdated = IsInstagramImageUpdated(itemConfig, iPrint);
            itemConfig.IsWebpUpdated = IsWebpUpdated(itemConfig, iPrint);
            itemConfig.IsWebpSmallUpdated = IsWebpSmallUpdated(itemConfig, iPrint);
            itemConfig.IsPngUpdated = IsPngUpdated(itemConfig, iPrint);
            itemConfig.IsQrUpdated = IsQrUpdated(itemConfig, iPrint);
            itemConfig.IsPdfUpdated = IsPdfUpdated(itemConfig, iPrint);
            itemConfig.HasMp3 = File.Exists($@"{iPrint.path.PrintMp3Dir}\{itemConfig.ItemKey}.mp4");

            if (itemConfig.CategoryName != "index")
            {
                var parentFolder = GetParentFolder(itemConfig.CategoryName, iPrint);
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
            return tag.Contains("q") || tag.Contains("a") || tag.Contains("表面") || tag.Contains("裏面");
        }

        private string GetParentFolder(string categoryName,IPrint2 iPrint)
        {
            var filePath = Directory.GetFiles(iPrint.path.GroupConfigDir, $"{categoryName}.csv", SearchOption.AllDirectories).FirstOrDefault();
            return filePath != null ? Directory.GetParent(filePath).Name : "新着";
        }

        private string GetGroupName(string parentFolderName)
        {
            var splited = parentFolderName.Split("_");
            return splited.Length < 3 ? "新着" : splited[2];
        }
        public bool IsInstagramImageUpdated(ItemConfig itemConfig, IPrint2 iPrint)
        {
            var NewFilePath = $@"{iPrint.path.PrintInstaDir}\{itemConfig.ItemName}.png";
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
        public bool IsWebpUpdated(ItemConfig itemConfig, IPrint2 iPrint)
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
        public bool IsWebpSmallUpdated(ItemConfig itemConfig, IPrint2 iPrint)
        {
            var NewFilePath = $@"{iPrint.path.PrintThumbnailDir}\{itemConfig.ItemName}.webp";
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
        public bool IsPngUpdated(ItemConfig itemConfig, IPrint2 iPrint)
        {
            var NewFilePath = $@"{iPrint.path.PrintPhpDir}\{itemConfig.ItemName}.png";
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
        private bool IsHtmlUpdated(ItemConfig itemConfig, IPrint2 iPrint)
        {
            var NewFilePath = $@"{iPrint.path.PrintPhpDir}\{itemConfig.ItemKey}.php";
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
        private bool IsQrUpdated(ItemConfig itemConfig, IPrint2 iPrint)
        {
            var QrPath = $@"{iPrint.path.PrintQrDir}\{itemConfig.ItemKey}.png";
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
        private bool IsPdfUpdated(ItemConfig itemConfig, IPrint2 iPrint)
        {
            var PdfPath = $@"{iPrint.path.PrintPdfDir}\{itemConfig.ItemKey}.pdf";
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
        private bool IsInvalidSvg(ItemConfig itemConfig, IPrint2 iPrint)
        {
            var cacheKey = $@"{iPrint.path.PrintSlideDir}\{itemConfig.ItemName}.svg";
            if (svgCache.TryGetValue(cacheKey, out var cachedValue))
            {
                return cachedValue.isInvalid;
            }

            try
            {
                var sampleSvg = $@"{iPrint.path.PrintSlideDir}\{itemConfig.ItemName}.svg"; 
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

        private bool IsSvgVertical(ItemConfig itemConfig, IPrint2 iPrint)
        {
            if (itemConfig.IsInvalidSvg)
            {
                return false;  // InvalidなSVGの時点で使わない
            }

            var cacheKey = $@"{iPrint.path.PrintSlideDir}\{itemConfig.ItemName}.svg";
            if (svgCache.TryGetValue(cacheKey, out var cachedValue))
            {
                return cachedValue.isVertical;
            }

            var sampleSvg = $@"{iPrint.path.PrintSlideDir}\{itemConfig.ItemName}.svg"; 
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

        public void RemoveEmptyItem(IPrint2 iPrint)
        {
            foreach (KeyValuePair<string, long> entry in GlobalConfig.EmptyFileSizes)
            {
                Console.WriteLine($"Remove empty {entry.Key}...");
                var folderName = entry.Key;
                var extention = folderName.Split("-")[0];
                var itemSourceExtention = folderName == "svg" ? "gslides" : "svg";
                var itemSourceFolder = itemSourceExtention == "gslides" ? "slide" : "svg";
                var TargetDirectory = $@"{iPrint.path.PrintDir}\{folderName}";
                var items = Directory.GetFiles(TargetDirectory, $"*.{extention}");
                foreach (string item in items)
                {
                    var fileInfo = new FileInfo(item);
                    var itemName = Path.GetFileNameWithoutExtension(item);
                    if (fileInfo.Length < entry.Value)
                    {
                        File.Delete(item);
                    }
                }

            }
        }
        public void RemoveInvalidItem(ItemsConfig itemsConfig, IPrint2 iPrint)
        {
            foreach(ItemConfig itemConfig in itemsConfig.itemConfigList) 
            {
                if(itemConfig.IsInvalidSvg)
                {
                    File.Delete($@"{iPrint.path.PrintSvgDir}\{itemConfig.ItemName}.svg");
                    File.Delete($@"{iPrint.path.PrintPngDir}\{itemConfig.ItemName}.png");
                    File.Delete($@"{iPrint.path.PrintWebpDir}\{itemConfig.ItemName}.webp");
                    File.Delete($@"{iPrint.path.PrintThumbnailDir}\{itemConfig.ItemName}.webp");
                    File.Delete($@"{iPrint.path.PrintPhpDir}\{itemConfig.ItemName}.html");
                    Console.WriteLine($"Delete invaliid item : {itemConfig.ItemName}");
                }
            }
        }
        public void RemoveTestItem(string ItemName, bool IsCreateImage, bool IsCreatePdf, bool IsCreateHtml, IPrint2 iPrint)
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
