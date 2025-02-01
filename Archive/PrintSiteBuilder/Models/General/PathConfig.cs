using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder.Models.General
{
    public class PathConfig
    {
        public string BaseDir;
        public string OriginDir;
        public string OriginCssDir;
        public string OriginAuthDir;
        public string OriginJsDir;
        public string OriginIconDir;
        public string GroupConfigDir;
        public string PrintDir;
        public string PrintSvgDir;
        public string PrintWebpDir;
        public string PrintThumbnailDir;
        public string PrintPhpDir;
        public string PrintQrDir;
        public string PrintInstaDir;
        public string PrintPngDir;
        public string PrintPdfDir;
        public string PrintGoodsDir;
        public string PrintConfigDir;
        public string PrintConfig;
        public string PrintSlideConfig;
        public string PrintAmazonDir;
        public string PrintLogo;
        public string PrintIconDir;
        public string PrintLogoDir;
        public string PrintSquareLogo;
        public string PrintSlideDir;
        public string PrintSvg4Dir;
        public string PrintMp3Dir;
        public string PrintPdf4Dir;
        public string PrintPdfqDir;
        public string PrintPdfaDir;
        public string PrintCoverDir;
        public string PrintUuidDir;
        public string PrintCssDir;
        public string PrintAuthDir;
        public string PrintJsDir;


        public PathConfig(string PrintId)
        {
            BaseDir = $@"C:\drive\work\www\item\print";
            OriginDir = $@"C:\drive\work\www\item\print\000000";
            OriginCssDir = $@"{OriginDir}\css";
            OriginAuthDir = $@"{OriginDir}\auth";
            OriginJsDir = $@"{OriginDir}\js";
            OriginIconDir = $@"{OriginDir}\icon";
            GroupConfigDir = $@"C:\drive\work\www\item\print\_config\group";
            PrintDir = Directory.CreateDirectory($@"{BaseDir}\{PrintId}").FullName;
            PrintSlideDir = Directory.CreateDirectory($@"{PrintDir}\slide").FullName;
            PrintAmazonDir = Directory.CreateDirectory($@"{PrintDir}\amazon").FullName;
            PrintSvgDir = Directory.CreateDirectory($@"{PrintDir}\svg").FullName;
            PrintWebpDir = Directory.CreateDirectory($@"{PrintDir}\webp").FullName;
            PrintThumbnailDir = Directory.CreateDirectory($@"{PrintDir}\thumb").FullName;
            PrintPhpDir = Directory.CreateDirectory($@"{PrintDir}\php").FullName;
            PrintQrDir = Directory.CreateDirectory($@"{PrintDir}\qr").FullName;
            PrintInstaDir = Directory.CreateDirectory($@"{PrintDir}\insta").FullName;
            PrintPngDir = Directory.CreateDirectory($@"{PrintDir}\png").FullName;
            PrintPdfDir = Directory.CreateDirectory($@"{PrintDir}\pdf").FullName;
            PrintGoodsDir = Directory.CreateDirectory($@"{PrintDir}\goods").FullName;
            PrintConfigDir = Directory.CreateDirectory($@"{PrintDir}\config").FullName;
            PrintLogoDir = Directory.CreateDirectory($@"{PrintDir}\logo").FullName;
            PrintConfig = $@"{PrintConfigDir}\config.json";
            PrintSlideConfig = $@"{PrintConfigDir}\config_slide.json";
            PrintLogo = $@"C:\drive\work\www\item\print\000000\icon\logo.svg";
            PrintSquareLogo = $@"C:\drive\work\www\item\print\000000\icon\logo_square.svg";
            PrintSvg4Dir = Directory.CreateDirectory($@"{PrintDir}\svg-4").FullName;
            PrintMp3Dir = Directory.CreateDirectory($@"{PrintDir}\mp3").FullName;
            PrintPdf4Dir = Directory.CreateDirectory($@"{PrintDir}\pdf-4").FullName;
            PrintPdfqDir = Directory.CreateDirectory($@"{PrintDir}\pdf-q").FullName;
            PrintPdfaDir = Directory.CreateDirectory($@"{PrintDir}\pdf-a").FullName;
            PrintCoverDir = Directory.CreateDirectory($@"{PrintDir}\cover").FullName;
            PrintUuidDir = Directory.CreateDirectory($@"{PrintDir}\uuid").FullName;
            PrintCssDir = Directory.CreateDirectory($@"{PrintDir}\css").FullName;
            PrintAuthDir = Directory.CreateDirectory($@"{PrintDir}\auth").FullName;
            PrintJsDir = Directory.CreateDirectory($@"{PrintDir}\js").FullName;
            PrintIconDir = Directory.CreateDirectory($@"{PrintDir}\icon").FullName;
            CopyDirectoryAndFiles(OriginCssDir, PrintCssDir);
            CopyDirectoryAndFiles(OriginJsDir, PrintJsDir);
            CopyDirectoryAndFiles(OriginIconDir, PrintIconDir);
            CopyDirectoryAndFiles(OriginAuthDir, PrintAuthDir);
        }
        private void CopyDirectoryAndFiles(string OriginDir, string DestDir)
        {
            if (Directory.GetFiles(DestDir).Length > 0) return;
            var files = Directory.GetFiles(OriginDir);
            foreach (var file in files)
            {
                string destFile = Path.Combine(DestDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }
        }
    }
}
