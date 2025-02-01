

using MathDomain.Entity;
using MysqlLibrary.Repository;
using MysqlLibrary.Repository.Print;
using System.Text;

namespace MathGenerator.Services
{
    public class QuestionSummarizer
    {
        private MysqlQuestionRepository questionRepository;
        private MysqlSQuestionRepository sQuestionRepository;
        public QuestionSummarizer(MysqlQuestionRepository questionRepository, MysqlSQuestionRepository sQuestionRepository)
        {
            this.questionRepository = questionRepository;
            this.sQuestionRepository = sQuestionRepository;
        }
        public async Task SummarizeQuestion()
        {
            var QuestionList = await questionRepository.GetAllAsync();
            var PageGroups = QuestionList.GroupBy(x => x.PageId);
            var pageIdsToDelete = PageGroups.Select(g => g.Key).ToList();
            var SummaryEntities = new List<sQuestionEntity>();
            foreach (var pageGroup in PageGroups)
            {
                var QuestionGroups = pageGroup.GroupBy(x => x.QuestionId);
                foreach (var questionGroup in QuestionGroups)
                {
                    var summary = new sQuestionEntity
                    {
                        PageId = questionGroup.First().PageId,
                        QuestionId = questionGroup.First().QuestionId,
                        Question = ConcatQuestionString(questionGroup, true, false),
                        Answer = ConcatQuestionString(questionGroup, false, true)
                    };

                    SummaryEntities.Add(summary);
                }
            }

            await sQuestionRepository.DeleteByPagesAsync(pageIdsToDelete);
            await sQuestionRepository.InsertEntityAsync(SummaryEntities);
        }
        private string ConcatQuestionString(IGrouping<int,mQuestionEntity> group, bool IsQuestionOnly,bool IsAnswerOnly)
        {
            var sb = new StringBuilder();

            foreach (var item in group)
            {
                if (IsQuestionOnly && !item.IsQuestion) continue;
                if (IsAnswerOnly && item.IsQuestion) continue;

                sb.Append(item.Value);
                sb.Append(item.Context);
            }

            return sb.ToString();
        }
    }
}
