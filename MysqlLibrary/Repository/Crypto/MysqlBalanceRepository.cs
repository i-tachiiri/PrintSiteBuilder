
using BlogDomain.Entity;
using CoincheckDomain.Entity;
using Microsoft.EntityFrameworkCore;
using MysqlLibrary.Config;
namespace MysqlLibrary.Repository
{
    public class MysqlBalanceRepository
    {
        private readonly CryptoDbContext _context;

        public MysqlBalanceRepository(CryptoDbContext context)
        {
            _context = context;
        }
        // 全件のブログ記事を取得する
        public async Task<List<BalanceEntity>> GetAllAsync()
        {
            return await _context.Balance.ToListAsync();
        }
        public async Task UpsertBalanceAsync(List<BalanceEntity> entities)
        {
            await _context.Balance.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

    }
}
