using MathDomain.Entity;
using MathGenerator.DataTransferObject;
using MysqlLibrary.Repository;


namespace MathGenerator.Services
{
    public class Multiplication
    {
        private MysqlQuestionRepository questionRepository;
        private MysqlMathRepository mathRepository;
        private QuestionEntityMapper questionEntityMapper;
        private SampleGetter sampleGetter;
        public Multiplication(MysqlQuestionRepository questionRepository, MysqlMathRepository mathRepository, QuestionEntityMapper questionEntityMapper, SampleGetter sampleGetter)
        {
            this.questionRepository = questionRepository;
            this.mathRepository = mathRepository;
            this.questionEntityMapper = questionEntityMapper;
            this.sampleGetter = sampleGetter;
        }
        public async Task InsertQuestionEntity()
        {
            var MathEntities = await mathRepository.GetByClassAsync("Multiplication");
            var QuestionEntities = new List<mQuestionEntity>();

            foreach (var MathEntity in MathEntities)
            {
                var QuestionId = 0;
                var Values = new List<List<QuestionObject>>();
                for (int i = MathEntity.MinValueI; i <= MathEntity.MaxValueI; i++)
                {
                    var MaxValueJ = MathEntity.HasFixedValue ? MathEntity.FixedValue : MathEntity.MaxValueJ;
                    var MinValueJ = MathEntity.HasFixedValue ? MathEntity.FixedValue : MathEntity.MinValueJ;
                    for (int j = MinValueJ; j <= MaxValueJ; j++)
                    {
                        if (MathEntity.IsOutOfRange(i * j)) continue;
                        //if (MathEntity.HasCarry.HasValue && HasCarry(i, j) != MathEntity.HasCarry) continue;
                        Values.AddRange(GetAdditionPair(i, j, MathEntity.IsOrdered));
                    }
                }
                var Samples = sampleGetter.GetSamplePairs(Values);
                QuestionEntities.AddRange(questionEntityMapper.MapToQuestionEntity(MathEntity.PageId, Samples));
            }
            await questionRepository.DeleteByPagesAsync(MathEntities.Select(p => p.PageId).ToList());
            await questionRepository.InsertEntityAsync(QuestionEntities);
        }
        private List<List<QuestionObject>> GetAdditionPair(int i, int j, bool IsOrdered)
        {
            var Values = new List<List<QuestionObject>>();
            Values.Add(new List<QuestionObject>()
                {
                    new QuestionObject() {Value = $"{j}", Context = "×",ValueId = 1, HasNext = true, IsQuestion = true},
                    new QuestionObject() {Value = $"{i}", Context = "=",ValueId = 2, HasNext = true,  IsQuestion = true},
                    new QuestionObject() {Value = $"{i*j}", Context = "",ValueId = 3, HasNext = false,  IsQuestion = false},
                });
            if (!IsOrdered)
            {
                Values.Add(new List<QuestionObject>()
                {
                    new QuestionObject() {Value = $"{i}", Context = "×",ValueId = 1, HasNext = true,  IsQuestion = true},
                    new QuestionObject() {Value = $"{j}", Context = "=",ValueId = 2, HasNext = true,  IsQuestion = true},
                    new QuestionObject() {Value = $"{i*j}", Context = "",ValueId = 3, HasNext = false,  IsQuestion = false},
                });
            }
            return Values;
        }
    }
}
