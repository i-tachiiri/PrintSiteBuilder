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
