using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.Logging;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.ComponentModel;
using System.Security.Policy;
using System.Windows.Forms.Design;
using PrintSiteBuilder.Models.General;

namespace PrintSiteBuilder.SiteItem
{
    public class Sitemap
    {
        public string SitemapPath = @"C:\drive\work\www\item\sitemap\sitemap.xml";
        public string SitemapBackupDir = @"C:\drive\work\www\item\#backup\sitemap";

        public string CreateSitemapText(ItemsConfig itemsConfig, DocsConfig docsConfig)
        {
            var itemName = new Item();
            //var FileNames = itemName.GetItemNames(GlobalConfig.HtmlDir,"html");
            var Configs = itemsConfig.itemConfigList; //.Concat(docsConfig.itemConfigList);
            var ItemKeyHaseSet = new HashSet<string>();
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?><urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"" xmlns:image=""http://www.google.com/schemas/sitemap-image/1.1"">";
            foreach (var config in Configs)
            {
                if (ItemKeyHaseSet.Contains(config.ItemKey))
                {
                    continue;
                }
                ItemKeyHaseSet.Add(config.ItemKey);
                //var priority = config.ItemKey.Contains("-") ? "0.1" : config.ItemKey.Contains("(") ? "0.5" : "1";
                var lastUpdate = System.IO.File.GetLastWriteTime($@"{GlobalConfig.HtmlDir}\{config.ItemKey}.html").ToString("yyyy-MM-dd");
                var WebpPath = System.IO.File.Exists($@"{GlobalConfig.WebpDir}\{config.ItemKey}.webp") ? $"https://tempri.tokyo/webp/${config.ItemKey}.webp" : $"https://tempri.tokyo/icon/doc-image.png";
                xml += "<url>";
                xml += $"<loc>https://tempri.tokyo/{config.ItemKey}.html</loc>";
                xml += $"<lastmod>{lastUpdate}</lastmod>";
                xml += "<changefreq>weekly</changefreq>";
                //xml += $"<priority>{priority}</priority>";
                xml += "<image:image>";
                xml += $"<image:loc>{WebpPath}</image:loc>";
                //xml += $"<image:caption>{description}</image:caption>";
                xml += $"<image:title>{config.Title}</image:title>";
                xml += $"<image:license>https://tempri.tokyo/doc/license.html</image:license>";
                xml += "</image:image>";
                xml += "</url>";
            }
            xml += "</urlset>";
            xml = xml.Replace("&", "・");
            return xml;
        }

        public void BackupSitemap()
        {
            var BackupFileName = DateTime.Now.ToString("yyyyMMddhhmmss");
            System.IO.File.Copy(SitemapPath, $@"{SitemapBackupDir}\{BackupFileName}.xml");
        }
        public void UpdateSitemap(ItemsConfig itemsConfig, DocsConfig docsConfig)
        {
            var SitemapText = CreateSitemapText(itemsConfig, docsConfig);
            System.IO.File.WriteAllText(SitemapPath, SitemapText);
        }
    }
}
