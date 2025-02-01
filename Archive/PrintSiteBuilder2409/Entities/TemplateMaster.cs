using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder2409.Entities
{
    [Table("m_template")]
    public class TemplateMaster
    {

        [Key]
        public string TemplateId { get; set; }
        public string SkuHeader { get; set; }
        public int Score { get; set; }
        public int ColumnNumber { get; set; }
        public int RowNumber { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
