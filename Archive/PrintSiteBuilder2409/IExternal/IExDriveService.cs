

using Google.Apis.Drive.v3.Data;

namespace PrintSiteBuilder2409.IExternal
{
    public interface IExDriveService
    {
        public Task<Google.Apis.Drive.v3.Data.File> CreateFolder(string PrintId);
        public Task<Google.Apis.Drive.v3.Data.File> CopyFile(Google.Apis.Drive.v3.Data.File file, string ParentFolderId, string SlideName);
        public Task MoveFile(string ParentFolderId, Google.Apis.Drive.v3.Data.File file);
        public Task<FileList> SearchFile(string PrintFolderId, string PrintSlideName);
        public Task<FileList> SearchFolder(string PrintFolderId, string PrintSlideName);
        public Task PermitReadToPublic(string fileId);
        public Task DenyPublicAccess(string fileId);
    }
}
