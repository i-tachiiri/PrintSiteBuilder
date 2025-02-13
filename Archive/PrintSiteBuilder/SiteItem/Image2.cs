using ImageMagick;
using PrintSiteBuilder.Interfaces;
using Svg;
using Svg.Transforms;
using PrintSiteBuilder.Models.General;

namespace PrintSiteBuilder.SiteItem
{
    public class Image2
    {
        ItemsConfig itemsConfig;
        IPrint2 iPrint;
        public Image2(ItemsConfig _itemsConfig, IPrint2 _iPrint)
        {
            itemsConfig = _itemsConfig;
            iPrint = _iPrint;
        }
        public void CreateSvgs()
        {
            foreach (var svg in Directory.GetFiles(iPrint.path.PrintSvgDir, "*.svg"))
            {
                File.Delete(svg);
            }
            var i = 0;
            foreach (var itemConfig in itemsConfig.itemConfigList)
            {
                i++;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}][{i}/{itemsConfig.itemConfigList.Count}] CreateSvgs: Create {itemConfig.ItemName}.svg ...");
                if (itemConfig.IsInvalidSvg)
                {
                    continue;
                }
                string originSvgPath = $@"{iPrint.path.PrintSlideDir}\{itemConfig.ItemName}.svg";
                string logoSvgPath = $@"{iPrint.path.PrintLogoDir}\{itemConfig.ItemKey}.svg";
                string outputPath = $@"{iPrint.path.PrintSvgDir}\{itemConfig.ItemName}.svg";
                AttachLogo(originSvgPath, logoSvgPath, outputPath, itemConfig);
            }
        }


        public void CreateImages(bool IsUpdateAll)
        {
            var i = 0;
            if(IsUpdateAll)
            {
                var imagePaths = new List<List<string>>
                    {
                        Directory.GetFiles(iPrint.path.PrintWebpDir).ToList(),
                        Directory.GetFiles(iPrint.path.PrintThumbnailDir).ToList(),
                        Directory.GetFiles(iPrint.path.PrintPngDir).ToList(),
                    };
                foreach (var imagePath in imagePaths)
                {
                    foreach (var image in imagePath)
                    {
                        File.Delete(image);
                    }
                }

            }
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
                    if(!itemConfig.IsWebpUpdated) CreateImage(itemConfig, 3508, iPrint.path.PrintWebpDir, MagickFormat.WebP);
                    if(!itemConfig.IsWebpSmallUpdated) CreateImage(itemConfig, 640, iPrint.path.PrintThumbnailDir, MagickFormat.WebP);
                    if (!itemConfig.IsPngUpdated) CreateImage(itemConfig, 1920, iPrint.path.PrintPngDir, MagickFormat.Png); 
                }
                else
                {
                    CreateImage(itemConfig, 3508, iPrint.path.PrintWebpDir, MagickFormat.WebP);
                    CreateImage(itemConfig, 640, iPrint.path.PrintThumbnailDir, MagickFormat.WebP);
                    CreateImage(itemConfig, 1920, iPrint.path.PrintPngDir, MagickFormat.Png);
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
                        resizedImage.Quality = 100;
                        resizedImage.Density = new Density(300);
                        resizedImage.Depth = 32;
                        resizedImage.FilterType = FilterType.Lanczos;
                        resizedImage.Resize(Width, Height);
                        string outputPath = $@"{TargetFolder}\{Path.GetFileNameWithoutExtension(itemConfig.ItemName)}.{magickFormat.ToString().ToLower()}";
                        resizedImage.Write(outputPath,magickFormat);
                    }
                }
            }
            catch { }
        }

        private const double A4Width = 210; // A4の幅 (mm)
        private const double A4Height = 297; // A4の高さ (mm)
        private const double Margin = 5;





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
                    CreateDocImage(docImagePath, 3508, iPrint.path.PrintWebpDir, MagickFormat.WebP);
                    CreateDocImage(docImagePath, 640, iPrint.path.PrintThumbnailDir, MagickFormat.WebP);
                    CreateDocImage(docImagePath, 1920, iPrint.path.PrintPngDir, MagickFormat.Png);
                }
            }
        }

       /*public void CreateInstagramImage(bool IsUpdateAll, ItemsConfig itemsConfig)
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
                        OriginImage.Write($@"{iPrint.path.PrintInstaDir}\{itemConfig.ItemName}.png");
                    }
                }
                catch
                {
                    continue;
                }
            }
        }*/
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
        public void AttachLogo(string originSvgPath, string logoSvgPath, string outputPath, ItemConfig itemConfig)
        {
            var originDoc = SvgDocument.Open(originSvgPath);
            var logoDoc = SvgDocument.Open(logoSvgPath);
            var OneCenti = itemConfig.IsVertical ? (float)originDoc.Bounds.Height * 10 / 297 : (float)originDoc.Bounds.Height / 10 /217;  //A4用紙の1cm相当
            logoDoc.Width = logoDoc.Width * (OneCenti * 2 / logoDoc.Height);
            logoDoc.Height = OneCenti * 2;
            var xPosition = new SvgUnit(((float)originDoc.Bounds.Width - logoDoc.Width) / 2); // 中央揃え
            var yPosition = new SvgUnit((float)originDoc.Bounds.Height - logoDoc.Height - (OneCenti * 6 / 10)); // 下部に配置

            var logoGrpuo = new SvgGroup();
            logoGrpuo.Transforms = new SvgTransformCollection();
            logoGrpuo.Transforms.Add(new SvgTranslate(xPosition, yPosition));

            logoGrpuo.Children.Add(logoDoc);
            originDoc.Children.Add(logoGrpuo);
            originDoc.Write(outputPath);
        }






        public MagickImage AttachLogoOnly(MagickImage OriginImage, string QrPath,ItemConfig itemConfig)
        {
            var OneCenti = itemConfig.IsVertical ? OriginImage.Height * 10 / 297 : OriginImage.Height / 21; //画像サイズが異なるので、1cm相当の大きさを計算する
            var PlaceHolder = new MagickImage(MagickColors.White, OriginImage.Width, OneCenti * 2);
            OriginImage.Composite(PlaceHolder, Gravity.South, CompositeOperator.Over);

            using (var logo = new MagickImage(iPrint.path.PrintLogo))
            {
                logo.Resize(0, OneCenti);
                var LogoFrameWidth = logo.Width + OneCenti * 5 / 10;
                var LogoFrameHeight = OneCenti * 6 / 10;
                using (var LogoFrame = new MagickImage(MagickColors.Transparent, LogoFrameWidth, LogoFrameHeight))
                {
                    LogoFrame.Composite(logo, Gravity.East, CompositeOperator.Over);
                    OriginImage.Composite(LogoFrame, Gravity.South, CompositeOperator.Over);
                }
            }
            return OriginImage;
        }
        public bool IsDocExistsAndNew(string OriginPath,IPrint2 iPrint)
        {
            var ItemName = Path.GetFileNameWithoutExtension(OriginPath);
            var TargetPaths = new List<string>()
            {
                $@"{iPrint.path.PrintWebpDir}\{ItemName}.webp",
                $@"{iPrint.path.PrintThumbnailDir}\{ItemName}.webp",
                $@"{iPrint.path.PrintPngDir}\{ItemName}.png",
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
                $@"{iPrint.path.PrintWebpDir}\{ItemName}.webp",
                $@"{iPrint.path.PrintThumbnailDir}\{ItemName}.webp",
                $@"{iPrint.path.PrintPngDir}\{ItemName}.png",
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
                $@"{iPrint.path.PrintInstaDir}\{ItemName}.webp",
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
                $@"{iPrint.path.PrintLogoDir}\{ItemName}.png",
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
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}][{i}/{itemsConfig.itemConfigList.Count()}]CreateQr : Create {itemConfig.ItemKey}.svg ...");
                if (!IsUpdateAll && itemConfig.IsQrUpdated)
                {
                    continue;
                }
                var qrCodeSvg = qr.GenerateQRCodeSvg($@"https://tempri.tokyo/print/{iPrint.PrintId}/php/{itemConfig.ItemKey}.php?uuid={iPrint.Uuid}");
                var outputPath = $@"{iPrint.path.PrintQrDir}\{itemConfig.ItemKey}.svg";
                File.WriteAllText(outputPath, qrCodeSvg);
            }
        }

        public void CreateAttachedLogoAndQr(bool UpdateAll, ItemsConfig itemsConfig)
        {
            var i = 0;
            foreach (var itemConfig in itemsConfig.itemConfigList)
            {
                i++;
                Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}][{i}/{itemsConfig.itemConfigList.Count()}]CreateAttachedQrAndLogo : Create {itemConfig.ItemKey}.svg ...");
                if (!UpdateAll && IsQrLogoPngExistsAndNew(itemConfig.QrPath))
                {
                    continue;
                }
                
                var logoFilePath = @$"{iPrint.path.PrintIconDir}\logo_for_qr.svg"; // ロゴ画像のファイルパス
                using (var qrImage = new MagickImage(itemConfig.QrPath))
                using (var logoImage = new MagickImage(logoFilePath))
                {
                    qrImage.Density = new Density(300, 300); // DPIを設定
                    qrImage.ColorSpace = ColorSpace.RGB; // 色空間をRGBに設定

                    logoImage.Density = new Density(300, 300); // DPIを設定
                    logoImage.ColorSpace = ColorSpace.RGB; // 色空間をRGBに設定

                    logoImage.Resize(qrImage.Width*8/10,0);//(0, qrImage.Height*7/10);
                    int combinedWidth = qrImage.Width;// + logoImage.Width;
                    int combinedHeight = qrImage.Height *105 /100+ logoImage.Height; // Math.Max(qrImage.Height, logoImage.Height);

                    using (var combinedImage = new MagickImage(MagickColors.White, combinedWidth, combinedHeight)) //背景を透明にすると線が入るのでWhiteに
                    {
                        combinedImage.Composite(qrImage, 0, 0, CompositeOperator.Over);
                        combinedImage.Composite(logoImage, (qrImage.Width-logoImage.Width)/2, qrImage.Height *105 /100, CompositeOperator.Over);//(logoImage, qrImage.Width, (combinedHeight - logoImage.Height) / 2, CompositeOperator.Over);

                        combinedImage.Density = new Density(300, 300); // DPIを設定
                        combinedImage.ColorSpace = ColorSpace.RGB; // 色空間をRGBに設定

                        combinedImage.Write($@"{iPrint.path.PrintLogoDir}\{itemConfig.ItemKey}.svg");
                    }
                }
            }
        }


    }
}
