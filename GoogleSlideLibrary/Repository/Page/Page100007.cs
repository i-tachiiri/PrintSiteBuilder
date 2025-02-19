using GoogleSlideLibrary.Repository.PageQuestion;
using GoogleSlideLibrary.Services;
using TempriDomain.Entity;

namespace GoogleSlideLibrary.Repository.Page
{
    public class Page100007
    {
        private Question100007 pageQuestion100007;
        private PageService pageService;

        public Page100007(Question100007 pageQuestion100007, PageService pageService)
        {
            this.pageQuestion100007 = pageQuestion100007;
            this.pageService = pageService;
        }

        public async Task<List<PrintPageEntity>> SetPageAsync(string presentationId,int pageCount)
        {
            var entities = new List<PrintPageEntity>();

            //PageNumber is 1,1,2,2...(The type int discards the fractional part)
            //PageIndex is 0,1,2,3...
            //AnswerPage has an odd PageIndex
            for(var i=0;i< pageCount; i++)
            {
                entities.Add(new PrintPageEntity()
                {
                    PageObjectId = await pageService.GetPageObjectIdByIndex(presentationId,i),
                    PageNumber = i/2 + 1,
                    PageIndex = i,
                    IsAnswerPage = i%2 == 1,
                });
            }

            //Set parent entity ⇔ child entity 
            var tasks = entities.Select(async entity =>
            {
                entity.QuestionList = await pageQuestion100007.SetPageQuestionEntity(entity.PageNumber);
                foreach (var pageQuestion in entity.QuestionList)
                {
                    pageQuestion.PageEntity = entity;
                }
            });
            await Task.WhenAll(tasks);

            return entities;
        }
    }
}
