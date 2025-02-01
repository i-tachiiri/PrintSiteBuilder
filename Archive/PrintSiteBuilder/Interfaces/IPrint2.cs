using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder.Models.General;
using PrintSiteBuilder.Models.Print;

namespace PrintSiteBuilder.Interfaces
{
    public interface IPrint2
    {
        int PagesCount { get; }
        string PrintId { get; }
        string PrintName { get; }
        string Uuid { get; }
        Dictionary<string, string> PrintId_CategoryName { get;}
        List<List<List<string>>> GetQuestionLists();
        string PrintSlideId { get; }
        string CoverSlideId { get; }
        string AmazonSlideId { get; }
        PathConfig path {  get; }
        Task InitializeAsync(string type);
        int TemplateNumber { get; }
        string FnSku { get; }
        string Asin { get; }
        IPrintType PrintType { get; }
        string Description { get; }
        string Keywords { get; }
        string Sku { get; }
    }
}
