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
