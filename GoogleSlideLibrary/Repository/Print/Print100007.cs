using Google.Apis.Slides.v1.Data;
using GoogleSlideLibrary.Repository.Page;
using TempriDomain.Entity;

namespace GoogleSlideLibrary.Repository.Print
{
    public class Print100007
    {
        private List<int> answerIndex = new List<int> { 4, 10 };
        private Page100007 page100007 = new Page100007();

        public async Task<PrintEntity> GetPrintAsync()
        {
            var presentationId = "16Nb-OMgA05mNjhxqeeMFpkHiDz_N4una9TTXZaj4O-s";
            var printEntity = new PrintEntity
            {
                PresentationId = presentationId,
                PrintId = "100007",
                PrintName = "ひとけたのたしざん(くりあがりなし)",
                PagesCount = 30, // 総ページ数
                Score = 10,
                pageEntityList = await page100007.SetPageAsync(presentationId) // 非同期処理の待機
            };

            return printEntity;
        }
    }

}
