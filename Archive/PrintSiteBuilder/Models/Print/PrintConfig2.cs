using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PrintSiteBuilder.Models.Print
{
    public class PrintConfig2
    {
        public HeaderConfig headerConfig { get;  }
        public List<CellConfig> cellConfigs { get;  }
        public string PrintDir = $@"C:\drive\work\www\item\print";
        public string PrintId;
        public string BaseDir;
        public string QrDir;
        public string SvgDir;
        public string UuidDir;
        public string WebpDir;
        public string ThumbDir;
        public PrintConfig2(HeaderConfig _headerConfig, List<CellConfig> _cellConfigs,string _printId)
        {
            headerConfig = _headerConfig;
            cellConfigs = _cellConfigs;
            PrintId = _printId;
            BaseDir = $@"{PrintDir}\{_printId}";
            QrDir = $@"{BaseDir}\qr";
            SvgDir = $@"{BaseDir}\svg";
            UuidDir = $@"{BaseDir}\uuid";
            WebpDir = $@"{BaseDir}\webp";
            ThumbDir = $@"{BaseDir}\thumb";
        }
    }
}
