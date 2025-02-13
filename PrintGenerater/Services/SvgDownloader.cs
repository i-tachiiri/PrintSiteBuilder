

using GoogleDriveLibrary.Services;
using ConsoleLibrary.Repository;
using GoogleSlideLibrary.Services;
using TempriDomain.Config;
using TempriDomain.ValueObject;
using TempriDomain.Entity;
using Google.Apis.Slides.v1.Data;
namespace PrintGenerater.Services
{
    public class SvgDownloader
    {
        private AuthorityService authorityService;
        private ExportService exportService;
        private ConsoleRepository consoleRepository;
        private FolderPathValue folderPathValue;
        public SvgDownloader(AuthorityService authorityService,ConsoleRepository consoleRepository, ExportService exportService, FolderPathValue folderPathValue)
        {
            this.authorityService = authorityService;
            this.consoleRepository = consoleRepository;
            this.exportService = exportService;
            this.folderPathValue = folderPathValue;
        }
        public async Task ExportPrintImages(string SlideId,string Extention)
        {
            //var slidePages = new SlidePages(SlideId);
            //var PrintConfigs = iPrint.PrintType.GetPrintConfigs();
            var xxx = new PrintEntity();  //仮
            var yyy = new PageEntity();　//仮
            Presentation presentation = 
            authorityService.PermitReadToPublic(SlideId);
            foreach (var printConfig in PrintConfigs)  //各PrintのConfigに対して
            {
                consoleRepository.LoopLog($"Export {Extention}");

                var DownloadUrl = $"https://docs.google.com/presentation/d/{SlideId}/export/{Extention}?pageid={slidePages.presentation.Slides[printConfig.headerConfig.PageIndex].ObjectId}";
                var printType = yyy.PrintType == "問題" ? "q" : "a";
                var ExportPath = Path.Combine(folderPathValue.SvgDir, $@"{xxx.PrintId}-{yyy.PrintNumber.ToString("D3")}-{printType}.{Extention}");  
                await exportService.ExportImage(DownloadUrl, ExportPath);
            }
            authorityService.DenyPublicAccess(SlideId);
        }
    }
}
