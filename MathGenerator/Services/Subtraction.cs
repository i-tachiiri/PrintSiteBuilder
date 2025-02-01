
using MathDomain.Entity;
using MathGenerator.DataTransferObject;
using MysqlLibrary.Repository;
using System.Collections.Generic;

namespace MathGenerator.Services
{
    public class Subtraction
    {
        private MysqlQuestionRepository questionRepository;
        private MysqlMathRepository mathRepository;
        private QuestionEntityMapper questionEntityMapper;
        private SampleGetter sampleGetter;
        public Subtraction(MysqlQuestionRepository questionRepository, MysqlMathRepository mathRepository, QuestionEntityMapper questionEntityMapper, SampleGetter sampleGetter)
        {
            this.questionRepository = questionRepository;
            this.mathRepository = mathRepository;
            this.questionEntityMapper = questionEntityMapper;
            this.sampleGetter = sampleGetter;
        }
        public async Task InsertQuestionEntity()
        {
            var MathEntities = await mathRepository.GetByClassAsync("Subtraction");
            var QuestionEntities = new List<mQuestionEntity>();
            var random = new Random(0);

            foreach (var MathEntity in MathEntities)
            {
                var QuestionId = 0;
                var QuestionCount = 100;
                var Values = new List<List<QuestionObject>>();
                for (int i = MathEntity.MinValueI; i <= MathEntity.MaxValueI; i++)
                {
                    var MaxValueJ = MathEntity.HasFixedValue ? MathEntity.FixedValue : MathEntity.MaxValueJ;
                    var MinValueJ = MathEntity.HasFixedValue ? MathEntity.FixedValue : MathEntity.MinValueJ;
                    for (int j = MinValueJ; j <= MaxValueJ; j++)
                    {
                        if (MathEntity.IsOutOfRange(i - j)) continue;
                        if (MathEntity.HasBorrow.HasValue && HasBorrow(i, j) != MathEntity.HasBorrow) continue;
                        Values.AddRange(GetSubstractionPair(i, j, MathEntity.IsOrdered));
                    }
                }
                var Samples = sampleGetter.GetSamplePairs(Values);
                QuestionEntities.AddRange(questionEntityMapper.MapToQuestionEntity(MathEntity.PageId, Samples));
            }
            await questionRepository.DeleteByPagesAsync(MathEntities.Select(p => p.PageId).ToList());
            await questionRepository.InsertEntityAsync(QuestionEntities);
        }
        private List<List<QuestionObject>> GetSubstractionPair(int i, int j, bool IsOrdered)
        {
            var Values = new List<List<QuestionObject>>();
            Values.Add(new List<QuestionObject>()
                {
                    new QuestionObject() {Value = $"{i}", Context = "-",ValueId = 1, HasNext = true, IsQuestion = true},
                    new QuestionObject() {Value = $"{j}", Context = "=",ValueId = 2, HasNext = true,  IsQuestion = true},
                    new QuestionObject() {Value = $"{i-j}", Context = "",ValueId = 3, HasNext = false,  IsQuestion = false},
                });
            if (!IsOrdered)
            {
                Values.Add(new List<QuestionObject>()
                {
                    new QuestionObject() {Value = $"{j}", Context = "-",ValueId = 1, HasNext = true,  IsQuestion = true},
                    new QuestionObject() {Value = $"{i}", Context = "=",ValueId = 2, HasNext = true,  IsQuestion = true},
                    new QuestionObject() {Value = $"{i-j}", Context = "",ValueId = 3, HasNext = false,  IsQuestion = false},
                });
            }
            return Values;
        }
        private bool HasBorrow(int i, int y)
        {
            while (i > 0 || y > 0)
            {
                int digitI = i % 10;
                int digitY = y % 10;
                if (digitI < digitY) return true;
                i /= 10;
                y /= 10;
            }
            return false;
        }

    }
    /*public async Task CreatePairWithFixedValue()
    {
        var MathEntities = await mathRepository.GetByFuncAndClassAsync("Subtraction", "PairWithFixedValue");
        var QuestionEntities = new List<QuestionEntity>();
        foreach (var MathEntity in MathEntities)
        {
            var QuestionId = 0;
            await questionRepository.DeleteByPageAsync(MathEntity.PageId);
            for (int i = MathEntity.MinValueI; i <= MathEntity.MaxValueI; i++)
            {
                if (i - MathEntity.FixedValue > MathEntity.MaxAnswer || i - MathEntity.FixedValue < MathEntity.MinAnswer) continue;
                QuestionId++;
                var Values = new List<(int, string, bool)>()
                {
                    ( i, "-" ,true),
                    ( MathEntity.FixedValue, "=" ,true),
                    (i - MathEntity.FixedValue, "", false)
                };
                QuestionEntities.AddRange(questionEntityMapper.MapToQuestionEntity(MathEntity.PageId, QuestionId, Values));
            }

        }
        await questionRepository.InsertEntityAsync(QuestionEntities);
    }
    public async Task CreateOrderedPair()
    {
        var MathEntities = await mathRepository.GetByFuncAndClassAsync("Subtraction", "OrderedPair");
        var QuestionEntities = new List<QuestionEntity>();
        foreach (var MathEntity in MathEntities)
        {
            var QuestionId = 0;
            await questionRepository.DeleteByPageAsync(MathEntity.PageId);
            for (int i = MathEntity.MinValueI; i <= MathEntity.MaxValueI; i++)
            {
                for (int j = MathEntity.MinValueJ; j <= MathEntity.MaxValueJ; j++)
                {
                    if (i - j > MathEntity.MaxAnswer || i - j < MathEntity.MinAnswer) continue;
                    QuestionId++;
                    var Values = new List<(int, string, bool)>()
                    {
                        ( i, "-" ,true),
                        ( j, "=" ,true),
                        (i - j, "", false)
                    };
                    QuestionEntities.AddRange(questionEntityMapper.MapToQuestionEntity(MathEntity.PageId, QuestionId, Values));
                }
            }
        }
        await questionRepository.InsertEntityAsync(QuestionEntities);
    }*/
}

