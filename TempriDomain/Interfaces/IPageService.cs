namespace TempriDomain.Interfaces
{
    public interface IPageService
    {
        Task<string> GetPageObjectIdByIndex(string presentationId, int pageIndex);
    }
}
