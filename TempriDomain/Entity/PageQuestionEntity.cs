namespace TempriDomain.Entity
{
    public class PageQuestionEntity
    {
        public int QuestionIndex { get; set; }  // 問題の番号（ページ内の順番）
        public List<string> QuestionStringList { get; set; }
        public PrintPageEntity PageEntity { get; set; }  // 親のPageEntity
        public List<QuestionTableEntity> QuestionTableList { get; set; }  // この問題に属するセルのリスト
    }
}
