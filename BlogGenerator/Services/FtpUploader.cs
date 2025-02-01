using BlogDomain.Config;
using LolipopLibrary;
namespace BlogGenerator.Services
{
    public class FtpUploader
    {
        private LolipopService service;
        public FtpUploader(LolipopService service)
        {
            this.service = service;
        }
        public async Task Upload()
        {
            await service.UploadDirectoryAsync(DomainConstants.Explorer.ReleaseFolder, "/");
        }
    }
}
