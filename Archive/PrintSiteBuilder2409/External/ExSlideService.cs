using FikaAmazonAPI.Services;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Slides.v1;
using Google.Apis.Slides.v1.Data;
using PrintSiteBuilder2409.Config;
using PrintSiteBuilder2409.Entities;
using PrintSiteBuilder2409.IExternal;


namespace PrintSiteBuilder2409.External
{
    public class ExSlideService
    {
        private readonly SlidesService service;
        private readonly IExDriveService exDriveService;
        private readonly IExplorer explorer;
        public ExSlideService(IExDriveService exDriveService, IExplorer explorer)
        {
            service = new GoogleApiClient().GetSlideService();
            this.exDriveService  = exDriveService;
            this.explorer = explorer;
        }
        public void batchUpdate(List<Request> requests,string PresentatonId)
        {
            try
            {
                var batchUpdateRequest = new BatchUpdatePresentationRequest { Requests = requests };
                var result = service.Presentations.BatchUpdate(batchUpdateRequest, PresentatonId).Execute();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Utilities.SlidePages.BatchUpdate]{ex.Message}");
            }
        }
        public Presentation GetPresentation(string PresentationId)
        {
            return service.Presentations.Get(PresentationId).Execute();
        }
        public Request GetDuplicateObjectRequest(string ObjectId)
        {
            return new Request
            {
                DuplicateObject = new DuplicateObjectRequest
                {
                    ObjectId = ObjectId
                }
            };
        }
        public Request GetDeleteObjectRequest(string ObjectId)
        {
            return new Request
            {
                DeleteObject = new DeleteObjectRequest
                {
                    ObjectId = ObjectId
                }
            };
        }
        public async Task ExportImages(Presentation presentation,string attribute,string Extention)
        {
            exDriveService.PermitReadToPublic(presentation.PresentationId);
            var Slides = presentation.Slides;
            for(var i=0;i< Slides.Count; i++)
            {
                var Url = $"https://docs.google.com/presentation/d/{presentation.PresentationId}/export/{Extention}?pageid={Slides[i].ObjectId}";
                var printType = i < Slides.Count / 2 ? "q" : "a";  
                var OutpuPath = $@"{Constants.Explorer.RootFolder}\{attribute}\{presentation.PresentationId}-{(i+1).ToString("D3")}-{printType}.{Extention}";
                await explorer.DownloadFromUrl(Url, OutpuPath);
            }
            exDriveService.DenyPublicAccess(presentation.PresentationId);
        }
    }
}
