using System.Net.Http;
using System.Text.Json;

namespace NotionLibrary.Services
{
    public class NotionDatabaseConverter
    {
        public string GetTitleValue(JsonElement record, string columnName)
        {
            var properties = record.GetProperty("properties");
            if (!properties.TryGetProperty(columnName, out var property))
            {
                return "";
            }
            if (!property.TryGetProperty("type", out var type) || type.GetString() != "title")
            {
                return "";
            }
            if (!property.TryGetProperty("title", out var titleArray) || titleArray.ValueKind != JsonValueKind.Array)
            {
                return "";
            }
            if (titleArray.GetArrayLength() == 0)
            {
                return "";
            }
            return titleArray[0].GetProperty("plain_text").GetString();
        }

        public string GetRichTextValue(JsonElement record, string columnName)
        {
            var properties = record.GetProperty("properties");
            if (!properties.TryGetProperty(columnName, out var property))
            {
                return "";
            }
            if (!property.TryGetProperty("type", out var type) || type.GetString() != "rich_text")
            {
                return "";
            }
            if (!property.TryGetProperty("rich_text", out var richTextArray) || richTextArray.ValueKind != JsonValueKind.Array)
            {
                return "";
            }
            if (richTextArray.GetArrayLength() == 0)
            {
                return "";
            }
            return richTextArray[0].GetProperty("plain_text").GetString();
        }
        public List<string> GetRelationPageIds(JsonElement record, string columnName)
        {
            var pageIds = new List<string>();
            var properties = record.GetProperty("properties");
            if (!properties.TryGetProperty(columnName, out var property))
            {
                return pageIds;  // 空のリストを返す
            }
            if (!property.TryGetProperty("type", out var type) || type.GetString() != "relation")
            {
                return pageIds;  // 空のリストを返す
            }
            if (property.TryGetProperty("relation", out var relationArray) && relationArray.ValueKind == JsonValueKind.Array)
            {
                foreach (var relation in relationArray.EnumerateArray())
                {
                    if (relation.TryGetProperty("id", out var id))
                    {
                        pageIds.Add(id.GetString());
                    }
                }
            }
            return pageIds;
        }

        public double GetDoubleValue(JsonElement record, string columnName)
        {
            var properties = record.GetProperty("properties");
            if (!properties.TryGetProperty(columnName, out var property))
            {
                return 0;
            }
            if (!property.TryGetProperty("type", out var type) || type.GetString() != "number")
            {
                return 0;
            }
            if (!property.TryGetProperty("number", out var numberElement) || numberElement.ValueKind != JsonValueKind.Number)
            {
                return 0;
            }
            return numberElement.GetDouble();
        }
        public int GetIntValue(JsonElement record, string columnName)
        {
            var properties = record.GetProperty("properties");
            if (!properties.TryGetProperty(columnName, out var property))
            {
                return 0;
            }
            if (!property.TryGetProperty("type", out var type) || type.GetString() != "number")
            {
                return 0;
            }
            if (!property.TryGetProperty("number", out var numberElement) || numberElement.ValueKind != JsonValueKind.Number)
            {
                return 0;
            }
            return numberElement.GetInt32();
        }
        public DateTime GetCreatedTime(JsonElement record)
        {
            if (!record.TryGetProperty("created_time", out var lastEditedTimeProperty))
            {
                return new DateTime();
            }

            return DateTime.Parse(lastEditedTimeProperty.GetString()); // 日時はISO 8601形式の文字列で返されます
        }
        public DateTime GetLastUpdateTime(JsonElement record)
        {
            if (!record.TryGetProperty("last_update_time", out var lastEditedTimeProperty))
            {
                return new DateTime();
            }

            return DateTime.Parse(lastEditedTimeProperty.GetString()); // 日時はISO 8601形式の文字列で返されます
        }

        public string GetUrlValue(JsonElement record, string ColumnName)
        {
            var properties = record.GetProperty("properties");
            foreach (var property in properties.EnumerateObject())
            {
                if (!property.Value.TryGetProperty("type", out JsonElement type))
                {
                    continue;
                }
                if (type.GetString() != "url")
                {
                    continue;
                }
                if (!property.Value.TryGetProperty("url", out JsonElement urlElement))
                {
                    continue;
                }
                return urlElement.GetString() ?? "";
            }
            return "";
        }
        public List<string> GetMultiSelectValues(JsonElement record, string ColumnName)
        {
            var tags = new List<string>();
            var properties = record.GetProperty("properties");
            if (!properties.TryGetProperty(ColumnName, out var property))
            {
                return tags;
            }
            if (!property.TryGetProperty("type", out JsonElement type) || type.GetString() != "multi_select")
            {
                return tags; // multi_select 列でない場合は空リストを返す
            }
            if (property.TryGetProperty("multi_select", out JsonElement multiSelectArray) && multiSelectArray.ValueKind == JsonValueKind.Array)
            {
                foreach (var tag in multiSelectArray.EnumerateArray())
                {
                    var tagName = tag.GetProperty("name").GetString();
                    tags.Add(tagName);
                }
            }
            return tags;
        }

        public string GetSelectValue(JsonElement record, string ColumnName)
        {
            var properties = record.GetProperty("properties");
            if (!properties.TryGetProperty(ColumnName, out var property))
            {
                return ""; // 指定された列が存在しない場合は空文字を返す
            }
            if (!property.TryGetProperty("type", out JsonElement type) || type.GetString() != "select")
            {
                return ""; // select 列でない場合は空文字を返す
            }
            if (property.TryGetProperty("select", out JsonElement select) && select.ValueKind != JsonValueKind.Null)
            {
                return select.GetProperty("name").GetString();
            }

            return ""; // select が null の場合は空文字を返す
        }

        public string GetId(JsonElement record)
        {
            if (record.TryGetProperty("id", out JsonElement pageIdElement))
            {
                return pageIdElement.GetString();
            }
            return "";
        }
        public async Task ExportNotionFile(JsonElement record, string columnName, string ExportFolder)
        {
            var fileUrls = new List<string>();
            var properties = record.GetProperty("properties");
            if (!properties.TryGetProperty(columnName, out var property))
            {
                return;
            }
            if (!property.TryGetProperty("type", out JsonElement type) || type.GetString() != "files")
            {
                return;
            }
            if (property.TryGetProperty("files", out JsonElement files) && files.ValueKind == JsonValueKind.Array)
            {
                foreach (var file in files.EnumerateArray())
                {
                    if (file.TryGetProperty("file", out JsonElement fileObject) &&
                        fileObject.TryGetProperty("url", out JsonElement url))
                    {
                        fileUrls.Add(url.GetString());
                    }
                }
            }
            if (fileUrls.Count > 0)
            {
                await DownloadAndSaveFileAsync(fileUrls.First(), GetId(record), ExportFolder);

            }
        }

        public async Task<string> DownloadAndSaveFileAsync(string fileUrl, string blockId, string saveDirectory)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    var response = await client.GetAsync(fileUrl);
                    response.EnsureSuccessStatusCode();
                    if (!Directory.Exists(saveDirectory))
                    {
                        Directory.CreateDirectory(saveDirectory);
                    }
                    string fileNameFromHeader = response.Content.Headers.ContentDisposition?.FileName?.Trim('"');
                    string extension = Path.GetExtension(fileNameFromHeader);
                    if (string.IsNullOrEmpty(extension))
                    {
                        string fileNameFromUrl = Path.GetFileName(new Uri(fileUrl).AbsolutePath);
                        extension = Path.GetExtension(fileNameFromUrl);
                    }
                    if (string.IsNullOrEmpty(extension))
                    {
                        extension = ".jpg"; // デフォルト拡張子を指定（適宜変更）
                    }
                    var fileName = Path.Combine(saveDirectory, $"{blockId}{extension}");
                    using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fileStream);
                    }
                    return $"{blockId}{extension}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ファイルのダウンロードに失敗しました: {ex.Message}");
                return string.Empty;
            }
        }

    }
}
