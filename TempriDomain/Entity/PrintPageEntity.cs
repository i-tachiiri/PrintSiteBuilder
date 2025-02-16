﻿namespace TempriDomain.Entity
{
    public class PrintPageEntity
    {
        public string PageObjectId { get; set; }
        public int PageNumber { get; set; }  // このページが何番目か
        public int PageIndex { get; set; }   // Googleスライド上のインデックス
        public bool IsAnswerPage { get; set; }

        public PrintEntity PrintEntity { get; set; }  // 親のPrintEntity
        public List<PageQuestionEntity> QuestionList { get; set; }  // このページの問題リスト
    }
}
