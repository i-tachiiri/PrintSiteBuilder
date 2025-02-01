

using BlogDomain.Entity;
using Microsoft.EntityFrameworkCore;
using MysqlLibrary.Config;

namespace MysqlLibrary.Repository.Print
{
    public class MysqlSearchRepository
    {
        private readonly PrintDbContext _context;

        public MysqlSearchRepository(PrintDbContext context)
        {
            _context = context;
        }
        public async Task UpsertCosineListAsync(List<SearchEntity> entities)
        {
            foreach (var entity in entities)
            {
                var existingEntity = await _context.Search.FirstOrDefaultAsync(b => b.FromDate == entity.FromDate && b.Period == entity.Period && b.PageId == entity.PageId && b.Query == entity.Query);

                if (existingEntity == null)
                {
                    await _context.Search.AddAsync(entity);
                }
                else
                {
                    _context.Entry(existingEntity).CurrentValues.SetValues(entity);

                }
            }
            if (_context.ChangeTracker.HasChanges())
            {
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<string>> GetMostClickedAsync()
        {
            return await _context.Search
                    .FromSqlRaw(@"SELECT PageId, SUM(Click)As Click
                                    FROM printsitebuilder.m_search 
                                    GROUP BY PageId order by Click desc LIMIT 10")
                    .Select(x => x.PageId)
                    .ToListAsync();
        }
    }
}
