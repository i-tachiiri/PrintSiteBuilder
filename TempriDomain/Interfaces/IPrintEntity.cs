using TempriDomain.Entity;

namespace TempriDomain.Interfaces
{
    public interface IPrintEntity
    {
        Task<PrintEntity> GetPrintAsync();
    }
}
