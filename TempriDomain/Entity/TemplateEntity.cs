using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TempriDomain.Entity
{

    [Table("m_template")]
    [PrimaryKey(nameof(PrintId))]
    public class TemplateEntity
    {
        public int PrintId { get; set; }
        public string PageId { get; set; }
        public string PrintName { get; set; }
        public string Template { get; set; }
        public string Status { get; set; }
        public string Summary { get; set; }
    }
}
