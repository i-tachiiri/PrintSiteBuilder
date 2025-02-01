using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder.Models.Print
{
    public class PrintConfig
    {
        public HeaderConfig headerConfig { get;  }
        public List<CellConfig> cellConfigs { get;  }
        public PrintConfig(HeaderConfig _headerConfig, List<CellConfig> _cellConfigs)
        {
            headerConfig = _headerConfig;
            cellConfigs = _cellConfigs;
        }
    }
}
