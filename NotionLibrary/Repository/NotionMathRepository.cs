using BlogDomain.Services;
using NotionLibrary.Config;
using NotionLibrary.External;
using NotionLibrary.Services;
using System.Net.Http.Headers;
using System.Text.Json;

namespace NotionLibrary.Repository
{
    public class NotionMathRepository
    {
        private readonly HttpClient httpClient;
        private NotionConnecter connecter;
        private NotionDatabaseConverter databaseConverter;

        public NotionMathRepository(NotionConnecter connecter, NotionDatabaseConverter databaseConveter)
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", NotionConstants.ApiKey);
            httpClient.DefaultRequestHeaders.Add("Notion-Version", NotionConstants.NotionVersion);
            this.connecter = connecter;
            this.databaseConverter = databaseConveter;
        }
        public async Task<JsonElement> GetAllAsync()
        {
            return await connecter.GetDatabaseAsync(NotionConstants.MathId);
        }
    }
}
