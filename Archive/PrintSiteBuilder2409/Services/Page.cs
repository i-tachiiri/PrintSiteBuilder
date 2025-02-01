using PrintSiteBuilder2409.IExternal;
using PrintSiteBuilder2409.IRepository;
using PrintSiteBuilder2409.IServices;
using PrintSiteBuilder2409.Config;
using PrintSiteBuilder2409.IValidation;
using PrintSiteBuilder2409.External;
using PrintSiteBuilder2409.Factory;
using PrintSiteBuilder2409.IFactory;


namespace PrintSiteBuilder2409.Services
{
    public class Page : IPage
    {
        private readonly IExSlideService exSlideService;
        private readonly ISlideRepository slideRepository;
        private readonly IPageRepository pageRepository;
        private readonly IPageValidation pageValidation;
        private readonly IEntityFactory entityFactory;
        private readonly IExplorer explorer;
        private string TestFolder;
        private string PrintFolder;
        private string PrintId;
        public Page(IExSlideService exSlideService, ISlideRepository slideMasterRepository, IPageValidation pageValidation, IEntityFactory entityFactory)
        {
            this.exSlideService = exSlideService;
            this.entityFactory = entityFactory;
            this.PrintId = PrintId;
            this.TestFolder = $@"{Constants.Explorer.RootFolder}\test\{PrintId}";
            this.PrintFolder = $@"{Constants.Explorer.RootFolder}\print\{PrintId}";

        }
        public void CreateDirectory(string PrintId)
        {
            foreach(var dir in Constants.Explorer.TestFolderNames) 
            { 
                Directory.CreateDirectory($@"{TestFolder}\{dir}"); 
            }
        }
        public async Task SetRecord(string PrintId)
        {
            var entity = await slideRepository.GetByIdAsync(PrintId);
            for(var PageNumber=1;PageNumber<=entity.PageCount;PageNumber++)
            {
                if (!await pageValidation.HasRecordByKeys(PrintId,PageNumber))
                {
                    var PageEntity = entityFactory.CreatePageMasterInstance();
                    PageEntity.PrintId = PrintId;   
                    PageEntity.PageNumber = PageNumber;
                    PageEntity.PrintSerial = $"{PrintId}-{ PageNumber.ToString("D3")}";
                    PageEntity.IsLogoCreated = false;
                    PageEntity.IsQrCreated = false;
                    PageEntity.IsPdfCreated = false;
                    PageEntity.IsPdf4Created = false;
                    PageEntity.IsWebpCreated = false;
                    PageEntity.IsUuidCreated = false;
                    pageRepository.SetRecordAsync(PageEntity);
                }
            }
        }
        public async void ExportSvg(string PrintId)
        {
            var entity = await slideRepository.GetByIdAsync(PrintId);
            var presentation = exSlideService.GetPresentation(entity.PrntSlideId);            
            await exSlideService.ExportImages(presentation, entity.Attribute,"svg");
        }
        public async void ExportQr(string PrintId)
        {
            var slideEntity = await slideRepository.GetByIdAsync(PrintId);
            var entities = await pageRepository.GetByIdAsync(PrintId);
            for(var i=1;i<entities.Count();i++)
            {
                var entity = entities[0];
                var QrFolder = $@"{Constants.Explorer.RootFolder}\test\{PrintId}\qr";
                var Url = $@"{Constants.Web.RootUrl}/print/{PrintId}/php/{PrintId}-{entity.PageNumber.ToString("D3")}.php?uuid={Uuid}";
                var outputPath = $@"{QrFolder}\{itemConfig.ItemKey}.svg";

                explorer.CreateQr(PrintId, entity.PageNumber, QrFolder, slideEntity.Uuid);
            }
            
        }
    }
}
