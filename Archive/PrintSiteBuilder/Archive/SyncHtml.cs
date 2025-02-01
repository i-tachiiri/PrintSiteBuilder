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
    public class SyncHtml
    {
        public DataTable CreateStatusTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Do", typeof(string));
            dataTable.Columns.Add("ItemName", typeof(string));
            dataTable.Columns.Add("html", typeof(string));
            dataTable.Columns.Add("slide", typeof(string));
            dataTable.Columns.Add("svg", typeof(string));
            dataTable.Columns.Add("png", typeof(string)); ;
            dataTable.Columns.Add("jpeg", typeof(string));
            dataTable.Columns.Add("pdf", typeof(string));
            dataTable.Columns.Add("tiff", typeof(string));
            dataTable.Columns.Add("webp", typeof(string));
            dataTable.Columns.Add("webp-small", typeof(string));
            dataTable.Columns.Add("webp-mobile", typeof(string));
            return dataTable;
        }
        public DataTable GetHtmlStatusTable()
        {
            var itemName = new Item();
            var StatusTable = CreateStatusTable();
            var HtmlItemNames = itemName.GetItemNames(GlobalConfig.HtmlDir, "html");
            foreach (var HtmlItemName in HtmlItemNames)
            {
                var newRow = StatusTable.NewRow();
                var slideExists = File.Exists($@"{GlobalConfig.SlideDir}\{HtmlItemName}.gslides");
                var svgExists = File.Exists($@"{GlobalConfig.SvgDir}\{HtmlItemName}.svg");
                var pngExists = File.Exists($@"{GlobalConfig.PngDir}\{HtmlItemName}.png");
                var jpegExists = File.Exists($@"{GlobalConfig.JpegDir}\{HtmlItemName}.jpeg");
                var pdfExists = File.Exists($@"{GlobalConfig.PdfDir}\{HtmlItemName}.pdf");
                var tiffExists = File.Exists($@"{GlobalConfig.TiffDir}\{HtmlItemName}.tiff");
                var webpExists = File.Exists($@"{GlobalConfig.WebpDir}\{HtmlItemName}.webp");
                var webpSmallExists = File.Exists($@"{GlobalConfig.WebpSmallDir}\{HtmlItemName}.webp");
                var webpMobileExists = File.Exists($@"{GlobalConfig.WebpMobileDir}\{HtmlItemName}.webp");
                var DoWhat = svgExists && pngExists && jpegExists && pdfExists && tiffExists && webpExists && webpSmallExists && webpMobileExists;

                newRow["ItemName"] = HtmlItemName;
                newRow["html"] = "有";
                newRow["slide"] = slideExists ? "〇" : "";
                newRow["svg"] = svgExists ? "〇" : "";
                newRow["png"] = pngExists ? "〇" : "";
                newRow["jpeg"] = jpegExists ? "〇" : "";
                newRow["pdf"] = pdfExists ? "〇" : "";
                newRow["tiff"] = tiffExists ? "〇" : "";
                newRow["webp"] = webpExists ? "〇" : "";
                newRow["webp-small"] = webpSmallExists ? "〇" : "";
                newRow["webp-mobile"] = webpMobileExists ? "〇" : "";
                newRow["Do"] = DoWhat ? "" : "削除";

                StatusTable.Rows.Add(newRow);
            }
            return StatusTable;
        }
        public void RemoveHtmls(DataTable StatusTable)
        {
            //var StatusTable = GetHtmlStatusTable();
            var BackupDirectory = $@"{GlobalConfig.HtmlBackupDir}\{DateTime.Now.ToString("yyyyMMdd_hhmmss")}";
            if (!Directory.Exists(BackupDirectory))
            {
                Directory.CreateDirectory(BackupDirectory);
            }
            foreach (DataRow row in StatusTable.Rows)
            {
                var HtmlItemName = row.Field<string>("ItemName");
                if (row.Field<string>("Do") == "削除")
                {
                    File.Move($@"{GlobalConfig.HtmlDir}\{HtmlItemName}.html", $@"{BackupDirectory}\{HtmlItemName}.html");
                }
            }
        }
    }
}
