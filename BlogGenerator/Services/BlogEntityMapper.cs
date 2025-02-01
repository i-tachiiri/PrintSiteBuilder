
using BlogDomain.Config;
using BlogDomain.Entity;
using BlogDomain.Services;
using BlogGenerator.Application.Services;
using MysqlLibrary.Repository;
using MysqlLibrary.Repository.Print;
using NotionLibrary.Repository;
using NotionLibrary.Services;

namespace BlogGenerator.Services
{
    public class BlogEntityMapper
    {
        private NotionBlogRepostory notionBlogRepostory;
        private NotionDatabaseConverter databaseConverter;
        private NotionPageRepository pageRepository;
        private NotionPageConverter pageConverter;
        private HtmlGenerator htmlGenerator;
        private MysqlBlogRepository mysqlBlogRepository;
        private TextVectorCalculator textVectorCalculator;
        private Logger logger;
        private MysqlSearchRepository mysqlSearchRepository;
        private string TestPageId = "115d9f5c28f580bca121f21c20eb4d58";
        public BlogEntityMapper(NotionBlogRepostory notionBlogRepostory, NotionDatabaseConverter databaseConveter, NotionPageRepository pageRepository, NotionPageConverter pageConverter, HtmlGenerator htmlGenerator, MysqlBlogRepository mysqlBlogRepository, TextVectorCalculator textVectorCalculator, Logger logger, MysqlSearchRepository mysqlSearchRepository)
        {
            this.databaseConverter = databaseConveter;
            this.notionBlogRepostory = notionBlogRepostory;
            this.pageConverter = pageConverter;
            this.pageRepository = pageRepository;
            this.htmlGenerator = htmlGenerator;
            this.mysqlBlogRepository = mysqlBlogRepository;
            this.textVectorCalculator = textVectorCalculator;
            this.logger = logger;
            this.mysqlSearchRepository = mysqlSearchRepository;
        }

        public async Task UpsertBlogEntitiesAsync()//(string ExportFolder, string BlogUrl)
        {
            var results = await notionBlogRepostory.GetAllAsync();
            var Entities = new List<BlogEntity>();
            foreach (var record in results.EnumerateArray())
            {
#if DEBUG
                //if (databaseConverter.GetId(record).Replace("-","") != TestPageId) continue;
#endif
                var entity = new BlogEntity();
                entity.Title = databaseConverter.GetTitleValue(record, "title");
                entity.Directory = databaseConverter.GetSelectValue(record, "directory");
                entity.MainCategory = databaseConverter.GetSelectValue(record, "main-category");
                entity.LocalFolder = Directory.CreateDirectory(Path.Combine(DomainConstants.Explorer.BlogDebugFolder, entity.Directory)).FullName;
                entity.LastUrl = databaseConverter.GetUrlValue(record, "last_url");
                entity.Status = databaseConverter.GetSelectValue(record, "status");
                entity.PageId = databaseConverter.GetId(record);
                entity.CreatedTime = databaseConverter.GetCreatedTime(record);
                entity.NotionPageUrl = $"https://notion.so/{entity.PageId}";
                entity.WebPageUrl = $"{DomainConstants.Web.BlogUrl}/{entity.Directory}/{entity.PageId}.html";
                entity.Header = DomainConstants.BlogTitles.TryGetValue(entity.Directory, out var blogTitle) ? blogTitle : "立入ブログ";
                entity.LastRecordUpdated = DateTime.Now;

                var blocks = await pageRepository.GetPageAsync(entity.PageId);
                entity.Content = await pageConverter.ConvertBlocksToHtml(blocks, entity.PageId, entity.Title, entity.LocalFolder, entity.Directory);
                entity.TextVector = "{}";// textVectorCalculator.CalcTextVector(entity.Title);
                entity.TableOfContent = pageConverter.GenerateTableOfContents(blocks);
                entity.ImageUrl = pageConverter.GetFirstImageUrl(blocks, entity, DomainConstants.Web.BlogUrl);
                Entities.Add(entity);
                logger.LoopLog(results.GetArrayLength(), "CreateBlogEntities",entity.Title);

            }
            // Entities の作成が完了した後にループして LastPostUrl と NextPostUrl を設定
            for (int i = 0; i < Entities.Count; i++)
            {
                var currentEntity = Entities[i];
                var previousIndex = (i == 0) ? Entities.Count - 1 : i - 1;
                currentEntity.LastPostUrl = $@"{Entities[previousIndex].Directory}/{Entities[previousIndex].PageId}.html";
                var nextIndex = (i == Entities.Count - 1) ? 0 : i + 1;
                currentEntity.NextPostUrl = $@"{Entities[nextIndex].Directory}/{Entities[nextIndex].PageId}.html";
            }
            var PopularPostIds = await mysqlSearchRepository.GetMostClickedAsync();
            var LatestPostIds = await mysqlBlogRepository.GetLatestPostIdsAsync();
            foreach (var entity in Entities)
            {
                entity.RelatedPosts = "";
                entity.Summary = htmlGenerator.GenerateSummary(entity);
                entity.PopularPosts = htmlGenerator.GeneratePopularPosts(Entities, PopularPostIds);
                entity.RelatedPosts = await htmlGenerator.GenerateRelatedPosts(entity);
                entity.LatestPosts = await htmlGenerator.GenerateLatestPosts(Entities, LatestPostIds);
                entity.HtmlContent = htmlGenerator.CombineEntityAndTemplate(entity, DomainConstants.Explorer.BlogDebugFolder, DomainConstants.Web.BlogUrl);
            }
            await mysqlBlogRepository.UpsertBlogListAsync(Entities);
        }
    }
}
