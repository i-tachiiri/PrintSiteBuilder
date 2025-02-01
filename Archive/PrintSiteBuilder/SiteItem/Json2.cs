using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Encodings.Web;
using PrintSiteBuilder.Interfaces;
using PrintSiteBuilder.Models.General;

namespace PrintSiteBuilder.SiteItem
{
    public class Json2
    {
        public void SerializeSlidesConfig(SlidesConfig slidesConfig, IPrint2 iPrint)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true // 読みやすい形式で出力
            };
            string jsonString = JsonSerializer.Serialize(slidesConfig, options);
            File.WriteAllText(iPrint.path.PrintSlideConfig, jsonString);
        }
        public void SerializeItemsConfig(ItemsConfig itemsConfig, IPrint2 iPrint)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true // 読みやすい形式で出力
            };
            string jsonString = JsonSerializer.Serialize(itemsConfig, options);
            File.WriteAllText(iPrint.path.PrintConfig, jsonString);
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
        public void SerializeAnyConfig(object Config,string FilePath)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true // 読みやすい形式で出力
            };
            string jsonString = JsonSerializer.Serialize(Config, options);
            File.WriteAllText(FilePath, jsonString);
        }
        public SlidesConfig DeserializeSlidesConfig(IPrint2 iPrint)
        {
            string jsonString = File.ReadAllText(iPrint.path.PrintSlideConfig);
            return JsonSerializer.Deserialize<SlidesConfig>(jsonString);
        }
        public ItemsConfig DeserializeItemsConfig(IPrint2 iPrint)
        {
            string jsonString = File.ReadAllText(iPrint.path.PrintConfig);
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
