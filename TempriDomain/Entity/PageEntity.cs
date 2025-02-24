using TempriDomain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using TempriDomain.Config;

namespace TempriDomain.Entity
{
    public class PageEntity : IPageEntity
    {
        public string PageObjectId { get; private set; }
        public int PageNumber { get; private set; }
        public int PageIndex { get; private set; }
        public bool IsAnswerPage { get; private set; }
        public PrintEntity PrintEntity { get; private set; }

        public PageEntity(PrintEntity printEntity, string pageObjectId, int pageNumber, int pageIndex, bool isAnswerPage)
        {
            PrintEntity = printEntity;
            PageObjectId = pageObjectId;
            PageNumber = pageNumber;
            PageIndex = pageIndex;
            IsAnswerPage = isAnswerPage;
        }

        public string GetFileName()
        {
            var printType = IsAnswerPage ? "a" : "q";
            return $"{PrintEntity.PrintId}-{PageNumber}-{printType}";
        }

        public string GetFileNameWithExtension(string extension)
        {
            var printType = IsAnswerPage ? "a" : "q";
            return $"{PrintEntity.PrintId}-{PageNumber}-{printType}.{extension}";
        }
        public string GetFilePathWithExtension(string folder, string extension)
        {
            var printType = IsAnswerPage ? "a" : "q";
            return $"{PrintEntity.PrintId}-{PageNumber}-{printType}.{extension}";
        }
        public string GetUrlWithExtention(string folder,string extension)
        {
            return $"{TempriConstants.BaseUrl}/print/{PrintEntity.PrintId}/{folder}/{GetFileNameWithExtension(extension)}";
        }

        public async Task<List<IPageEntity>> SetPageAsync(PrintEntity printEntity, string presentationId, int pageCount, IPageService pageService)
        {
            var entities = new List<IPageEntity>();

            for (var i = 0; i < pageCount; i++)
            {
                var pageObjectId = await pageService.GetPageObjectIdByIndex(presentationId, i);
                entities.Add(new PageEntity(printEntity, pageObjectId, i / 2 + 1, i, i % 2 == 1));
            }

            return entities;
        }

    }
}
