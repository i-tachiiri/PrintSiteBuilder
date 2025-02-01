using BlogDomain.Config;
using NotionLibrary.Repository;

namespace BlogGenerator.Services
{
    public class NotionFileDownloader
    {
        private NotionBlogRepostory notionBlogRepository;
        public NotionFileDownloader(NotionBlogRepostory notionBlogRepository)
        {
            this.notionBlogRepository = notionBlogRepository;
        }
        public async Task DownloadNotionFiles()
        {

                await notionBlogRepository.ExportNotionFiles("mp3", DomainConstants.Explorer.BlogMp3Folder);
                await notionBlogRepository.ExportNotionFiles("pdf", DomainConstants.Explorer.BlogPdfFolder);
            
        }
    }
}
