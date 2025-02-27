using Google.Apis.Slides.v1;
using Google.Apis.Slides.v1.Data;

namespace GoogleSlideLibrary.Repository.Material
{
    public class CoverRepository
    {
        private readonly string PresentationId = "1QcZsChmNC0WBc0tG1INoxCunW01fc9DSw-enGyP1zIY";
        private readonly SlidesService service;
        public CoverRepository(SlidesService service)
        {
            this.service = service;
        }
        public async Task<List<Request>> GetPrintTitleUpdateRequest(string PrintName)
        {
            var presentation = service.Presentations.Get(PresentationId).Execute();
            var slide = presentation.Slides[0];
            var PrintTitleRect = slide.PageElements.Where(element => element.Shape != null && element.Shape.ShapeType == "RECTANGLE").OrderBy(element => element.Transform.TranslateY).FirstOrDefault();

            var deleteTextRequest = new Request();
            deleteTextRequest.DeleteText = new DeleteTextRequest();
            deleteTextRequest.DeleteText.ObjectId = PrintTitleRect.ObjectId;
            deleteTextRequest.DeleteText.TextRange = new Google.Apis.Slides.v1.Data.Range() { Type = "ALL" };

            var insertTextRequest = new Request();
            insertTextRequest.InsertText = new InsertTextRequest();
            insertTextRequest.InsertText.ObjectId = PrintTitleRect.ObjectId;
            insertTextRequest.InsertText.Text = PrintName;
            insertTextRequest.InsertText.InsertionIndex = 0;

            var requests = new List<Request>() { deleteTextRequest, insertTextRequest };
            return requests;
        }
        public async Task<List<Request>> GetCoverImageReplaceRequest(string PngUrl)
        {
            var presentation = service.Presentations.Get(PresentationId).Execute();
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
    }
}
