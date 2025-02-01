using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder.Models.Print;

namespace PrintSiteBuilder.Interfaces
{
    public interface IPrint
    {
        int PagesCount { get; }
        List<PrintConfig> GetPrintConfigs();
        List<HeaderConfig> GetHeaderConfigs();
        string PresentationId { get; }
    }
}
