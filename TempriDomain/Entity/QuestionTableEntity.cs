namespace TempriDomain.Entity
{
    public class QuestionTableEntity
    {
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public string Value { get; set; }
        public bool IsAnswerCell {  get; set; }
        public PageQuestionEntity pageQuestionEntity { get; set; }
    }
}
