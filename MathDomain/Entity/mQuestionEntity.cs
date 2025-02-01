using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MathDomain.Entity
{
#if DEBUG
    [Table("m_question")]
#elif RELEASE
    [Table("m_question")]
#endif
    [PrimaryKey(nameof(Id))]
    public class mQuestionEntity
    {
        public int Id { get; set; }
        public string PageId { get; set; }
        public int QuestionId { get; set; }
        public int ValueId { get; set; }
        public string Value { get; set; }
        public string Context { get; set; }
        public bool HasNext { get; set; }
        public bool IsQuestion { get; set; }
    }
}
