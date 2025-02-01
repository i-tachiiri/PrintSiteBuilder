
using BlogDomain.Config;
using MysqlLibrary.Repository;
namespace BlogGenerator.Services
{
    public class IndexHtmlGenerator
    {
        private HtmlGenerator htmlGenerator;
        private MysqlBlogRepository blogRepository;

        public IndexHtmlGenerator(HtmlGenerator htmlGenerator, MysqlBlogRepository blogRepository) 
        {
            this.htmlGenerator = htmlGenerator;
            this.blogRepository = blogRepository;
        }
        public async Task GenerateIndexHtml()
        {
            var EntitiesHasDirectory = await blogRepository.GetHasDirectoryAsync();
            var posts = String.Empty;
            var PopularPosts = EntitiesHasDirectory.First().PopularPosts.Replace("../", "");
            var LatestPosts = EntitiesHasDirectory.First().LatestPosts.Replace("../", "");
            var DirectoryGroup = EntitiesHasDirectory.GroupBy(e => e.MainCategory).OrderByDescending(e => e.Count());
            foreach (var group in DirectoryGroup)
            {
                var OrderedGroup = group.OrderByDescending(entity => entity.CreatedTime);
                posts += $@"<aside><section class=""popular-articles""><h2 id=""menu"">{group.Key}</h2></section></aside>";
                posts += $@"<nav class=""index-posts"">";
                posts += $@"<ul>";
                foreach (var entity in OrderedGroup)
                {
                    posts += $@"<li><a href=""{entity.Directory}/{entity.PageId}.html"">{entity.Title}</a></li>";
                }
                posts += $@"</ul>";
                posts += $@"</nav>";
            }
            var html = htmlGenerator.CombineIndexAndTemplate(posts, PopularPosts, LatestPosts);
            File.WriteAllText(DomainConstants.Explorer.IndexHtml, html);
        }
    }
}
