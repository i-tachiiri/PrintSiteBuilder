using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Slides.v1;
using Newtonsoft.Json.Linq;
using System.IO;

namespace PrintSiteBuilder2409.Config
{
    public class GoogleApiClient
    {
        private JObject jObject;
        private string serviceAccountEmail;
        private string certificate;
        public SlidesService SlideService { get; private set; }

        public GoogleApiClient()
        {
            jObject = JObject.Parse(File.ReadAllText(@"client_secret.json"));
            serviceAccountEmail = jObject["client_email"].ToString();
            certificate = jObject["private_key"].ToString();
            SlideService = GetSlideService();
        }

        public ServiceAccountCredential GetCredential(string scope)
        {
            return new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(serviceAccountEmail)
                {
                    Scopes = new[] { scope }
                }.FromPrivateKey(certificate));
        }

        public SlidesService GetSlideService()
        {
            var credential = GetCredential(SlidesService.Scope.Presentations);
            return new SlidesService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
            });
        }
        public SheetsService GetSheetsService()
        {
            var credential = GetCredential(SheetsService.Scope.Spreadsheets);
            return new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
            });
        }
        public DriveService GetDriveService()
        {
            var credential = GetCredential(DriveService.Scope.DriveFile);
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
            });
        }

    }
}
