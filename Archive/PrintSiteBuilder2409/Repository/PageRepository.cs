using Microsoft.EntityFrameworkCore;
using PrintSiteBuilder2409.Config;
using PrintSiteBuilder2409.Entities;
using PrintSiteBuilder2409.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder2409.Repository
{
    public class PageRepository : IPageRepository
    {
        private readonly AppDbContext _context;

        public PageRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<PageMaster> GetByKeysAsync(string PrintId,int PageNumber)
        {
            return await _context.PageMaster.FindAsync(PrintId,PageNumber);
        }
        public async Task<List<PageMaster>> GetByConditionAsync(string PrintId)
        {
            return await _context.PageMaster
                                 .Where(x => x.PrintId == PrintId)
                                 .ToListAsync();
        }
    }
}
