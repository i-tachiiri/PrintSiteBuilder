using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Encodings.Web;
using PrintSiteBuilder.Models.General;

namespace PrintSiteBuilder.Archive
{
    public class Json
    {
        public void SerializeItemsConfig(ItemsConfig itemsConfig)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true // 読みやすい形式で出力
            };
            string jsonString = JsonSerializer.Serialize(itemsConfig, options);
            File.WriteAllText(GlobalConfig.ItemsConfigPath, jsonString);
        }
        public void SerializeDocsConfig(DocsConfig itemsConfig)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true // 読みやすい形式で出力
            };
            string jsonString = JsonSerializer.Serialize(itemsConfig, options);
            File.WriteAllText(GlobalConfig.DocsConfigPath, jsonString);
        }
        public void SerializeKeysConfig(KeysConfig keysConfig)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true // 読みやすい形式で出力
            };
            string jsonString = JsonSerializer.Serialize(keysConfig, options);
            File.WriteAllText(GlobalConfig.KeysConfigPath, jsonString);
        }
        public ItemsConfig DeserializeItemsConfig()
        {
            string jsonString = File.ReadAllText(GlobalConfig.ItemsConfigPath);
            return JsonSerializer.Deserialize<ItemsConfig>(jsonString);
        }
        public DocsConfig DeserializeDocsConfig()
        {
            string jsonString = File.ReadAllText(GlobalConfig.DocsConfigPath);
            return JsonSerializer.Deserialize<DocsConfig>(jsonString);
        }
        public ItemsConfig DeserializeKeysConfig()
        {
            string jsonString = File.ReadAllText(GlobalConfig.KeysConfigPath);
            return JsonSerializer.Deserialize<ItemsConfig>(jsonString);
        }
    }
}
