using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder2409.Entities
{
    [Table("m_print")]
    [PrimaryKey(nameof(PrintId))]
    public class PrintMaster
    {
        public string TemplateId { get; set; }

        public string PrintId { get; set; }
        public string PrintName { get; set; }

        public string SkuHeader { get; set; }

        public int PagesCount { get; set; }

        public int Score { get; set; }
    }
}
