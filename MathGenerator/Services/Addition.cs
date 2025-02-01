
using MathDomain.Entity;
using MathGenerator.DataTransferObject;
using MysqlLibrary.Repository;


namespace MathGenerator.Services
{
    public class Addition
    {
        private MysqlQuestionRepository questionRepository;
        private MysqlMathRepository mathRepository;
        private QuestionEntityMapper questionEntityMapper;
        private SampleGetter sampleGetter;
        public Addition(MysqlQuestionRepository questionRepository, MysqlMathRepository mathRepository, QuestionEntityMapper questionEntityMapper, SampleGetter sampleGetter)
        {
            this.questionRepository = questionRepository;
            this.mathRepository = mathRepository;
            this.questionEntityMapper = questionEntityMapper;
            this.sampleGetter = sampleGetter;
        }
        public async Task InsertQuestionEntity()
        {
            var MathEntities = await mathRepository.GetByClassAsync("Addition");
            var QuestionEntities = new List<mQuestionEntity>();
            var random = new Random(0);

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
                        if (MathEntity.IsOutOfRange(i + j)) continue;
                        if (MathEntity.HasCarry.HasValue && HasCarry(i, j) != MathEntity.HasCarry) continue;
                        Values.AddRange(GetAdditionPair(i,j, MathEntity.IsOrdered));
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
                    new QuestionObject() {Value = $"{i}", Context = "+",ValueId = 1, HasNext = true, IsQuestion = true},
                    new QuestionObject() {Value = $"{j}", Context = "=",ValueId = 2, HasNext = true,  IsQuestion = true},
                    new QuestionObject() {Value = $"{i+j}", Context = "",ValueId = 3, HasNext = false,  IsQuestion = false},
                }); 
            if (IsOrdered)
            {
                Values.Add(new List<QuestionObject>() 
                {
                    new QuestionObject() {Value = $"{j}", Context = "+",ValueId = 1, HasNext = true,  IsQuestion = true},
                    new QuestionObject() {Value = $"{i}", Context = "=",ValueId = 2, HasNext = true,  IsQuestion = true},
                    new QuestionObject() {Value = $"{i+j}", Context = "",ValueId = 3, HasNext = false,  IsQuestion = false},
                });
            }
            return Values;
        }
        private bool HasCarry(int x, int y)
        {
            return (x % 10 + y % 10) >= 10;
        }
        /*
                public async Task InsertQuestionEntity(string FunctionName,bool HasFixedValue,bool IsOrdered,bool? HasCarry)
        {
            var MathEntities = await mathRepository.GetByFuncAndClassAsync("Addition", FunctionName);
            var QuestionEntities = new List<QuestionEntity>();
            var random = new Random(0);

            foreach (var MathEntity in MathEntities)
            {
                var QuestionId = 0;
                var QuestionCount = 100;                
                var min = new List<int>() { MathEntity.MinAnswer / 2, MathEntity.MinValueI, MathEntity.MinValueJ }.Max(); //最小値の最大値
                var max = new List<int>() { MathEntity.MaxAnswer / 2, MathEntity.MaxValueI, MathEntity.MaxValueJ }.Min(); //最小値の最大値
                var DecimationValue = (int)((max - min) * (max - min) / 1000) +1;
                for (int i = MathEntity.MinValueI; i <= MathEntity.MaxValueI; i++)
                {
                    var MaxValueJ = HasFixedValue ? MathEntity.FixedValue : MathEntity.MaxValueJ;
                    var MinValueJ = HasFixedValue ? MathEntity.FixedValue : MathEntity.MinValueJ;
                    for (int j = MinValueJ; j <= MaxValueJ; j += random.Next(1, DecimationValue))
                    {
                        if (MathEntity.IsOutOfRange(i + j)) continue;
                        if (HasCarry.HasValue && MathEntity.HasCarry(i, j) != HasCarry) continue;
                        QuestionId++;
                        var Values = new List<(int, string, bool)>() { (i, "+", true), (j, "=", true), (i + j, "", false) };
                        QuestionEntities.AddRange(questionEntityMapper.MapToQuestionEntity(MathEntity.PageId, QuestionId, Values));
                        if(!IsOrdered)
                        {
                            QuestionId++;
                            Values = new List<(int, string, bool)>(){( j, "+" ,true),( i, "=" ,true),(i + j, "", false), };
                            QuestionEntities.AddRange(questionEntityMapper.MapToQuestionEntity(MathEntity.PageId, QuestionId, Values));
                        }
                        if (QuestionId >= QuestionCount) break;
                    }
                    if (QuestionId >= QuestionCount) break;
                }
            }
            await questionRepository.DeleteByPagesAsync(MathEntities.Select(p => p.PageId).ToList());
            await questionRepository.InsertEntityAsync(QuestionEntities);
        }
         */
    }
}
