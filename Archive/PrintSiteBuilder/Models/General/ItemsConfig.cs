using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder.Models.General
{
    public class ItemsConfig
    {
        public List<ItemConfig> itemConfigList { get; set; }
        public List<ItemConfig> singleItemConfigList { get; set; }
        public List<ItemConfig> groupItemConfigList { get; set; }
        public List<string> itemPaths { get; set; }
        public List<string> GroupCodes { get; set; }


        public HashSet<string> Keys { get; set; }
        public HashSet<string> Tags { get; set; }
    }
}
