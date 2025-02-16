using TempriDomain.Entity;

namespace TempriDomain.Interfaces
{
    public interface IPrint
    {
        Task<PrintEntity> GetPrintAsync();
    }
}
