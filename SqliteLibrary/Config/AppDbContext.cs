using TrainingDomain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Storage;

namespace SqliteLibrary.Config
{
    public class AppDbContext : DbContext
    {
        public DbSet<YesNoQuestionEntity> yesNoQuestionEntity { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "appdata.db");
                optionsBuilder.UseSqlite($"Filename={dbPath}");
            }
        }
    }
}
