using TempriDomain.Entity;

namespace GoogleSlideLibrary.Repository.PageTable
{
    public class QuestionTable100007
    {
        private List<List<(int, int)>> questionCells = new List<List<(int, int)>>
        {
            new List<(int, int)> { (0, 0), (0, 1), (0, 2), (0, 3), (0, 4), (0, 5) },
            new List<(int, int)> { (2, 0), (2, 1), (2, 2), (2, 3), (2, 4), (2, 5) },
            new List<(int, int)> { (4, 0), (4, 1), (4, 2), (4, 3), (4, 4), (4, 5) },
            new List<(int, int)> { (6, 0), (6, 1), (6, 2), (6, 3), (6, 4), (6, 5) },
            new List<(int, int)> { (8, 0), (8, 1), (8, 2), (8, 3), (8, 4), (8, 5) },
            new List<(int, int)> { (0, 7), (0, 8), (0, 9), (0, 10), (0, 11), (0, 12) },
            new List<(int, int)> { (2, 7), (2, 8), (2, 9), (2, 10), (2, 11), (2, 12) },
            new List<(int, int)> { (4, 7), (4, 8), (4, 9), (4, 10), (4, 11), (4, 12) },
            new List<(int, int)> { (6, 7), (6, 8), (6, 9), (6, 10), (6, 11), (6, 12) },
            new List<(int, int)> { (8, 7), (8, 8), (8, 9), (8, 10), (8, 11), (8, 12) },
        };
        private List<List<(int, int)>> answerCells = new List<List<(int, int)>>
        {
            new List<(int, int)> { (0, 5) },
            new List<(int, int)> { (2, 5) },
            new List<(int, int)> { (4, 5) },
            new List<(int, int)> { (6, 5) },
            new List<(int, int)> { (8, 5) },
            new List<(int, int)> { (0, 12) },
            new List<(int, int)> { (2, 12) },
            new List<(int, int)> { (4, 12) },
            new List<(int, int)> { (6, 12) },
            new List<(int, int)> { (8, 12) },
        };
        /// <summary>
        /// pageQuestionEntity=問題の文字列リストを元に、questionTableEntityのリストを作成します。
        /// questionCellsとanswerCellsで定義された行番号・列番号を元に、問題の文字列のRowNumber,ColumnNumber,Value,IsAnswerCellを設定します。
        /// </summary>
        /// <param name="questionEntityList"></param>
        /// <returns></returns>
        public async Task<List<QuestionTableEntity>> SetQuestionCellAsync(List<PageQuestionEntity> questionEntityList)
        {
            var entities = new List<QuestionTableEntity>();
            for(var i = 0;i< questionEntityList.Count;i++)
            {
                for(var j = 0; j < questionEntityList[i].QuestionStringList.Count;j++)
                {
                    var questionIndex = questionEntityList[i].QuestionIndex;
                    entities.Add(new QuestionTableEntity()
                    {
                        RowNumber = questionCells[questionIndex][j].Item1,
                        ColumnNumber = questionCells[questionIndex][j].Item2,
                        Value = questionEntityList[i].QuestionStringList[j],
                        IsAnswerCell = answerCells.Any(group =>
                            group.Any(cell => cell.Item1 == questionCells[questionIndex][j].Item1 && cell.Item2 == questionCells[questionIndex][j].Item2)),
                    });
                }
            }
            return entities;
        }

    }
}
