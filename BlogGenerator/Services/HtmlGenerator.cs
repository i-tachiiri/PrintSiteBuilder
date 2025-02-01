using BlogDomain.Config;
using BlogDomain.Entity;
using HtmlAgilityPack;
using MysqlLibrary.Repository;
using MysqlLibrary.Repository.Print;
using System.Text.RegularExpressions;

namespace BlogGenerator.Services
{
    public class HtmlGenerator
    {
        private MysqlBlogRepository blogRepository;
        private MysqlCosineRepository cosineRepository;
        public HtmlGenerator(MysqlBlogRepository blogRepository, MysqlCosineRepository cosineRepository)
        {
            this.blogRepository = blogRepository;
            this.cosineRepository = cosineRepository;
        }
        public async Task GenerateHtml()
        {
            var entities = await blogRepository.GetAllAsync();

            foreach (var entity in entities)
            {
#if DEBUG
                //if (entity.PageId.Replace("-", "") != TestPageId) continue;
                //Directory.CreateDirectory(entity.LocalFolder);
#endif
                if (entity.Status == "公開前" || entity.Status == "公開中")
                {
                    var pageId = entity.PageId;
                    var ExportPath = Path.Combine(entity.LocalFolder, $@"{pageId}.html");
                    File.Delete(ExportPath);
                    File.WriteAllText(ExportPath, entity.HtmlContent);
                }
            }
        }
        public string CombineEntityAndTemplate(BlogEntity entity, string BlogFolder, string BlogUrl)
        {
            var template = File.ReadAllText(DomainConstants.Explorer.TemplateHtml);
            var SquareAdsense = File.ReadAllText(DomainConstants.Explorer.AdsenseSquareHtml);
            var HorizonalAdsense = File.ReadAllText(DomainConstants.Explorer.AdsenseHorizonalHtml);

            if (!File.Exists(Path.Combine(BlogFolder, $"{entity.PageId}.webp")))
            {
                template = template.Replace(@"{cover}", "");
            }
            if (!File.Exists(Path.Combine(BlogFolder, "mp3", $"{entity.PageId}.mp3")))
            {
                template = template.Replace(@"{audio}", "");
            }
            if (!File.Exists(Path.Combine(BlogFolder, "pdf", $"{entity.PageId}.pdf")))
            {
                template = template.Replace(@"{pdf}", "");
            }
            template = template.Replace(@"<div class=""cover""></div>", "");
            return template
                .Replace("{title}", entity.Title)
                .Replace("{description}", entity.Summary)
                .Replace("{imageUrl}", entity.ImageUrl)
                .Replace("{pageUrl}", entity.WebPageUrl)
                .Replace("{last-post-url}", $@"../{entity.LastPostUrl.Replace(BlogUrl, "")}")
                .Replace("{next-post-url}", $@"../{entity.NextPostUrl.Replace(BlogUrl, "")}")
                .Replace("{tableOfContent}", entity.TableOfContent)
                .Replace("{content}", entity.Content)
                .Replace("{headers}", entity.Header)
                .Replace("{homeUrl}", $"{BlogUrl}/{entity.Directory}")
                .Replace("{popular-posts}", entity.PopularPosts)
                .Replace("{related-posts}", entity.RelatedPosts)
                .Replace("{latest-posts}", entity.LatestPosts)
                .Replace("{adsense-square}", SquareAdsense)
                .Replace("{adsense-horizonal}", HorizonalAdsense)
                .Replace("<div class=\"cover\"></div>", "")
                .Replace("{cover}", $@"<img src=""../assets/cover/{entity.PageId}.webp""</img>")
                .Replace("{audio}", $@"<audio controls><source src=""../assets/mp3/{entity.PageId}.mp3"" type=""audio/mpeg""></audio>")
                .Replace("{pdf}", $@"<a class=""button-link"" href=""../assets/pdf/{entity.PageId}.pdf"" download><button class=""nav-button"">PDFをダウンロード</button></a>")
                .Replace(@"<nav id=""table-of-contents""><h2 id=""menu"">まとめ</h2><ul></ul></nav>", ""); //見出しがない時にまとめを削除


        }
        public string CombineIndexAndTemplate(string posts, string PopularPosts,string LatestPosts)
        {
            var template = File.ReadAllText(Path.Combine(DomainConstants.Explorer.TemplateIndexHtml));
            return template
                    .Replace("{title}", "立入ブログ")
                    .Replace("{description}", "教育とITに関する記事を書いています。")
                    //.Replace("{imageUrl}", entity.ImageUrl)
                    .Replace("{pageUrl}", $"{DomainConstants.Web.RootUrl}/blog")
                    .Replace("{tableOfContents}", posts)
                    .Replace("{content}", "教育とITに関する記事を書いています。")
                    .Replace("{headers}", "立入ブログ")
                    .Replace("{homeUrl}", $"{DomainConstants.Web.BlogUrl}")
                    .Replace("{popular-posts}", PopularPosts)
                                .Replace("{latest-posts}", LatestPosts);

        }
        public string GeneratePopularPosts(List<BlogEntity> Entities, List<string> PageIds)
        {
           
            var html = String.Empty;
            foreach (var entity in Entities)
            {
                if (PageIds.Contains(entity.PageId))
                {
                    html += $@"<li><a href=""../{entity.Directory}/{entity.PageId}.html"">{entity.Title}</a></li>";
                }
            }
            return html;
        }
        public async Task<string> GenerateRelatedPosts(BlogEntity entity)
        {
            var html = String.Empty;
            var TargetIds = await cosineRepository.GetRelatedPostIds(entity.PageId);
            foreach (var targetId in TargetIds)
            {
                var targetEntity = await blogRepository.GetByPageIdAsync(targetId);
                html += $@"<li><a href=""../{targetEntity.Directory}/{targetId}.html"">{targetEntity.Title}</a></li>";
            }
            return html;
        }
        public async Task<string> GenerateLatestPosts(List<BlogEntity> Entities, List<string> PageIds)
        {
            var html = String.Empty;
            foreach (var entity in Entities)
            {
                if (PageIds.Contains(entity.PageId))
                {
                    html += $@"<li><a href=""../{entity.Directory}/{entity.PageId}.html"">{entity.Title}</a></li>";
                }
            }
            return html;
        }
        public string GenerateSummary(BlogEntity entity)
        {
            if (!string.IsNullOrEmpty(entity.Summary))
            {
                return entity.Summary;
            }
            var doc = new HtmlDocument();
            doc.LoadHtml(entity.Content);
            var firstP = doc.DocumentNode.SelectSingleNode("//p");
            if (firstP != null)
            {
                string pText = firstP.InnerText;
                string[] sentences = Regex.Split(pText, @"(?<=[。！？])");
                string firstTwoSentences = string.Join("", sentences.Take(2));
                return firstTwoSentences;
            }
            else
            {
                return entity.Summary;
            }
        }
    }
}
