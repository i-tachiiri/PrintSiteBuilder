using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder.Models.General;
using PrintSiteBuilder.SiteItem;

namespace PrintSiteBuilder.Archive
{
    public class SyncSvg
    {
        public DataTable CreateStatusTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Do", typeof(string));
            dataTable.Columns.Add("ItemName", typeof(string));
            dataTable.Columns.Add("svg", typeof(string));
            dataTable.Columns.Add("slide", typeof(string));
            dataTable.Columns.Add("png", typeof(string)); ;
            dataTable.Columns.Add("jpeg", typeof(string));
            dataTable.Columns.Add("pdf", typeof(string));
            dataTable.Columns.Add("tiff", typeof(string));
            dataTable.Columns.Add("webp", typeof(string));
            dataTable.Columns.Add("webp-small", typeof(string));
            dataTable.Columns.Add("webp-mobile", typeof(string));
            return dataTable;
        }
        public DataTable GetSvgStatusTable()
        {
            var itemName = new Item();
            var StatusTable = CreateStatusTable();
            var SvgItemNames = itemName.GetItemNames(GlobalConfig.SvgDir, "svg");
            foreach (var SvgItemName in SvgItemNames)
            {
                var newRow = StatusTable.NewRow();
                var slideExists = File.Exists($@"{GlobalConfig.SlideDir}\{SvgItemName}.gslides");
                var svgExists = File.Exists($@"{GlobalConfig.SvgDir}\{SvgItemName}.svg");
                var pngExists = File.Exists($@"{GlobalConfig.PngDir}\{SvgItemName}.png");
                var jpegExists = File.Exists($@"{GlobalConfig.JpegDir}\{SvgItemName}.jpeg");
                var pdfExists = File.Exists($@"{GlobalConfig.PdfDir}\{SvgItemName}.pdf");
                var tiffExists = File.Exists($@"{GlobalConfig.TiffDir}\{SvgItemName}.tiff");
                var webpExists = File.Exists($@"{GlobalConfig.WebpDir}\{SvgItemName}.webp");
                var webpSmallExists = File.Exists($@"{GlobalConfig.WebpSmallDir}\{SvgItemName}.webp");
                var webpMobileExists = File.Exists($@"{GlobalConfig.WebpMobileDir}\{SvgItemName}.webp");
                var hasAllItems = pngExists && jpegExists && pdfExists && tiffExists && webpExists && webpSmallExists && webpMobileExists;

                newRow["ItemName"] = SvgItemName;
                newRow["slide"] = slideExists ? "〇" : "";
                newRow["svg"] = svgExists ? "〇" : "";
                newRow["png"] = pngExists ? "〇" : "";
                newRow["jpeg"] = jpegExists ? "〇" : "";
                newRow["pdf"] = pdfExists ? "〇" : "";
                newRow["tiff"] = tiffExists ? "〇" : "";
                newRow["webp"] = webpExists ? "〇" : "";
                newRow["webp-small"] = webpSmallExists ? "〇" : "";
                newRow["webp-mobile"] = webpMobileExists ? "〇" : "";
                newRow["Do"] = hasAllItems ? "" : slideExists ? "再作成" : "削除";

                StatusTable.Rows.Add(newRow);
            }
            return StatusTable;
        }
        public void RemoveSvg(DataTable StatusTable)
        {
            //var StatusTable = GetHtmlStatusTable();
            var BackupDirectory = $@"{GlobalConfig.SvgBackupDir}\{DateTime.Now.ToString("yyyyMMdd_hhmmss")}";
            if (!Directory.Exists(BackupDirectory))
            {
                Directory.CreateDirectory(BackupDirectory);
            }
            foreach (DataRow row in StatusTable.Rows)
            {
                var SvgItemName = row.Field<string>("ItemName");
                if (row.Field<string>("Do") == "削除")
                {
                    File.Move($@"{GlobalConfig.SvgDir}\{SvgItemName}.svg", $@"{BackupDirectory}\{SvgItemName}.svg");
                }
                if (row.Field<string>("Do") == "再作成")
                {
                    File.Delete($@"{GlobalConfig.SvgSourceDir}\{SvgItemName}.svg");
                    File.Move($@"{GlobalConfig.SvgDir}\{SvgItemName}.svg", $@"{GlobalConfig.SvgSourceDir}\{SvgItemName}.svg");
                }
            }
        }
    }
}
