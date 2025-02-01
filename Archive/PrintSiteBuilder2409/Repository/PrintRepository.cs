using PrintSiteBuilder2409.Config;
using PrintSiteBuilder2409.Entities;
using PrintSiteBuilder2409.IRepository;

namespace PrintSiteBuilder2409.Repository
{
    public class PrintRepository : IPrintRepository
    {
        private readonly AppDbContext _context;

        public PrintRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<PrintMaster> GetByIdAsync(string PrintId)
        {
            return await _context.PrintMaster.FindAsync(PrintId);
        }
    }
}
