using PrintSiteBuilder.Models.Print;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder.Interfaces
{
    public interface IPrintType
    {
        List<PrintConfig2> GetPrintConfigs();
        List<HeaderConfig> GetHeaderConfigs();
        int Score { get; }
        string SkuHeader { get; }
    }
}
