using System.Text.Json;

namespace NotionLibrary.Services
{
    public class NotionPropertyConverter
    {
        public object SelectProperty(string value)
        {
            return new
            {
                select = new { name = value }
            };
        }

        public object TitleProperty(string value)
        {
            return new
            {
                title = new[]
                {
                    new { text = new { content = value } }
                }
            };
        }

        public object RichTextProperty(string value)
        {
            return new
            {
                rich_text = new[]
                {
                    new { text = new { content = value } }
                }
            };
        }
        public string GetSelectProperty(JsonElement properties, string propertyName)
        {
            try
            {
                return properties
                    .GetProperty(propertyName)        // 指定されたプロパティ名を取得
                    .GetProperty("select")            // select型のプロパティ
                    .GetProperty("name")              // 選択された値の名前を取得
                    .GetString();
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine($"Property {propertyName} not found.");
                return "";
            }
        }
        public string GetTitleProperty(JsonElement properties, string propertyName)
        {
            try
            {
                return properties
                    .GetProperty(propertyName)        // 指定されたプロパティ名を取得
                    .GetProperty("title")             // title型のプロパティ
                    .EnumerateArray()                 // titleは配列で格納されている
                    .FirstOrDefault()                 // 配列の最初の要素を取得
                    .GetProperty("text")
                    .GetProperty("content")
                    .GetString();
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine($"Property {propertyName} not found.");
                return "";
            }
        }


    }
}
