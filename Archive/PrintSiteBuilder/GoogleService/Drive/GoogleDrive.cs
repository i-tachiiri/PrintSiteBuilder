using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Requests;
using Google.Apis.Slides.v1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PrintSiteBuilder.GoogleService.Drive
{
    public class GoogleDrive
    {
        public GoogleApi googleApi;
        public string PresentationID;
        public DriveService driveService;
        public Google.Apis.Slides.v1.Data.Presentation presentation;
        public string ImageTempFolder;
        public GoogleDrive()
        {
            googleApi = new GoogleApi();
            driveService = googleApi.GetDriveService();
            ImageTempFolder = "1UjuJek5KktQdjkT-as8IFnexqqobH9GW";
        }
        public async Task<string> UploadTempImage(string localFilePath)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = Path.GetFileName(localFilePath),
                Parents = new List<string> { ImageTempFolder }
            };

            FilesResource.CreateMediaUpload request;
            using (var stream = new FileStream(localFilePath, FileMode.Open))
            {
                request = driveService.Files.Create(fileMetadata, stream, GetMimeType(localFilePath));
                request.Fields = "id";
                await request.UploadAsync();
            }

            var file = request.ResponseBody;
            if (file != null && !string.IsNullOrEmpty(file.Id))
            {
                return $"https://drive.google.com/uc?export=view&id={file.Id}";
            }

            return null;
        }

        private string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
            {
                mimeType = regKey.GetValue("Content Type").ToString();
            }
            return mimeType;
        }
        public async Task PermitReadToPublic(string fileId)
        {
            var batch = new BatchRequest(driveService);

            Permission userPermission = new Permission
            {
                Type = "anyone",
                Role = "reader"
            };

            var request = driveService.Permissions.Create(userPermission, fileId);
            request.Fields = "id";

            await request.ExecuteAsync();
        }

        public async Task DenyPublicAccess(string fileId)
        {
            var permissions = await driveService.Permissions.List(fileId).ExecuteAsync();
            foreach (var permission in permissions.Permissions)
            {
                if (permission.Type == "anyone")
                {
                    await driveService.Permissions.Delete(fileId, permission.Id).ExecuteAsync();
                }
            }
        }
    }
}

