using TempriDomain.Entity;

namespace TempriDomain.Interfaces
{
    public interface IPrintEntity
    {
        Task<PrintEntity> SetPrintAsync(IPageService pageService);
        string PresentationId { get; }
        string PrintName { get; }
        int PrintId { get; }
        int PagesCount { get; }
        int Score { get; }
        List<IPageEntity> Pages { get; }
        string GetDirectoryPath();
        string GetDirectoryPathWithName(string FolderName);

    }
}
