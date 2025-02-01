using Google.Apis.Services;
using Google.Apis.Slides.v1;
using Google.Apis.Slides.v1.Data;
using Newtonsoft.Json.Linq;
using PrintSiteBuilder.Interfaces;
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
    public class CoverSlidePages
    {
        public GoogleApi googleApi;
        public string PresentationID;
        public SlidesService slideService;
        public Google.Apis.Slides.v1.Data.Presentation presentation;
        public CoverSlidePages(string ID)
        {
            googleApi = new GoogleApi();
            PresentationID = ID;
            slideService = googleApi.GetSlideService();
            presentation = slideService.Presentations.Get(PresentationID).Execute();
        }
        public async Task UpdateCoverSlide(IPrint2 iPrint)
        {
            var requests = new List<Request>();
            var CoverUrl = Directory.GetFiles(iPrint.path.PrintPngDir).FirstOrDefault();
            var BarcodeUrl = $@"{iPrint.path.PrintCoverDir}\code128.png";
            requests.AddRange(await GetCoverImageReplaceRequest(CoverUrl));
            requests.AddRange(await GetCoverBarcodeReplaceRequest(BarcodeUrl));
            requests.AddRange(await GetPrintTitleUpdateRequest(iPrint));
            batchUpdate(requests);
            var exportSlide = new ExportSlide();
            await exportSlide.ExportCoverPdf(presentation, iPrint);
        }
        public async Task<List<Request>> GetCoverImageReplaceRequest(string PngPath)
        {
            var drive = new GoogleDrive();
            var PngUrl = await drive.UploadTempImage(PngPath);
            var requests = new List<Request>();
            var CoverImageId = presentation.Slides[0].PageElements.Where(element => element.Image != null).OrderBy(element => element.Transform.TranslateY).ToList()[1].ObjectId;
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
        public async Task<List<Request>> GetCoverBarcodeReplaceRequest(string PngPath)
        {
            var drive = new GoogleDrive();
            var PngUrl = await drive.UploadTempImage(PngPath);
            var requests = new List<Request>();
            var CoverImageId = presentation.Slides[0].PageElements.Where(element => element.Image != null).OrderBy(element => element.Transform.TranslateY).ToList()[2].ObjectId;
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
        public async Task<List<Request>> GetPrintTitleUpdateRequest(IPrint2 iPrint)
        {
            var slide = presentation.Slides[0];
            var PrintTitleRect = slide.PageElements.Where(element => element.Shape != null && element.Shape.ShapeType == "RECTANGLE").OrderBy(element => element.Transform.TranslateY).FirstOrDefault();

            var deleteTextRequest = new Request();
            deleteTextRequest.DeleteText = new DeleteTextRequest();
            deleteTextRequest.DeleteText.ObjectId = PrintTitleRect.ObjectId;
            deleteTextRequest.DeleteText.TextRange = new Google.Apis.Slides.v1.Data.Range() { Type = "ALL" };

            var insertTextRequest = new Request();
            insertTextRequest.InsertText = new InsertTextRequest();
            insertTextRequest.InsertText.ObjectId = PrintTitleRect.ObjectId;
            insertTextRequest.InsertText.Text = iPrint.PrintName;
            insertTextRequest.InsertText.InsertionIndex = 0;

            var requests = new List<Request>() { deleteTextRequest, insertTextRequest };
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
