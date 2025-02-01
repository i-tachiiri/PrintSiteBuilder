using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
namespace BlogDomain.Entity
{
#if DEBUG
    [Table("m_blog")]
#elif RELEASE
    [Table("m_blog")]
#endif
    [PrimaryKey(nameof(Id))]
    public class BlogEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PageId { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string Directory { get; set; }
        public string MainCategory { get; set; }
        public string LocalFolder { get; set; }
        public string NotionPageUrl { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastRecordUpdated { get; set; }

        public string LastUrl { get; set; }
        //public List<string> Categories { get; set; }
        public string Status { get; set; }
        public string Header { get; set; }
        public string ImageUrl { get; set; }
        public string WebPageUrl { get; set; }
        public string LastPostUrl { get; set; }
        public string NextPostUrl { get; set; }
        public string PopularPosts { get; set; }
        public string RelatedPosts { get; set; }
        public string LatestPosts { get; set; }

        public string TextVector { get; set; }
        public string HtmlContent { get; set; }
        public string TableOfContent { get; set; }
    }
}
