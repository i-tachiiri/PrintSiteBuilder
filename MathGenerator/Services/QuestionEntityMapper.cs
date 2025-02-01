using MathDomain.Entity;
using MathGenerator.DataTransferObject;

namespace MathGenerator.Services
{
    public class QuestionEntityMapper
    {
        public List<mQuestionEntity> MapToQuestionEntity(string PageId, List<List<QuestionObject>> Values)
        {
            var QuestionEntities = new List<mQuestionEntity>();
            var QuestionId = 0;
            for (var i = 0; i < Values.Count; i++)
            {
                for (var j = 0; j < Values[i].Count; j++)
                {
                    QuestionEntities.Add(new mQuestionEntity()
                    {
                        Id = 0,
                        PageId = PageId,
                        QuestionId = i,
                        ValueId = j,
                        Value = Values[i][j].Value.ToString(),
                        Context = Values[i][j].Context,
                        HasNext = j < Values[i].Count - 1,
                        IsQuestion = Values[i][j].IsQuestion
                    });
                }
            }
            return QuestionEntities;
        }
    }
}
