using TempriDomain.Entity;

namespace TempriDomain.Interfaces
{
    public interface IPrint
    {
        Task<PrintEntity> GetPrintAsync();
        string presentationId {  get; }
        int printId {  get; }
        int pagesCount {  get; }
        int score {  get; }
        List<IPage> pages { get; }
    }
}
