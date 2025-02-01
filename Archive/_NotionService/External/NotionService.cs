using Microsoft.VisualBasic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using NotionService.Config;
namespace NotionService.External
{
    public class NotionService
    {
        private readonly HttpClient _httpClient;
        private const string NotionApiBaseUrl = "https://api.notion.com/v1";
        private const string NotionVersion = "2022-06-28"; // Notion API のバージョン

        public NotionService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", InfrastructureConstants.Notion.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("Notion-Version", NotionVersion);
        }
        public async Task<JsonElement> GetDatabaseAsync()
        {
            string url = $"{NotionApiBaseUrl}/databases/{InfrastructureConstants.Notion.TempriId}/query";
            var response = await _httpClient.PostAsync(url, null);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(content);
            return jsonDocument.RootElement.GetProperty("results");
        }
        public async Task<bool> InsertRowAsync(string status, string printname, string filename)
        {
            string url = $"{NotionApiBaseUrl}/pages";

            // JSONボディを作成
            var jsonBody = new
            {
                parent = new { database_id = InfrastructureConstants.Notion.TempriId },
                properties = new
                {
                    status = new
                    {
                        select = new { name = status } // selectプロパティ
                    },
                    printname = new
                    {
                        title = new[]
                        {
                            new { text = new { content = printname } } // titleプロパティ
                        }
                    },
                    filename = new
                    {
                        rich_text = new[]
                        {
                            new { text = new { content = filename } } // rich_textプロパティ
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(jsonBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                return true; // 成功
            }
            else
            {
                // エラー内容をログ出力などで確認
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {errorContent}");
                return false; // 失敗
            }
        }

        // ページIDからページのテキストを取得
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
