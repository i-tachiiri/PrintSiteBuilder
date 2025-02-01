using FikaAmazonAPI.AmazonSpApiSDK.Models.VendorOrders;
using PrintSiteBuilder2409.External;
using PrintSiteBuilder2409.IExternal;
using PrintSiteBuilder2409.IValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder2409.Validation
{
    public class DriveValidation : IDriveValidation
    {
        private readonly IExDriveService ExDriveService;
        public DriveValidation(IExDriveService exDriveService)
        {
            this.ExDriveService = exDriveService;
        }
        public async Task<bool> IsPrintFolderCreated(string PrintId)
        {
            var SearchResult = await ExDriveService.SearchFolder(Config.Constants.GoogleDrive.RootFolderId,PrintId);
            return SearchResult.Files.Any();
        }
        public async Task<bool> IsFileCreated(string PrintId, string SearchWord)
        {
            var SearchParentFolder = await ExDriveService.SearchFolder(Config.Constants.GoogleDrive.RootFolderId, PrintId);
            Google.Apis.Drive.v3.Data.File ParentFolder;
            if (SearchParentFolder.Files.Any())
            {
                ParentFolder = SearchParentFolder.Files[0];
            }
            else
            {
                var CreatedFolder = await ExDriveService.CreateFolder(PrintId);
                ParentFolder = CreatedFolder;
            }
            var SearchResult = await ExDriveService.SearchFile(ParentFolder.Id, SearchWord);
            return SearchResult.Files.Any();
        }
    }
}
