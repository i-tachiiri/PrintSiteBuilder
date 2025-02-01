using PrintSiteBuilder2409.Config;
using PrintSiteBuilder2409.Entities;
using PrintSiteBuilder2409.IRepository;
using System.Diagnostics.Metrics;

namespace PrintSiteBuilder2409.Repository
{
    public class SlideRepository : ISlideRepository
    {
        private readonly AppDbContext _context;

        public SlideRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<SlideMaster> GetByIdAsync(string PrintId)
        {
            return await _context.SlideMaster.FindAsync(PrintId);
        }
        public async Task SetRecordAsync(SlideMaster entity)
        {
            var record = await GetByIdAsync(entity.PrintId);
            if (record == null)
            {
                await _context.SlideMaster.AddAsync(entity);
            }
            else
            {
                _context.Entry(record).CurrentValues.SetValues(entity);
            }
            await _context.SaveChangesAsync();
        }
    }
}
