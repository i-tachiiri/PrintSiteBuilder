using BlogDomain.Config;
using MysqlLibrary.Repository;

namespace BlogGenerator.Services
{
    public class HtaccessGenerator
    {
        private MysqlBlogRepository blogRepository;
        public HtaccessGenerator(MysqlBlogRepository blogRepository)
        {
            this.blogRepository = blogRepository;
        }
        public async Task GenerateHtaccess()
        {
            var EntitiesHasLastUrl = await blogRepository.GetHasLastUrlAsync();
            var FolderPath = DomainConstants.Explorer.DebugFolder;
            var filePath = $@"{FolderPath}\.htaccess";
            var lines = new List<string>();
            lines.Add("RewriteEngine On");

            foreach (var entity in EntitiesHasLastUrl)
            {
                if (!string.IsNullOrEmpty(entity.LastUrl) && !string.IsNullOrEmpty(entity.WebPageUrl))
                {
                    lines.Add($"RewriteCond %{{REQUEST_URI}} ^/{entity.LastUrl.Replace("https://www.tachiiri.com/", "").Replace("http://www.tachiiri.com/", "").Replace("https://it.tachiiri.com/", "")}$");
                    string redirectRule = $"RewriteRule ^(.*)$ {entity.WebPageUrl} [L,R=301]";
                    lines.Add(redirectRule);
                }
            }

            // UTF-8エンコーディングとLF（\n）改行コードで保存
            var content = string.Join("\n", lines);  // LF改行コードに設定
            File.Delete(filePath);
            File.WriteAllText(filePath, content, new System.Text.UTF8Encoding(false));  // BOMなしのUTF-8
        }
    }
}
