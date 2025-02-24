using TempriDomain.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TempriDomain.Interfaces
{
    public interface IPageEntity
    {
        Task<List<IPageEntity>> SetPageAsync(PrintEntity printEntity, string presentationId, int pageCount,IPageService pageService);
        string PageObjectId { get; }
        int PageNumber { get; }  // Page number
        int PageIndex { get; }   // Google Slides index
        bool IsAnswerPage { get; }
        PrintEntity PrintEntity { get; }
        string GetFileNameWithExtension(string extension);
        string GetFilePathWithExtension(string folder, string extension);
        string GetUrlWithExtention(string folder, string extension);
        string GetFileName();
    }
}
