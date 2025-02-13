namespace TempriDomain.Entity
{
    public class PageTableEntity
    {
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public string Value { get; set; }
        public List<int> AnswerColumn { get; set; }
        public List<int> AnswerRow { get; set; }
        PageEntity pageEntity { get; set; }
    }
}
