﻿using GoogleSlideLibrary.Services;
using TempriDomain.Entity;

namespace GoogleSlideLibrary.Repository.Page
{
    public class Page100007
    {
        private PageService pageService;

        public Page100007(PageService pageService)
        {
            this.pageService = pageService;
        }

        public async Task<List<PageEntity>> SetPageAsync(string presentationId,int pageCount)
        {
            var entities = new List<PageEntity>();

            //PageNumber is 1,1,2,2...(The type int discards the fractional part)
            //PageIndex is 0,1,2,3...
            //AnswerPage has an odd PageIndex
            for(var i=0;i< pageCount; i++)
            {
                entities.Add(new PageEntity()
                {
                    PageObjectId = await pageService.GetPageObjectIdByIndex(presentationId,i),
                    PageNumber = i/2 + 1,
                    PageIndex = i,
                    IsAnswerPage = i%2 == 1,
                });
            }

            return entities;
        }
    }
}
