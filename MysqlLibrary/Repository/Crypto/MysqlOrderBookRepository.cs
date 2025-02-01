using CoincheckDomain.Entity;
using MysqlLibrary.Config;
using Microsoft.EntityFrameworkCore;

namespace MysqlLibrary.Repository.Crypto
{
    public class MysqlOrderBookRepository
    {
        private readonly CryptoDbContext _context;

        public MysqlOrderBookRepository(CryptoDbContext context)
        {
            _context = context;
        }
        public async Task<List<OrderBookEntity>> GetAllAsync()
        {
            return await _context.OrderBook.ToListAsync();
        }
        public async Task InsertOrderBookAsync(List<OrderBookEntity> entities)
        {
            await _context.OrderBook.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }
    }
}
