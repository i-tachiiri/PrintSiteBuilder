using Google.Apis.Auth.OAuth2;
using Google.Apis;
using Google.Apis.Drive;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using PrintSiteBuilder.Interfaces;
using Google;

namespace PrintSiteBuilder.GoogleService.Drive
{
    public class TemplateFolder
    {
        public string TemplateFolderId = "1hFL0vySMJT7uFZkWDLFja_Do-B3OqP00";
        public string RootFolderId = "18LcIP4xebaTDCgH7qGcZXaNpLffFwVAm";
        public List<string> PrintSlideNames;
        public GoogleDrive drive;
        public DriveService driveService;
        public IPrint2 iPrint;
        public TemplateFolder(IPrint2 iPrint2)
        {
            drive = new GoogleDrive();
            driveService = drive.driveService;
            iPrint = iPrint2;
            PrintSlideNames = new List<string> { $"{iPrint.PrintId}-template", $"{iPrint.PrintId}-cover", $"{iPrint.PrintId}-amazon" };
        }
        public async Task CopyTemplate()
        {
            var PrintFolderId = await GerPrintFolderId();
            var TemplateSlides = await GetTemplateSlides();
            foreach (var slide in TemplateSlides)
            {
                await CopyTemplateFile(slide, PrintFolderId);
            }
        }

        public async Task<bool> IsFileInFolder(string PrintFolderId, string PrintSlideName)
        {
            var request = driveService.Files.List();
            request.Q = $"'{PrintFolderId}' in parents and name = '{PrintSlideName}'";
            request.Fields = "files(id, name)";
            var result = await request.ExecuteAsync();
            return result.Files.Count > 0;
        }

        public async Task<string> GerPrintFolderId()
        {
            try
            {
                var request = driveService.Files.List();
                request.Q = $"'{RootFolderId}' in parents and mimeType = 'application/vnd.google-apps.folder' and name = '{iPrint.PrintId}'";
                request.Fields = "files(id, name)";
                request.SupportsAllDrives = true; // 共有ドライブをサポート
                request.IncludeItemsFromAllDrives = true; // すべてのドライブからアイテムを含める

                var result = await request.ExecuteAsync();
                var folder = result.Files.Count > 0 ? result.Files[0] : null;
                var folderId = folder?.Id;
                if (folderId != null)
                {
                    return folderId;
                }
                var newFolder = new Google.Apis.Drive.v3.Data.File
                {
                    Name = iPrint.PrintId,
                    MimeType = "application/vnd.google-apps.folder",
                    Parents = new List<string> { RootFolderId }
                };
                var request2 = driveService.Files.Create(newFolder);
                request2.Fields = "id";
                request2.SupportsAllDrives = true; // 共有ドライブをサポート
                var createdFolder = await request2.ExecuteAsync();
                return createdFolder.Id;
            }
            catch (GoogleApiException ex)
            {
                Console.WriteLine($"Google API error occurred: {ex.Message}");
                if (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine("The specified parent folder ID was not found. Please verify the ID.");
                }
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<IList<Google.Apis.Drive.v3.Data.File>> GetTemplateSlides()
        {
            var request = driveService.Files.List();
            request.Q = $"'{TemplateFolderId}' in parents and mimeType = 'application/vnd.google-apps.presentation'";
            request.Fields = "nextPageToken, files(id, name, mimeType)";
            request.SupportsAllDrives = true; // 共有ドライブをサポート
            request.IncludeItemsFromAllDrives = true; // すべてのドライブからアイテムを含める            
            var result = await request.ExecuteAsync();
            return result.Files;
        }


        private async Task CopyTemplateFile(Google.Apis.Drive.v3.Data.File slide, string PrintFolderId)
        {
            var PrintSlideName = slide.Name.Replace("000000", iPrint.PrintId);
            if (await IsFileInFolder(PrintFolderId, PrintSlideName))
            {
                return;
            }
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            { 
                Name = PrintSlideName,
                Parents = new List<string> { PrintFolderId }
            };

            var request = driveService.Files.Copy(fileMetadata, slide.Id);
            var file = await request.ExecuteAsync();

            var updateRequest = driveService.Files.Update(new Google.Apis.Drive.v3.Data.File(), file.Id);
            updateRequest.AddParents = PrintFolderId;
            updateRequest.RemoveParents = "root";
            updateRequest.Fields = "id, parents";
            await updateRequest.ExecuteAsync();
            
        }
        public async Task<List<Google.Apis.Drive.v3.Data.File>> GetPrintSlides()
        {
            var request = driveService.Files.List();
            var printFolderId = await GerPrintFolderId();
            request.Q = $"'{printFolderId}' in parents and mimeType = 'application/vnd.google-apps.presentation'";
            request.Fields = "nextPageToken, files(id, name, mimeType)";
            request.SupportsAllDrives = true;
            request.IncludeItemsFromAllDrives = true;
            var result = await request.ExecuteAsync();
            return result.Files.ToList();
        }
        public async Task<string> CreateGoogleSlide(string slideName)
        {
            try
            {
                // Googleスライドを作成
                var newSlide = new Google.Apis.Drive.v3.Data.File
                {
                    Name = slideName,
                    MimeType = "application/vnd.google-apps.presentation",
                    Parents = new List<string> { RootFolderId }
                };

                var request = driveService.Files.Create(newSlide);
                request.Fields = "id";
                var createdSlide = await request.ExecuteAsync();

                Console.WriteLine($"Google Slide created: {createdSlide.Id}");
                return createdSlide.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
    }
}
