using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder2409.Entities
{
    [Table("m_slide")]
    [PrimaryKey(nameof(PrintId))]
    public class SlideMaster
    {
        public string PrintId { get; set; }
        public string Attribute { get; set; }
        public int PageCount { get; set; }
        public int TemplateNumber { get; set; }
        public string FolderId { get; set; }
        public string PrntSlideId { get; set; }
        public string CoverSlideId { get; set; }
        public string AmazonSlideId { get; set; }
        public string Uuid { get; set; }
        public string Createdtime { get; set; }
    }
}
