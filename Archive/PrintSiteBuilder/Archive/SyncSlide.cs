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
    public class SyncSlide
    {
        public void DeleteSlideWithoutSvg()
        {
            var Slides = Directory.GetFiles(GlobalConfig.SlideDir, "*.gslides");
            foreach (var Slide in Slides)
            {
                var FileName = Path.GetFileNameWithoutExtension(Slide);
                if (!File.Exists($@"{GlobalConfig.SvgDir}\{FileName}.svg"))
                {
                    File.Delete($@"{GlobalConfig.SlideTempDir}\{FileName}.gslides");
                    File.Move(Slide, $@"{GlobalConfig.SlideTempDir}\{FileName}.gslides");
                }
            }
        }
        public DataTable CreateStatusTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Do", typeof(string));
            dataTable.Columns.Add("ItemName", typeof(string));
            dataTable.Columns.Add("slide", typeof(string));
            dataTable.Columns.Add("svg", typeof(string));
            return dataTable;
        }

        public DataTable GetSlideStatusTable()
        {
            var itemName = new Item();
            var StatusTable = CreateStatusTable();
            var HtmlItemNames = itemName.GetItemNames(GlobalConfig.SlideDir, "gslides");
            foreach (var HtmlItemName in HtmlItemNames)
            {
                var newRow = StatusTable.NewRow();
                var slideExists = File.Exists($@"{GlobalConfig.SlideDir}\{HtmlItemName}.gslides");
                var svgExists = File.Exists($@"{GlobalConfig.SvgDir}\{HtmlItemName}.svg");
                var DoWhat = svgExists;

                newRow["ItemName"] = HtmlItemName;
                newRow["slide"] = slideExists ? "〇" : "";
                newRow["svg"] = svgExists ? "〇" : "";
                newRow["Do"] = DoWhat ? "" : "再作成";

                StatusTable.Rows.Add(newRow);
            }
            return StatusTable;
        }
        public void RemoveSlide(DataTable StatusTable)
        {
            //var StatusTable = GetSlideStatusTable();
            var BackupDirectory = $@"{GlobalConfig.SlideBackupDir}\{DateTime.Now.ToString("yyyyMMdd_hhmmss")}";
            if (!Directory.Exists(BackupDirectory))
            {
                Directory.CreateDirectory(BackupDirectory);
            }
            foreach (DataRow row in StatusTable.Rows)
            {
                var SlideItemName = row.Field<string>("ItemName");
                if (row.Field<string>("Do") == "再作成")
                {
                    File.Move($@"{GlobalConfig.SlideDir}\{SlideItemName}.gslides", $@"{GlobalConfig.SlideTempDir}\@{SlideItemName}.gslides");
                }
                if (row.Field<string>("Do") == "削除")
                {
                    File.Move($@"{GlobalConfig.SlideDir}\{SlideItemName}.gslides", $@"{BackupDirectory}\{SlideItemName}.gslides");
                }
            }


        }
    }
}
