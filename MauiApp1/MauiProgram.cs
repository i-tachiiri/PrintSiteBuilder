using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MysqlLibrary.Config;
using MysqlLibrary.Repository.Crypto;
using SqliteLibrary.Config;

namespace MauiApp1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var CryptoConnectionString = "Server=localhost;Database=crypto;User ID=root;Password=@dmin1239;";

            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");});

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddDbContext<AppDbContext>();
            builder.Services.AddDbContext<CryptoDbContext>(options => options.UseMySql(CryptoConnectionString, ServerVersion.AutoDetect(CryptoConnectionString)));
            builder.Services.AddSingleton<MysqlPublicTradeRepository>();


#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            var app = builder.Build();

            return app;
        }
    }
}


/* SQLiteで起動時に「DBがなければ作成」する処理
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite($"Filename={Path.Combine(FileSystem.AppDataDirectory, "appdata.db")}"));
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated(); 
}
*/
