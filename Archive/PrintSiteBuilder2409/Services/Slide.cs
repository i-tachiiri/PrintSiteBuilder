using PrintSiteBuilder2409.IServices;
using PrintSiteBuilder2409.IValidation;
using PrintSiteBuilder2409.IExternal;
using PrintSiteBuilder2409.IRepository;
using PrintSiteBuilder2409.Validation;
using PrintSiteBuilder2409.IFactory;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Google.Apis.Slides.v1.Data;
using PrintSiteBuilder2409.Entities;


namespace PrintSiteBuilder2409.Services
{
    public class Slide : ISlide
    {
        private readonly IExDriveService exDriveService;
        private readonly IExSlideService exSlideService;
        private readonly IDriveValidation driveValidation;
        private readonly ISlideValidation slideValidation;
        private readonly IEntityFactory slideFactory;
        private readonly IExerciseRepository exerciseMasterRepository;
        private readonly ISlideRepository slideMasterRepository;
        public Slide(IExDriveService exDriveService, IDriveValidation driveValidation, ISlideValidation slideValidation,
            IEntityFactory slideFactory, IExerciseRepository exerciseMasterRepository)
        {
            this.exDriveService = exDriveService;
            this.exSlideService = exSlideService;
            this.driveValidation = driveValidation;
            this.slideValidation = slideValidation;
            this.slideFactory = slideFactory;
            this.exerciseMasterRepository = exerciseMasterRepository;
        }
        public async Task CopyTemplate(string PrintId)
        {
            if (!await driveValidation.IsPrintFolderCreated(PrintId)) exDriveService.CreateFolder(PrintId);
            var Searchwords = new List<string> { "template", "cover", "amazon" };
            foreach (var searchWord in Searchwords)
            {
                if (!await driveValidation.IsFileCreated(PrintId, searchWord))
                {
                    var ParentFolders = await exDriveService.SearchFolder(Config.Constants.GoogleDrive.RootFolderId, PrintId);
                    var ParentFolder = ParentFolders.Files[0];
                    var TemplateSlides = await exDriveService.SearchFile(Config.Constants.GoogleDrive.RootFolderId, $"000000-{searchWord}");
                    var TemplateSlide = TemplateSlides.Files[0];
                    var CopiedFile = await exDriveService.CopyFile(TemplateSlide, ParentFolder.Id, $"{PrintId}-{searchWord}");
                    await exDriveService.MoveFile(ParentFolder.Id, CopiedFile);
                }

            }
        }
        public async Task CreateSlideMaster(string PrintId)
        {
            if (!await slideValidation.HasRecordByPrintId(PrintId))
            {
                var entity = slideFactory.CreateSlideMasterInstance();
                var SearchedFolders = await exDriveService.SearchFolder(Config.Constants.GoogleDrive.RootFolderId, PrintId);
                var SearchedFolderId = SearchedFolders.Files[0].Id;
                var SearchedFiles = await exDriveService.SearchFile(SearchedFolderId, PrintId);
                entity.PrintId = PrintId;
                entity.Attribute = "";
                entity.PageCount = 0;
                entity.TemplateNumber = 0;
                entity.FolderId = SearchedFolderId;
                entity.PrntSlideId = SearchedFiles.Files.FirstOrDefault(file => file.Name.Contains("template")).Id;
                entity.AmazonSlideId = SearchedFiles.Files.FirstOrDefault(file => file.Name.Contains("amazon")).Id;
                entity.CoverSlideId = SearchedFiles.Files.FirstOrDefault(file => file.Name.Contains("cover")).Id;
                entity.Uuid = string.IsNullOrEmpty(entity.PrntSlideId) ? "" : entity.PrntSlideId.Substring(0, 12);
            }
        }
        public async Task CreatePrintSlide(string PrintId)
        {
            if (await slideValidation.IsRecordSet(PrintId))
            {
                var entity = await slideMasterRepository.GetByIdAsync(PrintId);
                var presentation = exSlideService.GetPresentation(entity.PrntSlideId);
                int desiredPageCount = entity.PageCount * 2;
                int currentPageCount = presentation.Slides.Count;

                List<Request> requests = new List<Request>();
                if (currentPageCount < desiredPageCount)  // スライドが不足している場合、1枚目のスライドをコピー
                {
                    var slideObjectId = presentation.Slides[0].ObjectId;
                    for (int i = currentPageCount; i < desiredPageCount; i++)
                    {
                        requests.Add(exSlideService.GetDuplicateObjectRequest(slideObjectId));
                    }
                }
                if (currentPageCount > desiredPageCount)  // スライドが多い場合、後ろから削除
                {
                    for (int i = currentPageCount - 1; i >= desiredPageCount; i--)
                    {
                        requests.Add(exSlideService.GetDuplicateObjectRequest(presentation.Slides[i].ObjectId));
                    }
                }
                if (requests.Count > 0)
                {
                    exSlideService.batchUpdate(requests, presentation.PresentationId);
                }

            }
        }
    }
}