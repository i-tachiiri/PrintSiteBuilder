using PrintSiteBuilder2409.Entities;
using PrintSiteBuilder2409.IRepository;
using PrintSiteBuilder2409.IServices;

namespace PrintSiteBuilder2409.Services
{
    public class P100004 : IExercise
    {

        IExerciseRepository exerciseMasterRepository;
        IPrintRepository printMasterRepository;
        private readonly string printId;
        public P100004(IExerciseRepository exerciseMasterRepository, IPrintRepository printMasterRepository, string printId)
        {
            this.exerciseMasterRepository = exerciseMasterRepository;
            this.printMasterRepository = printMasterRepository;
            this.printId = printId;
        }
        public async Task RenewExerciseMaster()
        {
            Func<ExerciseMaster, bool> condition = master => master.PrintId == printId;
            var data = await CreateExerciseMasterList();
            await exerciseMasterRepository.RenewMasterAsync(condition, data);
        }
        private async Task<IEnumerable<ExerciseMaster>> CreateExerciseMasterList()
        {
            var ExerciseMasterList = new List<ExerciseMaster>();
            Random random = new Random();
            var PrintMaster = await printMasterRepository.GetByIdAsync(printId);
            var PagesCount = PrintMaster.PagesCount;

            for (var page = 0; page < PagesCount; page++)
            {
                List<int> QuestionRowNumbers = Enumerable.Range(1, 10).OrderBy(x => random.Next()).ToList();
                List<int> QuestionColumnNumbers = Enumerable.Range(1, 10).OrderBy(x => random.Next()).ToList();
                for (var row = 1; row < 11; row++)
                {
                    for (var column = 1; column < 11; column++)
                    {
                        var answer = QuestionRowNumbers[column - 1] + QuestionColumnNumbers[row - 1];
                        ExerciseMasterList.AddRange(CreateAnswerExerciseMaster(QuestionRowNumbers, QuestionColumnNumbers, page, row, column, answer.ToString()));
                    }
                }
            }
            return ExerciseMasterList;
        }
        private List<ExerciseMaster> CreateAnswerExerciseMaster(List<int> QuestionRowNumbers, List<int> QuestionColumnNumbers, int page, int row, int column, string answer)
        {
            return new List<ExerciseMaster>()
            {
                new ExerciseMaster()
                {
                    PrintId = printId,
                    PageId = page,
                    ExerciseId = (row - 1) * 10 + column-1,
                    CellNumber = 1,
                    ValueType = "Q",
                    ColumnNumber = 0,
                    RowNumber = row,
                    Value = QuestionColumnNumbers[row-1].ToString()
                },
                new ExerciseMaster()
                {
                    PrintId = printId,
                    PageId = page,
                    ExerciseId = (row - 1) * 10 + column-1,
                    CellNumber = 2,
                    ValueType = "Q",
                    ColumnNumber = 11,
                    RowNumber = row,
                    Value = QuestionColumnNumbers[row-1].ToString()
                },
                new ExerciseMaster()
                {
                    PrintId = printId,
                    PageId = page,
                    ExerciseId = (row - 1) * 10 + column - 1,
                    CellNumber = 3,
                    ValueType = "Q",
                    ColumnNumber = column,
                    RowNumber = 0,
                    Value = QuestionRowNumbers[column-1].ToString()
                },
                new ExerciseMaster()
                {
                    PrintId = printId,
                    PageId = page,
                    ExerciseId = (row - 1) * 10 + column-1,
                    CellNumber = 4,
                    ValueType = "A",
                    ColumnNumber = column,
                    RowNumber = row,
                    Value = answer
                }
            };
        }
    }
}
