using PrintSiteBuilder.Interfaces;
using PrintSiteBuilder.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder.Models.Print
{
    public class PrintItemConfig
    {
        int PagesCount { get; }
        string PrintId { get; }
        string PrintName { get; }
        string Sku { get; }
        string Uuid { get; }
        Dictionary<string, string> PrintId_CategoryName { get; }
        string PrintSlideId { get; }
        string CoverSlideId { get; }
        string AmazonSlideId { get; }
        int TemplateNumber { get; }
        string Barcode { get; }
        IPrintType PrintType { get; }
    }
}
