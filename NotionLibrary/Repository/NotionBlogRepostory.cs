using BlogDomain.Services;
using NotionLibrary.Config;
using NotionLibrary.External;
using NotionLibrary.Services;
using System.Net.Http.Headers;
using System.Text.Json;

namespace NotionLibrary.Repository
{
    public class NotionBlogRepostory
    {
        private readonly HttpClient httpClient;
        private NotionConnecter connecter;
        private NotionDatabaseConverter databaseConverter;
        private string TestPageId = "115d9f5c28f580bca121f21c20eb4d58";
        private Logger logger;

        public NotionBlogRepostory(NotionConnecter connecter, NotionDatabaseConverter databaseConveter, Logger logger)
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", NotionConstants.ApiKey);
            httpClient.DefaultRequestHeaders.Add("Notion-Version", NotionConstants.NotionVersion);
            this.connecter = connecter;
            this.databaseConverter = databaseConveter;
            this.logger = logger;
        }
        public async Task ExportNotionFiles(string ColumnName,string ExportFolder)
        {
            var records = await connecter.GetDatabaseAsync(NotionConstants.BlogId);//notion.GetDatabaseAsync();
            foreach (var record in records.EnumerateArray())
            {
#if DEBUG
                if (databaseConverter.GetId(record).Replace("-", "") != TestPageId) continue;
#endif
                try
                {
                    await databaseConverter.ExportNotionFile(record, ColumnName, ExportFolder);
                }
                catch(Exception ex)
                {
                    logger.ExceptionLog(ex.Message);
                }
            }
        }

        public async Task<JsonElement> GetAllAsync()
        {
            return await connecter.GetDatabaseAsync(NotionConstants.BlogId);
        }

        /*public async Task InsertUniqueTemplateAsync(List<TemplateEntity> Entities)
        {
            var results = await GetAllAsync();
            var UniqueEntities = new List<TemplateEntity>();
            foreach (var entity in Entities)
            {
                if (!results.Any(result => result.Summary == entity.Summary))
                {
                    UniqueEntities.Add(entity);
                }
            }
            await InsertTemplatesAsync(UniqueEntities);
        }*/


    }
}
