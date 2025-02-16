using Google.Apis.Slides.v1.Data;
using GoogleSlideLibrary.Repository.Page;
using TempriDomain.Entity;

namespace GoogleSlideLibrary.Repository.Print
{
    public class Print100007
    {
        private readonly string presentationId = "16Nb-OMgA05mNjhxqeeMFpkHiDz_N4una9TTXZaj4O-s";
        private readonly int printId = 100007;
        private readonly string printName = "ひとけたのたしざん(くりあがりなし)";
        private readonly int pagesCount = 20;
        private readonly int score = 10;
        private PrintPage100007 page100007;
        public Print100007(PrintPage100007 page100007)
        {
            this.page100007 = page100007;
        }

        public async Task<PrintEntity> GetPrintAsync()
        {
            var printEntity = new PrintEntity
            {
                PresentationId = presentationId,
                PrintId = printId,
                PrintName = printName,
                PagesCount = pagesCount, // 総ページ数
                Score = score,
                PageEntityList = await page100007.SetPageAsync(presentationId,pagesCount)
            };
            return printEntity;
        }
    }
}