using Google.Apis.Services;
using Google.Apis.Slides.v1;
using Google.Apis.Slides.v1.Data;
using Newtonsoft.Json.Linq;
using PrintSiteBuilder.Interfaces;
using PrintSiteBuilder.Models;
using PrintSiteBuilder.Models.Print;
using PrintSiteBuilder.Utilities;
using Svg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using PrintSiteBuilder.GoogleService.Drive;


namespace PrintSiteBuilder.GoogleService.Slide
{
    public class AmazonSlidePages
    {
        private ExportSlide exportSlide = new ExportSlide();
        public GoogleApi googleApi;
        public string PresentationID;
        public SlidesService slideService;
        public Google.Apis.Slides.v1.Data.Presentation presentation;
        public AmazonSlidePages(string ID)
        {
            googleApi = new GoogleApi();
            PresentationID = ID;
            slideService = googleApi.GetSlideService();
            presentation = slideService.Presentations.Get(PresentationID).Execute();
        }
        public async Task UpdateAndExportAmazonSlide(IPrint2 iPrint)
        {
            var requests = new List<Request>();
            var CoverUrl = Directory.GetFiles(iPrint.path.PrintPngDir).FirstOrDefault();
            requests.AddRange(await GetCoverImageReplaceRequest(CoverUrl));
            batchUpdate(requests);
            var exportSlide = new ExportSlide();
            await exportSlide.ExportAmazonImages(iPrint, "png");
        }
        public async Task<List<Request>> GetCoverImageReplaceRequest(string PngPath)
        {
            var drive = new GoogleDrive();
            var PngUrl = await drive.UploadTempImage(PngPath);
            var requests = new List<Request>();
            var CoverImageId = presentation.Slides[0].PageElements.Where(element => element.Image != null).OrderBy(element => element.Transform.TranslateY).ToList()[0].ObjectId;
            requests.Add(new Request()
            {
                ReplaceImage = new ReplaceImageRequest()
                {
                    ImageObjectId = CoverImageId,
                    ImageReplaceMethod = "CENTER_INSIDE",
                    Url = PngUrl
                }
            });
            return requests;
        }
        public void batchUpdate(List<Request> requests)
        {
            try
            {
                var batchUpdateRequest = new BatchUpdatePresentationRequest { Requests = requests };
                var result = slideService.Presentations.BatchUpdate(batchUpdateRequest, PresentationID).Execute();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Utilities.SlidePages.BatchUpdate]{ex.Message}");
            }
        }
    }
}
