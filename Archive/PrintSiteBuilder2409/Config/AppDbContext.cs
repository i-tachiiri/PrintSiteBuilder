using Microsoft.EntityFrameworkCore;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder2409.Entities;

namespace PrintSiteBuilder2409.Config
{
    public class AppDbContext : DbContext
    {
        string connectionString = "Server=localhost;Database=printsitebuilder;User ID=root;Password=@dmin1239;";
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TemplateMaster> TemplateMaster { get; set; }
        public DbSet<ExerciseMaster> ExerciseMaster { get; set; }
        public DbSet<PrintMaster> PrintMaster { get; set; }
        public DbSet<SlideMaster> SlideMaster { get; set; }
        public DbSet<PageMaster> PageMaster { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
    }
}
