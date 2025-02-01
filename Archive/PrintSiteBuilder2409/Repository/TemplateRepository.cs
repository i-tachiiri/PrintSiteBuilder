using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder2409.Config;
using PrintSiteBuilder2409.Entities;
using PrintSiteBuilder2409.IRepository;

namespace PrintSiteBuilder2409.Repository
{
    public class TemplateRepository : ITemplateRepository
    {
        private readonly AppDbContext _context;

        public TemplateRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<TemplateMaster> GetByIdAsync(string id)
        {
            return await _context.TemplateMaster.FindAsync(id);
        }
    }
}
