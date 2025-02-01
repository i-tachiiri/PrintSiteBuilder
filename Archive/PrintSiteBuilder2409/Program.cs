using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using PrintSiteBuilder2409.Repository;
using PrintSiteBuilder2409.IRepository;
using PrintSiteBuilder2409.Config;
using PrintSiteBuilder2409.Services;
using PrintSiteBuilder2409.External;

namespace PrintSiteBuilder2409
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            var services = new ServiceCollection();
            ConfigureServices(services);
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var form1 = serviceProvider.GetRequiredService<Form1>();
                System.Windows.Forms.Application.Run(form1);
            }
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            string connectionString = "Server=localhost;Database=printsitebuilder;User ID=root;Password=@dmin1239;";
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            services.AddScoped<ITemplateRepository, TemplateRepository>();
            services.AddScoped<IExerciseRepository, ExerciseRepository>();
            services.AddScoped<IPrintRepository, PrintRepository>();
            services.AddScoped<Form1>();
            services.AddScoped<ExDriveService>();
            services.AddScoped<P100004>(provider 
                => new P100004(provider.GetRequiredService<IExerciseRepository>(), provider.GetRequiredService<IPrintRepository>(), "P100004"));


        }
    }
}