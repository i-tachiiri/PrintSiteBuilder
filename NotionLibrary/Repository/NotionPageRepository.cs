using NotionLibrary.Config;
using System.Net.Http.Headers;
using System.Text.Json;

namespace NotionLibrary.Repository
{
    public class NotionPageRepository
    {
        private readonly HttpClient _httpClient;

        public NotionPageRepository()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", NotionConstants.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("Notion-Version", NotionConstants.NotionVersion);
        }
        public async Task<JsonElement> GetPageAsync(string pageID)
        {

            string url = $"https://api.notion.com/v1/blocks/{pageID}/children";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(content);
            return jsonDocument.RootElement.GetProperty("results");
        }
        public JsonElement GetTableRows(string tableBlockId)
        {
            string url = $"https://api.notion.com/v1/blocks/{tableBlockId}/children";
            var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();  // ステータスコードが正常であることを確認
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var jsonDocument = JsonDocument.Parse(content);
            return jsonDocument.RootElement.GetProperty("results");
        }
    }
}
