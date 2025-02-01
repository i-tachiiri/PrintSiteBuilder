using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using Microsoft.VisualBasic.Logging;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Policy;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Mail;
using System.Linq.Expressions;
using PrintSiteBuilder.SiteItem;
using PrintSiteBuilder.Utilities;
using PrintSiteBuilder.Models.General;

namespace PrintSiteBuilder.SiteItem
{
    public class Image
    {
        public void CreateImages(bool IsUpdateAll, ItemsConfig itemsConfig)
        {
            var i = 0;
            foreach (var itemConfig in itemsConfig.itemConfigList)
            {
                i++;
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}][{i}/{itemsConfig.itemConfigList.Count()}]CreateImages : Create {itemConfig.ItemName}.webp ...");
                if (itemConfig.IsInvalidSvg)
                {
                    continue;
                }
                if(!IsUpdateAll)
                {
                    if(!itemConfig.IsWebpUpdated) CreateImage(itemConfig, 3508, GlobalConfig.WebpDir, MagickFormat.WebP);
                    if(!itemConfig.IsWebpSmallUpdated) CreateImage(itemConfig, 640, GlobalConfig.WebpSmallDir, MagickFormat.WebP);
                    if (!itemConfig.IsWebpMobileUpdated) CreateImage(itemConfig, 320, GlobalConfig.WebpMobileDir, MagickFormat.WebP);
                    //if (!itemConfig.IsPngUpdated) CreateImage(itemConfig, 1920, GlobalConfig.PngDir, MagickFormat.Png); //Pngフォルダにwebpができていて機能してない＆使ってないのでコメントアウト
                }
                else
                {
                    CreateImage(itemConfig, 3508, GlobalConfig.WebpDir, MagickFormat.WebP);
                    CreateImage(itemConfig, 640, GlobalConfig.WebpSmallDir, MagickFormat.WebP);
                    CreateImage(itemConfig, 320, GlobalConfig.WebpMobileDir, MagickFormat.WebP);
                    //CreateImage(itemConfig, 1920, GlobalConfig.PngDir, MagickFormat.Png);
                }
            }
        }

        public void CreateImage(ItemConfig itemConfig, int Length, string TargetFolder, MagickFormat magickFormat)
        {
            try
            {
                if (itemConfig.IsInvalidSvg)
                {
                    return;
                }
                
                var Width = itemConfig.IsVertical ? 0 : Length;
                var Height = itemConfig.IsVertical ? Length : 0;
                using (var originalImage = new MagickImage(itemConfig.SvgPath))
                {
                    using (var resizedImage = (MagickImage)originalImage.Clone())
                    {
                        resizedImage.Resize(Width, Height);
                        resizedImage.Quality = 90;
                        string outputPath = $@"{TargetFolder}\{Path.GetFileNameWithoutExtension(itemConfig.ItemName)}.webp";
                        AttachImage(resizedImage, $@"{GlobalConfig.QrDir}\{itemConfig.ItemKey}.png", itemConfig);
                        resizedImage.Write(outputPath, magickFormat);
                    }
                }
            }
            catch { }
        }
        public void CreateDocImage(string docImagePath, int Length, string TargetFolder, MagickFormat magickFormat)
        {
            try
            {
                var Width = Length;
                var Height = 0;
                using (var originalImage = new MagickImage(docImagePath))
                {
                    using (var resizedImage = (MagickImage)originalImage.Clone())
                    {
                        resizedImage.Resize(Width, Height);
                        resizedImage.Quality = 90;
                        string outputPath = $@"{TargetFolder}\{Path.GetFileNameWithoutExtension(docImagePath)}.webp";
                        //AttachImage(resizedImage, $@"{GlobalConfig.QrDir}\{docConfig.ItemKey}.png", docConfig);
                        resizedImage.Write(outputPath, magickFormat);
                    }
                }
            }
            catch (Exception ex)
            {
                ;
            }
        }
        public void CreateDocImages(bool IsUpdateAll, DocsConfig docsConfig, Label ProgressLabel)
        {
            var i = 0;
            foreach (var itemConfig in docsConfig.itemConfigList)
            {
                i++;
                ProgressLabel.Text = $"[{i}/{docsConfig.itemConfigList.Count()}]CreateImages : Create image in {itemConfig.MarkdownName}.md ...";
                ProgressLabel.Update();
                foreach (var docImagePath in itemConfig.DocImagePaths)
                {
                    if (!IsUpdateAll && IsWebpExistsAndNew(docImagePath))
                    {
                        continue;
                    }
                    //CreatePrintImage(itemConfig, GlobalConfig.WebpPrintDir, MagickFormat.WebP);
                    CreateDocImage(docImagePath, 3508, GlobalConfig.WebpDir, MagickFormat.WebP);
                    CreateDocImage(docImagePath, 640, GlobalConfig.WebpSmallDir, MagickFormat.WebP);
                    CreateDocImage(docImagePath, 320, GlobalConfig.WebpMobileDir, MagickFormat.WebP);
                    CreateDocImage(docImagePath, 1920, GlobalConfig.PngDir, MagickFormat.Png);
                }
            }
        }

        public void CreateInstagramImage(bool IsUpdateAll, ItemsConfig itemsConfig)
        {
            var logoFilePath = @"C:\drive\work\www\item\icon\logo_for_instagram.png";
            var i = 0;
            foreach (var itemConfig in itemsConfig.itemConfigList)
            {
                i++;
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}][{i}/{itemsConfig.itemConfigList.Count()}]InstagramImage-single : Create {itemConfig.ItemName}.png ...");
                if (itemConfig.IsInvalidSvg) continue;
                if (!IsUpdateAll && itemConfig.IsInstagramImageUpdated)
                {
                    continue;
                }
                try
                {
                    using (var OriginImage = new MagickImage(itemConfig.SvgPath))
                    {
                        OriginImage.Resize(1026, 726);
                        OriginImage.Crop(OriginImage.Width, OriginImage.Height - 52);
                        OriginImage.Extent(OriginImage.Width, OriginImage.Height + 52, Gravity.Center);

                        OriginImage.BorderColor = new MagickColor("#b2b2b2");
                        OriginImage.Border(2);
                        OriginImage.Extent(1080, 1080, Gravity.Center);

                        AttachText(OriginImage, itemConfig.ItemKey);
                        AttachImage(OriginImage, logoFilePath, itemConfig);
                        OriginImage.Write($@"{GlobalConfig.InstaDir}\{itemConfig.ItemName}.png");
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
        public MagickImage AttachText(MagickImage OriginImage, string AttachText)
        {
            new Drawables()
                    .FontPointSize(36)
                    .Font(@"C:\WINDOWS\FONTS\BIZ-UDGOTHICR.TTC")
                    .FillColor(MagickColors.Black)
                    .TextAlignment(TextAlignment.Center)
                    .Text(OriginImage.Width / 2, 108, AttachText)
                    .Draw(OriginImage);
            return OriginImage;
        }
        public MagickImage AttachImage(MagickImage OriginImage, string QrPath, ItemConfig itemConfig)
        {
            var OneCenti = itemConfig.IsVertical ? OriginImage.Height * 10 / 297 : OriginImage.Height / 21; //画像サイズが異なるので、1cm相当の大きさを計算する
            var PlaceHolder = new MagickImage(MagickColors.White, OriginImage.Width, OneCenti * 2);
            OriginImage.Composite(PlaceHolder, Gravity.South, CompositeOperator.Over);

            using (var logo = new MagickImage($@"{GlobalConfig.IconDir}\logo_for_qr.png"))
            using (var Qr = new MagickImage(QrPath))
            {
                logo.Resize(0, OneCenti);
                Qr.Resize(0, OneCenti * 6 / 5);
                if (itemConfig.IsGroup && OriginImage.FileName.Contains("折り紙"))
                {
                    var FramWidth = logo.Width + OneCenti * 3 / 2;  //右余白1.5cm
                    var FrameHeight = logo.Height + OneCenti / 2;  //下余白0.5cm
                    using (var LogoFrame = new MagickImage(MagickColors.Transparent, FramWidth, FrameHeight))
                    {
                        LogoFrame.Composite(logo, 0, 0, CompositeOperator.Over);
                        OriginImage.Composite(LogoFrame, Gravity.Southeast, CompositeOperator.Over);
                    }
                }
                else
                {
                    var LogoFrameWidth = logo.Width + Qr.Width + OneCenti * 5 / 10;
                    var LogoFrameHeight = Qr.Height + OneCenti * 6 / 10;
                    using (var LogoFrame = new MagickImage(MagickColors.Transparent, LogoFrameWidth, LogoFrameHeight))
                    {
                        LogoFrame.Composite(logo, Gravity.East, CompositeOperator.Over);
                        LogoFrame.Composite(Qr, Gravity.West, CompositeOperator.Over);
                        OriginImage.Composite(LogoFrame, Gravity.South, CompositeOperator.Over);
                    }
                }

            }
            return OriginImage;
        }
        public bool IsDocExistsAndNew(string OriginPath)
        {
            var ItemName = Path.GetFileNameWithoutExtension(OriginPath);
            var TargetPaths = new List<string>()
            {
                $@"{GlobalConfig.WebpDir}\{ItemName}.webp",
                $@"{GlobalConfig.WebpSmallDir}\{ItemName}.webp",
                $@"{GlobalConfig.WebpMobileDir}\{ItemName}.webp",
                $@"{GlobalConfig.PngDir}\{ItemName}.png",
            };
            foreach (var path in TargetPaths)
            {
                if (!File.Exists(path))
                {
                    return false;
                }
            }
            var OriginFileInfo = new FileInfo(OriginPath);
            foreach (var path in TargetPaths)
            {
                var OutputedFileInfo = new FileInfo(path);
                if (OriginFileInfo.LastWriteTime > OutputedFileInfo.LastWriteTime)
                {
                    return false;
                }
            }
            return true;
        }
        public bool IsWebpExistsAndNew(string OriginPath)
        {
            var ItemName = Path.GetFileNameWithoutExtension(OriginPath);
            var TargetPaths = new List<string>()
            {
                $@"{GlobalConfig.WebpDir}\{ItemName}.webp",
                $@"{GlobalConfig.WebpSmallDir}\{ItemName}.webp",
                $@"{GlobalConfig.WebpMobileDir}\{ItemName}.webp",
                $@"{GlobalConfig.PngDir}\{ItemName}.png",
            };
            foreach (var path in TargetPaths)
            {
                if (!File.Exists(path))
                {
                    return false;
                }
            }
            var OriginFileInfo = new FileInfo(OriginPath);
            foreach (var path in TargetPaths)
            {
                var OutputedFileInfo = new FileInfo(path);
                if (OriginFileInfo.LastWriteTime > OutputedFileInfo.LastWriteTime)
                {
                    return false;
                }
            }
            return true;
        }
        public bool IsInstagramImageExistsAndNew(string OriginPath)
        {
            var ItemName = Path.GetFileNameWithoutExtension(OriginPath);
            var TargetPaths = new List<string>()
            {
                $@"{GlobalConfig.InstaDir}\{ItemName}.webp",
            };
            foreach (var path in TargetPaths)
            {
                if (!File.Exists(path))
                {
                    return false;
                }
            }
            var OriginFileInfo = new FileInfo(OriginPath);
            foreach (var path in TargetPaths)
            {
                var OutputedFileInfo = new FileInfo(path);
                if (OriginFileInfo.LastWriteTime > OutputedFileInfo.LastWriteTime)
                {
                    return false;
                }
            }
            return true;
        }
        public bool IsQrLogoPngExistsAndNew(string OriginPath)
        {
            var ItemName = Path.GetFileNameWithoutExtension(OriginPath);
            var TargetPaths = new List<string>()
            {
                $@"{GlobalConfig.QrLogoDir}\{ItemName}.png",
            };
            foreach (var path in TargetPaths)
            {
                if (!File.Exists(path))
                {
                    return false;
                }
            }
            var OriginFileInfo = new FileInfo(OriginPath);
            foreach (var path in TargetPaths)
            {
                var OutputedFileInfo = new FileInfo(path);
                if (OriginFileInfo.LastWriteTime > OutputedFileInfo.LastWriteTime)
                {
                    return false;
                }
            }
            return true;
        }

        public void CreateQr(bool IsUpdateAll, ItemsConfig itemsConfig)
        {
            var i = 0;
            var qr = new Qr();
            foreach (var itemConfig in itemsConfig.itemConfigList)
            {
                i++;
                Console.WriteLine( $"[{DateTime.Now.ToString("HH:mm:ss")}][{i}/{itemsConfig.itemConfigList.Count()}]CreateQr : Create {itemConfig.ItemKey}.png ...");
                if (!IsUpdateAll && itemConfig.IsQrUpdated)
                {
                    continue;
                }
                qr.GenerateQRCode($@"https://tempri.tokyo/page/{itemConfig.ItemKey}.html").Save($@"{GlobalConfig.QrDir}\{itemConfig.ItemKey}.png", ImageFormat.Png); ;
            }
        }
        public void CreateAttachedLogoAndQr(bool UpdateAll, ItemsConfig itemsConfig)
        {
            var i = 0;
            foreach (var itemConfig in itemsConfig.itemConfigList)
            {
                i++;
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}][{i}/{itemsConfig.itemConfigList.Count()}]CreateAttachedQrAndLogo : Create {itemConfig.ItemKey}.png ...");
                if (!UpdateAll && IsQrLogoPngExistsAndNew(itemConfig.QrPath))
                {
                    continue;
                }
                var logoFilePath = @"C:\drive\work\www\item\icon\logo_for_qr.svg"; // ロゴ画像のファイルパス
                using (var qrImage = new MagickImage(itemConfig.QrPath))
                using (var logoImage = new MagickImage(logoFilePath))
                {
                    //qrImage.Resize(0, 300);
                    logoImage.Resize(0, qrImage.Height);
                    int combinedWidth = qrImage.Width + logoImage.Width;
                    int combinedHeight = 300;// Math.Max(qrImage.Height, logoImage.Height);
                    using (var combinedImage = new MagickImage(MagickColors.Transparent, combinedWidth, combinedHeight))
                    {
                        combinedImage.Composite(qrImage, 0, 0, CompositeOperator.Over);
                        combinedImage.Composite(logoImage, qrImage.Width, (combinedHeight - logoImage.Height) / 2, CompositeOperator.Over);
                        combinedImage.Write(@$"{GlobalConfig.QrLogoDir}\{itemConfig.ItemKey}.png");
                    }
                }
            }
        }
        public void CreatePdf(bool IsUpdateAll, ItemsConfig itemsConfig, Label ProgressLabel)
        {
            var i = 0;
            var item = new Item();
            foreach (var itemConfig in itemsConfig.itemConfigList)
            {
                var OutputedPdf = $@"{GlobalConfig.PdfDir}\{itemConfig.ItemKey}.pdf"; // PDFファイルの出力先ディレクトリとファイル名を設定
                i++;
                ProgressLabel.Text = $"[{i}/{itemsConfig.itemConfigList.Count()}]Pdf : Create {itemConfig.ItemKey}.pdf ...";
                ProgressLabel.Update();
                if (!IsUpdateAll && itemConfig.IsPdfUpdated)
                {
                    continue;
                }
                try
                {
                    using (var originalImage = new MagickImage(itemConfig.SvgPath))
                    {
                        string outputPath = $@"{GlobalConfig.PdfDir}\{itemConfig.ItemKey}.pdf";
                        originalImage.Resize(3508, 2480);
                        originalImage.Density = new Density(300, 300, DensityUnit.PixelsPerInch);
                        AttachImage(originalImage, $@"{GlobalConfig.QrDir}\{itemConfig.ItemKey}.png", itemConfig);
                        originalImage.Write(outputPath, MagickFormat.Pdf);
                    }
                }
                catch
                {
                    // エラー処理をここに挿入（ログ記録など）
                    continue;
                }
            }
        }
        public void CreatePdfFromGroup(bool IsUpdateAll, ItemsConfig itemsConfig)
        {
            var i = 0;
            var item = new Item();
            foreach (var key in itemsConfig.Keys)
            {
                i++;
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}][{i}/{itemsConfig.Keys.Count()}]CreatePdfFromGroup : Create {key}.pdf ...");

                var itemConfigs = itemsConfig.itemConfigList.Where(config => config.SvgPath.Contains(key)).ToList();
                if (!IsUpdateAll && itemConfigs.Any(config => config.IsPdfUpdated) ) //keyは全部同じだからAnyでいい
                {
                    continue;
                }
                if (itemConfigs.Any(config => config.IsInvalidSvg))
                {
                    continue;
                }
                using (var images = new MagickImageCollection())
                {
                    foreach (var itemConfig in itemConfigs)
                    {
                        var originImage = new MagickImage(itemConfig.SvgPath);
                        var processedImage = AttachImage(originImage, $@"{GlobalConfig.QrDir}\{itemConfig.ItemKey}.png", itemConfig);
                        images.Add(processedImage);
                    }
                    images.Write($@"{GlobalConfig.PdfDir}\{key}.pdf");
                }


            }
        }
    }
}
