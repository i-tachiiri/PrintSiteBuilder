using CoincheckDomain.Entity;
using Microsoft.EntityFrameworkCore;

namespace MysqlLibrary.Config
{
    public class CryptoDbContext : DbContext
    {
        public CryptoDbContext(DbContextOptions<CryptoDbContext> options) : base(options) { }
        public DbSet<BalanceEntity> Balance { get; set; }
        public DbSet<PublicTradeEntity> PublicTrade { get; set; }
        public DbSet<OrderBookEntity> OrderBook { get; set; }
    }
}
