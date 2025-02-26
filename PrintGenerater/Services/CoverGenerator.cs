
using Google.Apis.Slides.v1.Data;
using Google.Apis.Slides.v1;
using GoogleSlideLibrary.Repository.Material;
using TempriDomain.Interfaces;

namespace PrintGenerater.Services
{
    public class CoverGenerator
    {

        private readonly CoverRepository coverRepository;
        public CoverGenerator(CoverRepository coverRepository)
        {
            this.coverRepository = coverRepository;
        }
        public async Task UpdateCoverSlide(IPrintEntity print)
        {
            //generate barcode before export cover
            //adjust reference
            //call drive service from this application layer
            var requests = new List<Request>();


            var CoverUrl = Directory.GetFiles(iPrint.path.PrintPngDir).FirstOrDefault();
            var BarcodeUrl = $@"{iPrint.path.PrintCoverDir}\code128.png";
            requests.AddRange(await GetCoverImageReplaceRequest(CoverUrl));
            requests.AddRange(await GetCoverBarcodeReplaceRequest(BarcodeUrl));
            requests.AddRange(await GetPrintTitleUpdateRequest(iPrint));
            batchUpdate(requests);
            var exportSlide = new ExportSlide();
            await exportSlide.ExportCoverPdf(presentation, iPrint);

            var drive = new GoogleDrive();
            var PngUrl = await drive.UploadTempImage(PngPath);
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
