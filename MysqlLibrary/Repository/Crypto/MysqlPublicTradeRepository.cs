using CoincheckDomain.Entity;
using MathDomain.Entity;
using Microsoft.EntityFrameworkCore;
using MysqlLibrary.Config;

namespace MysqlLibrary.Repository.Crypto
{
    public class MysqlPublicTradeRepository
    {
        private readonly CryptoDbContext _context;

        public MysqlPublicTradeRepository(CryptoDbContext context)
        {
            _context = context;
        }
        public async Task<List<PublicTradeEntity>> GetAllAsync()
        {
            return await _context.PublicTrade.ToListAsync();
        }
        public async Task<List<PublicTradeEntity>> GetTopRecordsAsync()
        {
            return await _context.PublicTrade
                .OrderBy(e => e.CreatedAt)  // CreatedAt プロパティで並び替え
                .Take(1000000)               // 上位 1,000,000 件のみ取得
                .ToListAsync();
        }
        public async Task UpsertPublicTradeAsync(List<PublicTradeEntity> entities)
        {
            foreach (var entity in entities)
            {
                var existingEntity = await _context.PublicTrade.FirstOrDefaultAsync(b => b.Pair == entity.Pair && b.TradeId == entity.TradeId);

                if (existingEntity == null)
                {
                    await _context.PublicTrade.AddAsync(entity);
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
