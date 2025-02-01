using BlogDomain.Entity;
using Microsoft.EntityFrameworkCore;
using MysqlLibrary.Config;

namespace MysqlLibrary.Repository.Print
{
    public class MysqlCosineRepository
    {
        private readonly PrintDbContext _context;

        public MysqlCosineRepository(PrintDbContext context)
        {
            _context = context;
        }
        public async Task<List<CosineEntity>> GetAllAsync()
        {
            return await _context.Cosine.ToListAsync();
        }
        public async Task UpsertCosineListAsync(List<CosineEntity> entities)
        {
            foreach (var entity in entities)
            {
                var existingEntity = await _context.Cosine.FirstOrDefaultAsync(b => b.BaseId == entity.BaseId && b.TargetId == entity.TargetId);

                if (existingEntity == null)
                {
                    await _context.Cosine.AddAsync(entity);
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
        public async Task<List<string>> GetRelatedPostIds(string BaseId)
        {
            return await _context.Cosine
                    .FromSqlRaw(@"SELECT TargetId
                                    FROM printsitebuilder.m_cosine 
                                    JOIN printsitebuilder.m_blog ON m_cosine.BaseId = m_blog.PageId
                                    WHERE m_cosine.BaseId = {0} AND TitleSimilarity > 0
                                    ORDER BY Score DESC LIMIT 10", BaseId)
                    .Select(x => x.TargetId)
                    .ToListAsync();
        }
    }
}
