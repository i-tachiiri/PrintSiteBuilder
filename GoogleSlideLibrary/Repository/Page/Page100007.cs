using TempriDomain.Entity;

namespace GoogleSlideLibrary.Repository.Page
{
    public class Page100007
    {
        public async Task<List<PageEntity>> SetPageAsync(string presentationId)
        {
            var slides = await _googleSlidesService.GetSlidesAsync(presentationId);
            var pages = new List<PageEntity>();

            foreach (var slide in slides)
            {
                // テーブルデータを取得
                var tables = await _googleSlidesTableRepository.GetTablesAsync(slide.Id);
                pages.Add(new PageEntity
                {
                    PrintId = presentationId,
                    PrintType = "Default",
                    PrintNumber = slide.Index + 1,
                    PageIndex = slide.Index,
                    printEntity = null,  // ナビゲーションプロパティ
                    printTableEntity = tables  // テーブルデータをセット
                });
            }

            return pages;
        }
    }

}
