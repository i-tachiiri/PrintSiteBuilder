using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Newtonsoft.Json.Linq;

namespace GoogleDriveLibrary.Config
{
    public class GoogleDriveConnector
    {
        private JObject jObject = JObject.Parse(File.ReadAllText(@"client_secret.json"));
        private string serviceAccountEmail => jObject["client_email"].ToString();
        private string certificate => jObject["private_key"].ToString();

        public ServiceAccountCredential GetCredential()
        {
            return new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(serviceAccountEmail)
                {
                    Scopes = new[] { DriveService.Scope.Drive }
                }.FromPrivateKey(certificate));
        }

        public DriveService GetDriveService()
        {
            var credential = GetCredential();
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DriveConnector"
            });
        }
    }
}
