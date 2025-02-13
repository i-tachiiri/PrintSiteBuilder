using Google.Apis.Slides.v1;
using Google.Apis.Slides.v1.Data;
using PrintSiteBuilder.Interfaces;

namespace PrintSiteBuilder.GoogleService.Slide
{
    public class SlidePages
    {
        public GoogleApi googleApi;
        public string PresentationID;
        public SlidesService slideService;
        public Google.Apis.Slides.v1.Data.Presentation presentation;
        public SlidePages(string ID)
        {
            googleApi = new GoogleApi();
            PresentationID = ID;
            slideService = googleApi.GetSlideService();
            presentation = slideService.Presentations.Get(PresentationID).Execute();
        }
        public void UpdateSlide(IPrint printClass)
        {
            SyncPageNumber(printClass);
            presentation = slideService.Presentations.Get(PresentationID).Execute();
            var requests = new List<Request>();
            var printConfigs = printClass.GetPrintConfigs().OrderBy(printConfig => printConfig.headerConfig.PageIndex).ToList();
            foreach (var printConfig in printConfigs)
            {
                var slidePage = new PrintSlidePage(presentation.Slides[printConfig.headerConfig.PageIndex]);
                var updateHeaderRequests = slidePage.GetUpdateHeaderRequest(printConfig.headerConfig.PrintType, printConfig.headerConfig.PrintName, printConfig.headerConfig.Score);
                requests.AddRange(updateHeaderRequests);

                foreach (var cellConfig in printConfig.cellConfigs)
                {
                    bool IsEmptyCell;
                    if (cellConfig.AnswerRow == null)
                    {
                        IsEmptyCell = printConfig.headerConfig.PrintType == "問題" && cellConfig.AnswerColumn.Contains(cellConfig.ColumnNumber);
                    }
                    else
                    {
                        IsEmptyCell = printConfig.headerConfig.PrintType == "問題" && cellConfig.AnswerColumn.Contains(cellConfig.ColumnNumber) && cellConfig.AnswerRow.Contains(cellConfig.RowNumber);
                    }
                    requests.AddRange(slidePage.GetUpdateCellRequest(cellConfig, IsEmptyCell));
                }
            }
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}]batchUpdate...");
            batchUpdate(requests);
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
        public void SyncPageNumber(IPrint printClass)
        {
            int desiredPageCount = printClass.PagesCount * 2;
            int currentPageCount = presentation.Slides.Count;

            List<Request> requests = new List<Request>();

            if (currentPageCount < desiredPageCount)
            {
                // スライドが不足している場合、1枚目のスライドをコピー
                string slideObjectId = presentation.Slides[0].ObjectId;
                for (int i = currentPageCount; i < desiredPageCount; i++)
                {
                    requests.Add(new Request
                    {
                        DuplicateObject = new DuplicateObjectRequest
                        {
                            ObjectId = slideObjectId
                        }
                    });
                }
            }
            else if (currentPageCount > desiredPageCount)
            {
                // スライドが多い場合、後ろから削除
                for (int i = currentPageCount - 1; i >= desiredPageCount; i--)
                {
                    requests.Add(new Request
                    {
                        DeleteObject = new DeleteObjectRequest
                        {
                            ObjectId = presentation.Slides[i].ObjectId
                        }
                    });
                }
            }

            if (requests.Count > 0)
            {
                batchUpdate(requests);
            }
        }
        public void SyncTemplateSlide(IPrint2 iPrint)
        {
            int currentPageCount = presentation.Slides.Count;
            List<Request> requests = new List<Request>();
            for (int i = 0; i < currentPageCount; i++)
            {
                if (i != iPrint.TemplateNumber - 1)
                {
                    var deleteRequest = new Request();
                    deleteRequest.DeleteObject = new DeleteObjectRequest();
                    deleteRequest.DeleteObject.ObjectId = presentation.Slides[i].ObjectId;
                    requests.Add(deleteRequest);
                }
            }
            if (requests.Count > 0)
            {
                batchUpdate(requests);
            }
        }
    }
}