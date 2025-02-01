using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrainingDomain.Entity
{

    [Table("m_yes_no_question")]
    [PrimaryKey(nameof(Id))]
    public class YesNoQuestionEntity
    {
        public int Id {  get; set; }
        public string Question {  get; set; }
        public string Answer { get; set; }
        public int CorrectAnswerCount { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CorrectAnswerTime { get; set; }
        public DateTime IncorrectAnswerTime { get; set; }

    }
}
