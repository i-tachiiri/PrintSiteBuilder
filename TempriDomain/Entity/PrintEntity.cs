using TempriDomain.Config;
using TempriDomain.Interfaces;
namespace TempriDomain.Entity
{
    public abstract class PrintEntity : IPrintEntity
    {
        public string PresentationId { get; set; }
        public int PrintId { get; set; }
        public string PrintName { get; set; }
        public int PagesCount { get; set; }
        public int Score { get; set; }
        public List<IPageEntity> Pages { get; set; }

        // Abstract method for subclass instantiation
        protected abstract PrintEntity CreateInstanceWithPages(List<IPageEntity> pages);

        public async Task<PrintEntity> SetPrintAsync(IPageService pageService)
        {
            var pageEntities = new List<IPageEntity>();

            foreach (var page in Pages)
            {
                var pageEntity = await page.SetPageAsync(this,PresentationId, PagesCount,pageService);
                pageEntities.AddRange(pageEntity);
            }

            return CreateInstanceWithPages(pageEntities);
        }
        public string GetDirectoryPath()
        {
            return Path.Combine(TempriConstants.BaseDir, PrintId.ToString());
        }
        public string GetDirectoryPathWithName(string FolderName)
        {
            return Path.Combine(TempriConstants.BaseDir, PrintId.ToString(), FolderName);
        }
    }
}
