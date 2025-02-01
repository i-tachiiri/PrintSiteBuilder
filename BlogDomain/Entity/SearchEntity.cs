using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;


namespace BlogDomain.Entity
{
#if DEBUG
    [Table("m_search")]
#elif RELEASE
    [Table("m_search")]
#endif
    [PrimaryKey(nameof(FromDate), nameof(Period),nameof(PageId),nameof(Query))]
    public class SearchEntity
    {
        public DateTime FromDate { get; set; }
        public int Period { get; set; }
        public string PageId { get; set; }          
        public string Query { get; set; }        
        public Double Impression { get; set; }       
        public Double Position { get; set; }
        public Double? Click { get; set; }
    }
}