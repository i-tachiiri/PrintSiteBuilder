using System.Collections.Generic;
using System.Threading.Tasks;
using TempriDomain.Entity;
using TempriDomain.Interfaces;
using GoogleSlideLibrary.Repository.Page;

namespace GoogleSlideLibrary.Repository.Print
{
    public class Print100007 : IPrint
    {
        // Properties with public getters (following PascalCase naming convention)
        public string PresentationId { get; } = "16Nb-OMgA05mNjhxqeeMFpkHiDz_N4una9TTXZaj4O-s";
        public int PrintId { get; } = 100007;
        public int PagesCount { get; } = 20;
        public string PrintName { get; } = "xxx";
        public int Score { get; } = 10;
        public List<IPage> Pages { get; }

        // Constructor injection for pages
        public Print100007(List<IPage> pages)
        {
            Pages = pages ?? new List<IPage>(); // Ensure Pages is initialized
        }

        public async Task<PrintEntity> SetPrintAsync()
        {
            var pageEntities = new List<PageEntity>();

            foreach (var page in Pages)
            {
                var pageEntity = await page.SetPageAsync(PresentationId, PagesCount);
                pageEntities.AddRange(pageEntity);
            }

            return new PrintEntity
            {
                PresentationId = PresentationId,
                PrintId = PrintId,
                PrintName = PrintName,
                PagesCount = PagesCount,
                Score = Score,
                PageEntityList = pageEntities
            };
        }
    }
}
