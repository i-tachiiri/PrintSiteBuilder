using Microsoft.Extensions.Logging;
using NotionLibrary.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BlogDomain.Services;
using System.Net.Http;
namespace NotionLibrary.External
{
    public class NotionConnecter
    {
        private readonly HttpClient _httpClient;
        public NotionConnecter()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", NotionConstants.ApiKey);
            _httpClient.DefaultRequestHeaders.Add("Notion-Version", NotionConstants.NotionVersion);
        }
        public async Task<JsonElement> GetTop100DatabaseAsync(string DatabaseId)
        {
            string url = $"{NotionConstants.NotionApiBaseUrl}/databases/{DatabaseId}/query";
            var response = await _httpClient.PostAsync(url, null);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(content);
            return jsonDocument.RootElement.GetProperty("results");
        }
        public async Task<JsonElement> GetDatabaseAsync(string DatabaseId)
        {
            var resultsList = new List<JsonElement>();
            string url = $"{NotionConstants.NotionApiBaseUrl}/databases/{DatabaseId}/query";
            string nextCursor = null;
            bool hasMore = false; // ループの継続を制御するフラグ

            do
            {
                // start_cursor が null でない場合だけペイロードに含める
                var payload = nextCursor != null
                    ? new { start_cursor = nextCursor }
                    : null;

                var content = new StringContent(payload != null ? JsonSerializer.Serialize(payload) : "{}", Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode(); // ここでエラーが発生

                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(responseContent);
                var root = jsonDocument.RootElement;

                // 現在のページの "results" を取得してリストに追加
                foreach (var result in root.GetProperty("results").EnumerateArray())
                {
                    resultsList.Add(result);
                }

                // ページネーション用の next_cursor を取得
                nextCursor = root.TryGetProperty("next_cursor", out var cursor) && cursor.ValueKind != JsonValueKind.Null
                    ? cursor.GetString()
                    : null;

                // "has_more" の値を取得してループ継続を判断
                hasMore = root.GetProperty("has_more").GetBoolean();

            } while (hasMore); // has_more が true なら次のページを取得

            // 全ての結果を1つの "results" 配列として結合して新しい JSON を作成
            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(new { results = resultsList }, options);
            var mergedJsonDocument = JsonDocument.Parse(jsonString);

            return mergedJsonDocument.RootElement.GetProperty("results"); ;
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
        public async Task<JsonElement> GetPagePropertiesAsync(string pageID)
        {
            string url = $"https://api.notion.com/v1/pages/{pageID}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(content);

            // "properties" フィールドを取得
            return jsonDocument.RootElement.GetProperty("properties");
        }

        public async Task InsertRowsAsync(List<string> jsonBodies)
        {
            var tasks = jsonBodies.Select(async body =>
            {
                string url = $"{NotionConstants.NotionApiBaseUrl}/pages";
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {errorContent}");
                }
            });
            await Task.WhenAll(tasks);
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
