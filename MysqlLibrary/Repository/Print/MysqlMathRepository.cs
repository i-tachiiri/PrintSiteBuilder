using BlogDomain.Entity;
using MathDomain.Entity;
using Microsoft.EntityFrameworkCore;
using MysqlLibrary.Config;

namespace MysqlLibrary.Repository
{
    public class MysqlMathRepository
    {
        private readonly PrintDbContext _context;

        public MysqlMathRepository(PrintDbContext context)
        {
            _context = context;
        }
        public async Task<List<mMathEntity>> GetAllAsync()
        {
            return await _context.Math.ToListAsync();
        }
        public async Task<List<mMathEntity>> GetByClassAsync(string ClassName)
        {
            //return await _context.Math.Where(x => x.FunctionName == FunctionName && x.ClassName == ClassName).ToListAsync();
            return await _context.Math.Where(x => x.ClassName == ClassName).ToListAsync();
        }

        public async Task UpsertMathListAsync(List<mMathEntity> entities)
        {
            foreach (var entity in entities)
            {
                var existingEntity = await _context.Math.FirstOrDefaultAsync(b => b.PageId == entity.PageId);

                if (existingEntity == null)
                {
                    await _context.Math.AddAsync(entity);
                }
                else
                {
                    entity.Id = existingEntity.Id;
                    _context.Entry(existingEntity).CurrentValues.SetValues(entity);

                }
            }
            if (_context.ChangeTracker.HasChanges())
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}
