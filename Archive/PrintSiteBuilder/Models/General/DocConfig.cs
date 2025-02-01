using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder.Models.General
{
    public class DocConfig
    {
        public string MarkdownPath { get; set; }
        public string QrPath { get; set; }
        public string MarkdownName { get; set; }
        public string CategoryName { get; set; }
        public string ItemKey { get; set; }
        public string ItemType { get; set; }
        public string GroupName { get; set; }
        public List<string> Tags { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> DocImageNames { get; set; }
        public List<string> DocImagePaths { get; set; }
        public bool IsGroup { get; set; }
        public bool IsHtmlUpdated { get; set; }
        public bool IsVertical { get; set; }
        public bool IsInvalidSvg { get; set; }
        public bool IsQrUpdated { get; set; }
        public bool IsPdfUpdated { get; set; }
    }
}
