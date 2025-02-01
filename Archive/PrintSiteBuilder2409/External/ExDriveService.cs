using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Requests;
using PrintSiteBuilder2409.Config;
using PrintSiteBuilder2409.IExternal;
namespace PrintSiteBuilder2409.External
{
    public class ExDriveService : IExDriveService
    {
        private readonly DriveService service;
        public ExDriveService()
        {
            service = new GoogleApiClient().GetDriveService();
        }
        public async Task<Google.Apis.Drive.v3.Data.File> CopyFile(Google.Apis.Drive.v3.Data.File file, string ParentFolderId, string SlideName)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = SlideName,
                Parents = new List<string> { ParentFolderId }
            };
            var request = service.Files.Copy(fileMetadata, file.Id);
            return await request.ExecuteAsync();
        }
        public async Task<Google.Apis.Drive.v3.Data.File> CreateFolder(string PrintId)
        {
            var folderMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = PrintId, 
                MimeType = "application/vnd.google-apps.folder", 
                Parents = new List<string> { Config.Constants.GoogleDrive.RootFolderId } 
            };
            var createRequest = service.Files.Create(folderMetadata);
            createRequest.Fields = "id, name, parents"; 
            var folder = await createRequest.ExecuteAsync();
            return folder;
        }

        public async Task MoveFile(string ParentFolderId, Google.Apis.Drive.v3.Data.File file)
        {
            var updateRequest = service.Files.Update(new Google.Apis.Drive.v3.Data.File(), file.Id);
            updateRequest.AddParents = ParentFolderId;
            updateRequest.RemoveParents = "root";
            updateRequest.Fields = "id, parents";
            await updateRequest.ExecuteAsync();
        }
        public async Task<FileList> SearchFile(string PrintFolderId, string PrintSlideName)
        {
            var request = service.Files.List();
            request.Q = $"'{PrintFolderId}' in parents and name = '{PrintSlideName}'";
            request.Fields = "files(id, name)";
            var result = await request.ExecuteAsync();
            return result;
        }
        public async Task<FileList> SearchFolder(string PrintFolderId, string PrintSlideName)
        {
            var request = service.Files.List();
            request.Q = $"'{PrintFolderId}' in parents and name = '{PrintSlideName}' and mimeType = 'application/vnd.google-apps.folder'";
            request.Fields = "files(id, name)";
            var result = await request.ExecuteAsync();
            return result;
        }
        public async Task PermitReadToPublic(string fileId)
        {
            Permission userPermission = new Permission
            {
                Type = "anyone",
                Role = "reader"
            };
            var request = service.Permissions.Create(userPermission, fileId);
            request.Fields = "id";
            await request.ExecuteAsync();
        }

        public async Task DenyPublicAccess(string fileId)
        {
            var permissions = await service.Permissions.List(fileId).ExecuteAsync();
            foreach (var permission in permissions.Permissions)
            {
                if (permission.Type == "anyone")
                {
                    await service.Permissions.Delete(fileId, permission.Id).ExecuteAsync();
                }
            }
        }

    }
}
