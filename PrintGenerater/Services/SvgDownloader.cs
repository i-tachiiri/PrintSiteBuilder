

using GoogleDriveLibrary.Services;
using ConsoleLibrary.Repository;
using GoogleSlideLibrary.Services;
using TempriDomain.Config;
using TempriDomain.ValueObject;
using TempriDomain.Entity;
using Google.Apis.Slides.v1.Data;
using PrintGenerater.Factories;
using TempriDomain.Interfaces;
namespace PrintGenerater.Services
{
    public class SvgDownloader
    {
        private AuthorityService authorityService;
        private ExportService exportService;
        private ConsoleRepository consoleRepository;
        private FolderPathValue folderPathValue;
        private readonly PrintFactory printFactory;

        public SvgDownloader(PrintFactory printFactory,AuthorityService authorityService,ConsoleRepository consoleRepository, ExportService exportService, FolderPathValue folderPathValue)
        {
            this.printFactory = printFactory;
            this.authorityService = authorityService;
            this.consoleRepository = consoleRepository;
            this.exportService = exportService;
            this.folderPathValue = folderPathValue;
        }
        public async Task ExportSvgs(IPrintEntity iPrint)
        {
            await authorityService.PermitReadToPublic(iPrint.PresentationId);
            foreach (var page in iPrint.Pages)  //各PrintのConfigに対して
            {
                consoleRepository.LoopLog($"Exporting SVGs.");

                var DownloadUrl = $"https://docs.google.com/presentation/d/{iPrint.PresentationId}/export/svg?pageid={page.PageObjectId}";
                var printType = page.IsAnswerPage ? "a" : "q";
                var ExportPath = Path.Combine(folderPathValue.SvgDir, $@"{iPrint.PrintId}-{page.PageNumber.ToString("D3")}-{printType}.svg");  
                await exportService.ExportImage(DownloadUrl, ExportPath);
            }
            authorityService.DenyPublicAccess(iPrint.PresentationId);
        }
    }
}
