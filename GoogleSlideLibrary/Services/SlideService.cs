using Google.Apis.Slides.v1;
using Google.Apis.Slides.v1.Data;
using GoogleSlideLibrary.Config;
using TempriDomain.Interfaces;

namespace GoogleSlideLibrary.Services
{
    public class SlideService
    {
        public string PresentationID;
        public SlidesService slideService;
        public Presentation presentation;
        private readonly SlidesConnecter slidesConnecter;
        public SlideService(string ID, SlidesConnecter slidesConnecter)
        {
            PresentationID = ID;
            this.slidesConnecter = slidesConnecter;
            slideService = slidesConnecter.GetSlidesService();
            presentation = slideService.Presentations.Get(PresentationID).Execute();
        }
        public void batchUpdate(List<Request> requests)
        {
            try
            {
                var batchUpdateRequest = new BatchUpdatePresentationRequest { Requests = requests };
                var result = slideService.Presentations.BatchUpdate(batchUpdateRequest, PresentationID).Execute();
            }
            catch ( Exception ex )
            {
                //MessageBox.Show($"[Utilities.SlidePages.BatchUpdate]{ex.Message}");
            }
        }
        public void SyncPageNumber(IPrintEntity printClass)
        {
            int desiredPageCount = printClass.PagesCount * 2;
            int currentPageCount = presentation.Slides.Count;

            List<Request> requests = new List<Request>();

            if ( currentPageCount < desiredPageCount )
            {
                // スライドが不足している場合、1枚目のスライドをコピー
                string slideObjectId = presentation.Slides[0].ObjectId;
                for ( int i = currentPageCount; i < desiredPageCount; i++ )
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
            else if ( currentPageCount > desiredPageCount )
            {
                // スライドが多い場合、後ろから削除
                for ( int i = currentPageCount - 1; i >= desiredPageCount; i-- )
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

            if ( requests.Count > 0 )
            {
                batchUpdate(requests);
            }
        }
    }
}
