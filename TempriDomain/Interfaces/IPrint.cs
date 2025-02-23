using TempriDomain.Entity;

namespace TempriDomain.Interfaces
{
    public interface IPrint
    {
        Task<PrintEntity> SetPrintAsync();
        string PresentationId {  get; }
        string PrintName { get; }

        int PrintId {  get; }
        int PagesCount {  get; }
        int Score {  get; }
        List<IPage> Pages { get; }
    }
}
