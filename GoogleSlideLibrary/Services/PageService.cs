using GoogleSlideLibrary.Config;
using TempriDomain.Interfaces;

namespace GoogleSlideLibrary.Services
{
    public class PageService : IPageService
    {
        private readonly SlidesConnecter slidesConnecter;

        public PageService(SlidesConnecter slidesConnecter)
        {
            this.slidesConnecter = slidesConnecter;
        }

        /// <summary>
        /// Get the PageObjectId from Google Slides by PageIndex
        /// </summary>
        /// <param name="presentationId">Google Slides Presentation ID</param>
        /// <param name="pageIndex">Index of the page (0-based)</param>
        /// <returns>PageObjectId as a string</returns>
        public async Task<string> GetPageObjectIdByIndex(string presentationId, int pageIndex)
        {
            try
            {
                // Get the Slides API service
                var slidesService = slidesConnecter.GetSlidesService();

                // Retrieve the presentation
                var request = slidesService.Presentations.Get(presentationId);
                var presentation = await request.ExecuteAsync();

                // Validate the page index
                if (presentation.Slides == null || presentation.Slides.Count <= pageIndex || pageIndex < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(pageIndex), "Invalid page index.");
                }

                // Return the PageObjectId of the requested page
                return presentation.Slides[pageIndex].ObjectId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching PageObjectId: {ex.Message}");
                return null;
            }
        }
    }
}
