using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Google.Apis.Sheets.v4.Data;
using PrintSiteBuilder.Utilities;


namespace PrintSiteBuilder.Models.General
{

    public static class GlobalConfig
    {
        public static string ItemDir = @"C:\drive\work\www\item";
        public static string InstaDir = @"C:\drive\work\www\item\instagram";
        public static string LogDir = @"C:\drive\work\www\item\log";
        public static string HtmlDir = @"C:\drive\work\www\item\page";
        public static string QrDir = @"C:\drive\work\www\item\qr";
        public static string QrLogoDir = @"C:\drive\work\www\item\qr-logo";
        public static string HtmlTestDir = @"C:\drive\work\www\item\testpage";
        public static string SlideDir = @"C:\drive\work\www\item\slide";
        public static string WebpDir = @"C:\drive\work\www\item\webp";
        public static string WebpPrintDir = @"C:\drive\work\www\item\webp-print";
        public static string PngDir = @"C:\drive\work\www\item\png";
        public static string JpegDir = @"C:\drive\work\www\item\jpeg";
        public static string TiffDir = @"C:\drive\work\www\item\tiff";
        public static string PdfDir = @"C:\drive\work\www\item\pdf";
        public static string SvgDir = @"C:\drive\work\www\item\svg";
        public static string SvgSourceDir = @"C:\drive\work\www\item\svg-source";
        public static string SvgGroupDir = @"C:\drive\work\www\item\svg-group";
        public static string SlideTempDir = @"C:\drive\work\www\item\slide-temp";
        public static string WebpSmallDir = @"C:\drive\work\www\item\webp-small";
        public static string WebpMobileDir = @"C:\drive\work\www\item\webp-mobile";
        public static string RakutenDir = @"C:\drive\work\www\item\rakuten";
        public static string GroupDir = @"C:\drive\work\www\item\group";
        public static string IconDir = @"C:\drive\work\www\item\icon";
        public static string CssDir = @"C:\drive\work\www\item\css";
        public static string JsDir = @"C:\drive\work\www\item\js";
        public static string ConfigDir = @"C:\drive\work\www\item\config";
        public static string DocMdDir = @"C:\drive\work\www\item\doc-md";
        public static string DocDir = @"C:\drive\work\www\item\doc";
        public static string DocGroupDir = @"C:\drive\work\www\item\group-doc";
        public static string DocTestDir = @"C:\drive\work\www\item\doc-test";
        public static string DocImageDir = @"C:\drive\work\www\item\doc-image";
        public static string SitemapDir = @"C:\drive\work\www\item\sitemap";
        public static string HtmlBackupDir = @"C:\drive\work\www\item\#backup\page";
        public static string SlideBackupDir = @"C:\drive\work\www\item\#backup\slide";
        public static string WebpBackupDir = @"C:\drive\work\www\item\#backup\webp";
        public static string PngBackupDir = @"C:\drive\work\www\item\#backup\png";
        public static string JpegBackupDir = @"C:\drive\work\www\item\#backup\jpeg";
        public static string TiffBackupDir = @"C:\drive\work\www\item\#backup\tiff";
        public static string PdfBackupDir = @"C:\drive\work\www\item\#backup\pdf";
        public static string SvgBackupDir = @"C:\drive\work\www\item\#backup\svg";
        public static string WebpSmallBackupDir = @"C:\drive\work\www\item\#backup\webp-small";
        public static string WebpMobileBackupDir = @"C:\drive\work\www\item\#backup\webp-mobile";

        public static string SitemapPath = @"C:\drive\work\www\item\sitemap\sitemap.xml";
        public static string SitemapBackupDir = @"C:\drive\work\www\item\#backup\sitemap";

        public static string ItemsConfigPath = @"C:\drive\work\www\item\config\itemsConfig.json";
        public static string DocsConfigPath = @"C:\drive\work\www\item\config\docsConfig.json";
        public static string GroupConfigPath = @"C:\drive\work\www\item\group\config.csv";
        public static string GroupIndexPath = @"C:\drive\work\www\item\group\index.csv";
        public static string KeysPath = @"C:\drive\work\www\item\js\keys.js";
        public static string KeysConfigPath = @"C:\drive\work\www\item\js\keys.json";
        public static string KeysBackupPath = @"C:\drive\work\www\item\#backup\keys";

        public static Dictionary<string, long> EmptyFileSizes = new Dictionary<string, long>()
         {
            { "pdf", 280000 },
            { "jpeg", 12000 },
            { "png", 2000 },
            { "svg", 1000 },
            { "tiff", 0 }, // まだ分からないので最小値に
            { "webp", 2500 },
            { "webp-mobile", 500 }, // モバイル用WebPファイルの想定
            { "webp-small", 1000 } // 小サイズWebPファイルの想定
        };
        public static Dictionary<string, string> GroupCodeWithGroupName = new Dictionary<string, string>()
         {
            { "00", "新着" },
            { "20", "算数" },
            { "10", "就学前" },
            { "30", "国語" },
        };


        public static long EmptyPdfSize = 280000;
        public static long EmptyJpegSize = 12000;
        public static long EmptyPngSize = 2000;
        public static long EmptySvgSize = 1000;
        public static long EmptyTiffSize = 0;  //まだ分からないので最小値に
        public static long EmptyWebpSize = 2500;
        public static long EmptyWebpMobileSize = 500;
        public static long EmptyWebpSmallSize = 1000;

        //public static spreadSheets mathSheet = new spreadSheets("1U3n6exHNvS1cxokRQtWGvY7grn2VlCHMqFYC6Vy4tCg","data");
    }
}
