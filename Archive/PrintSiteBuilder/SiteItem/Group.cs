using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder.Models.General;

namespace PrintSiteBuilder.SiteItem
{
    public class Group
    {
        public void CreateGroupConfig()
        {
            var directories = Directory.GetDirectories(GlobalConfig.GroupDir);
            var FolderNames = directories.Select(item => Path.GetFileName(item));
            var Keys = new HashSet<string>(File.ReadAllLines(GlobalConfig.GroupConfigPath).Select(line => line.Split(',')[0])); ;
            foreach (var folderName in FolderNames)
            {
                if (!Keys.Contains(folderName))
                {
                    File.AppendAllText(GlobalConfig.GroupConfigPath, $@"{folderName}{Environment.NewLine}");
                }
            }
        }

    }
}
