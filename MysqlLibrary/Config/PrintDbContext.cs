using BlogDomain.Entity;
using MathDomain.Entity;
using Microsoft.EntityFrameworkCore;
namespace MysqlLibrary.Config
{
    public class PrintDbContext : DbContext
    {
        public PrintDbContext(DbContextOptions<PrintDbContext> options) : base(options) { }

        public DbSet<BlogEntity> Blog { get; set; }
        public DbSet<CosineEntity> Cosine { get; set; }
        public DbSet<SearchEntity> Search { get; set; }
        public DbSet<mMathEntity> Math { get; set; }
        public DbSet<mQuestionEntity> Question { get; set; }
        public DbSet<sQuestionEntity> sQuestion { get; set; }
    }
}
