

using GoogleSlideLibrary.Repository.PageTable;
using TempriDomain.Entity;

namespace GoogleSlideLibrary.Repository.PageQuestion
{
    public class PageQuestion100007
    {
        private QuestionTable100007 questionTable100007;
        private int seed;
        private int QuestionCount = 10;
        private int MaxQuestionCount = 1000;
        public PageQuestion100007(QuestionTable100007 questionTable100007)
        {
            this.questionTable100007 = questionTable100007;
        }
        /// <summary>
        /// PageQuestionEntityを返します。
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public async Task<List<PageQuestionEntity>> SetPageQuestionEntity(int pageNumber)
        {
            seed = pageNumber;
            var entities = SetQuestions();

            //Select "QuestionCount" number of random questions
            //Seed is fixed by PageNumber
            Random random = new Random(pageNumber);
            var selectedQuestions = entities.OrderBy(_ => random.Next()).Take(QuestionCount).ToList();

            //Set child entity list(QuestionCells) for each parent entity(PageQuestion)
            var tasks = selectedQuestions.Select(async entity =>
            {
                entity.QuestionTableList = await questionTable100007.SetQuestionCellAsync(selectedQuestions);
            });
            await Task.WhenAll(tasks); 

            // Set the parent entity(PageQuestionEntity) for each child entity(QuestionCells)
            foreach (var question in selectedQuestions)
            {
                foreach (var questionTableEntity in question.QuestionTableList)
                {
                    questionTableEntity.pageQuestionEntity = question;
                }
            }
            return selectedQuestions;
        }
        /// <summary>
        /// 問題の文字列を作成し、QuestionIndexと共に、PageQuestionEntityのリストを返します。
        /// 問題数の上限はMaxQuestionCOuntです。
        /// </summary>
        /// <returns></returns>
        public List<PageQuestionEntity> SetQuestions()
        {
            var entities = new List<PageQuestionEntity>();
            var indexList = GenerateRandomIndexList(1, 9); 

            //Add question when...
            //x and y is between 1 and 9
            //x + y is less than 10
            //total entities under 1000
            for (var i = 0; i < indexList.Count; i++)
            {
                for (var j = 0; j < indexList.Count; j++)
                {
                    if (indexList[i] + indexList[j] <= 10)
                    {
                        if (entities.Count >= MaxQuestionCount) return entities;

                        //Add QuestionStringList
                        var index = entities.Count;
                        entities.Add(new PageQuestionEntity()
                        {
                            QuestionIndex = index,
                            QuestionStringList = new List<string>()
                            {
                                indexList[i].ToString(),
                                "+",
                                indexList[j].ToString(),
                                "=",
                                (indexList[i] + indexList[j]).ToString()
                            }
                        });
                    }
                }
            }
            return entities;
        }

        /// <summary>
        /// 指定された範囲の数値リストを作成し、シード値を使ってランダムな順序に並べる。
        /// 算数専用の数値を出す関数ではなく、問題番号のインデックスを取得する関数。
        /// </summary>
        /// <param name="min">リストの最小値</param>
        /// <param name="max">リストの最大値</param>
        /// <param name="seed">シード値（毎回同じ順序を得るため）</param>
        /// <returns>ランダムに並べられた数値リスト</returns>
        private List<int> GenerateRandomIndexList(int min, int max)
        {
            var random = new Random(seed);
            return Enumerable.Range(min, max - min + 1).OrderBy(_ => random.Next()).ToList();
        }
    }
}
