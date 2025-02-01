using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;


namespace BlogDomain.Entity
{
#if DEBUG
    [Table("m_cosine")]
#elif RELEASE
    [Table("m_cosine")]
#endif
    [PrimaryKey(nameof(BaseId),nameof(TargetId))]
    public class CosineEntity
    {
        public string BaseId { get; set; }          // VARCHAR(45)
        public string TargetId { get; set; }        // VARCHAR(45)
        public string BaseTitle { get; set; }       // VARCHAR(1000)
        public string TargetTitle { get; set; }     // VARCHAR(1000)
        public double TitleSimilarity { get; set; } // DOUBLE
        public double TextSimilarity { get; set; }  // DOUBLE
        public double Score { get; set; }  // DOUBLE
        public DateTime LastUpdated { get; set; }   // DATETIME
    }
}
