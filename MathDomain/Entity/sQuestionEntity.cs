

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathDomain.Entity
{
#if DEBUG
    [Table("s_question")]
#elif RELEASE
    [Table("s_question")]
#endif
    [PrimaryKey(nameof(Id))]
    public class sQuestionEntity
    {
        public int Id { get; set; }
        public string PageId { get; set; }
        public int QuestionId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
