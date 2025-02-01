using BlogDomain.Config;
using MysqlLibrary.Repository;

namespace BlogGenerator.Services
{
    public class SitemapGenerator
    {
        private MysqlBlogRepository blogRepository;
        public SitemapGenerator(MysqlBlogRepository blogRepository)
        {
            this.blogRepository = blogRepository;
        }
        public async Task GenerateSitemapText()
        {
            var Entities = await blogRepository.GetPublishedAsync();
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?><urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"" xmlns:image=""http://www.google.com/schemas/sitemap-image/1.1"">";
            foreach (var entity in Entities)
            {
                xml += "<url>";
                xml += $"<loc>{entity.WebPageUrl}</loc>";
                xml += $"<lastmod>{entity.CreatedTime.ToString("yyyy-MM-dd")}</lastmod>";
                xml += "<changefreq>weekly</changefreq>";
                //xml += $"<priority>{priority}</priority>";
                /*xml += "<image:image>";
                xml += $"<image:loc>{entity.ImageUrl}</image:loc>";
                xml += $"<image:caption>{entity.Summary}</image:caption>";
                xml += $"<image:title>{entity.Title}</image:title>";
                xml += "</image:image>";*/
                xml += "</url>";
            }
            xml += "</urlset>";
            xml = xml.Replace("&", "・");
            File.WriteAllText(DomainConstants.Explorer.SitemapPath, xml);
        }
    }
}
