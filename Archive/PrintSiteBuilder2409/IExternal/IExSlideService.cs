using Google.Apis.Slides.v1.Data;
namespace PrintSiteBuilder2409.IExternal
{
    public interface IExSlideService
    {
        public void batchUpdate(List<Request> requests, string PresentatonId);
        public Presentation GetPresentation(string PresentationId);
        public Request GetDuplicateObjectRequest(string ObjectId);
        public Request GetDeleteObjectRequest(string ObjectId);
        public Task ExportImages(Presentation presentation, string Attribute,string Extention);

        public Task ExportLogo(Presentation presentation, string Attribute, string Extention);
    }
}
