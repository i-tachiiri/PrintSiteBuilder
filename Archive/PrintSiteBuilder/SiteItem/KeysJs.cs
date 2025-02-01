using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder.Models.General;
using PrintSiteBuilder.SiteItem;
using PrintSiteBuilder.Utilities;
using PrintSiteBuilder.Archive;


namespace PrintSiteBuilder.SiteItem
{
    public class KeysJs
    {
        public string CreateKeysText()
        {
            var itemName = new Item();
            //var HtmlItemNames = itemName.GetItemNames(GlobalConfig.HtmlDir, "html");
            var SingleItemPaths = Directory.GetFiles(GlobalConfig.SvgDir, $"*.svg").ToList();
            var GroupItemPaths = Directory.GetFiles(GlobalConfig.SvgGroupDir, $"*.svg").ToList();
            var ItemPaths = SingleItemPaths.Concat(GroupItemPaths).ToList();
            var Script = "const keys = [";
            foreach (string ItemPath in ItemPaths)
            {
                Script += $"'{Path.GetFileNameWithoutExtension(ItemPath)}',";
            }
            Script = $"{Script.TrimEnd(',')}];";
            return Script;
        }
        public void BackupKeys()
        {
            var BackupFileName = DateTime.Now.ToString("yyyyMMddhhmmss");
            File.Copy(GlobalConfig.KeysPath, $@"{GlobalConfig.KeysBackupPath}\{BackupFileName}.js");
        }
        public void UpdateKeys(ItemsConfig itemsConfig, DocsConfig docsConfig)
        {
            //var KeysText = CreateKeysText();
            //System.IO.File.WriteAllText(GlobalConfig.KeysPath, KeysText);
            var json = new Json();
            var KeysConfig = SetKeysConfig(itemsConfig, docsConfig);
            json.SerializeKeysConfig(KeysConfig);
        }
        public KeysConfig SetKeysConfig(ItemsConfig itemsConfig, DocsConfig docsConfig)
        {
            var keysConfig = new KeysConfig();
            var ItemKeysHaseSet = new HashSet<string>();
            var ConfigList = itemsConfig.itemConfigList; //itemsConfig.itemConfigList.Concat(docsConfig.itemConfigList); --docを一旦諦める
            keysConfig.KeyConfigList = new List<KeyConfig>();
            foreach (ItemConfig itemConfig in ConfigList)
            {
                if (ItemKeysHaseSet.Contains(itemConfig.ItemKey))
                {
                    continue;
                }
                ItemKeysHaseSet.Add(itemConfig.ItemKey);
                var keyConfig = new KeyConfig();
                keyConfig.title = itemConfig.Title;
                keyConfig.description = itemConfig.Description;
                keyConfig.itemName = itemConfig.ItemName;
                keyConfig.itemKey = itemConfig.ItemKey;
                keysConfig.KeyConfigList.Add(keyConfig);
            }
            return keysConfig;
        }
    }
}
