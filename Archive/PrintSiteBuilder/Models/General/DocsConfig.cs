using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder.Models.General
{
    public class DocsConfig
    {
        public List<DocConfig> itemConfigList { get; set; }
        public List<DocConfig> singleItemConfigList { get; set; }
        public List<DocConfig> groupItemConfigList { get; set; }
        public List<string> MarkdownPaths { get; set; }
        public List<string> DocImagePaths { get; set; }
        public List<string> GroupCodes { get; set; }

        public HashSet<string> Keys { get; set; }
        public HashSet<string> Tags { get; set; }
    }
}
