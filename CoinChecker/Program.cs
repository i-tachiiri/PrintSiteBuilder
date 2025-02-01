using CoincheckDomain.Entity;
using CoincheckDomain.Services;
using CoincheckLibrary.External;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MysqlLibrary.Config;
using MysqlLibrary.Repository;
using MysqlLibrary.Repository.Crypto;
using SpreadSheetLibrary.Config;
using SpreadSheetLibrary.Repository;


var services = new ServiceCollection();
var CryptoConnectionString = "Server=localhost;Database=crypto;User ID=root;Password=@dmin1239;";

services.AddSingleton<CoincheckConnecter>();
services.AddSingleton<MysqlBalanceRepository>();
services.AddSingleton<MysqlPublicTradeRepository>();
services.AddSingleton<MysqlOrderBookRepository>();
services.AddSingleton<PublicTradeSheetRepository>();
services.AddSingleton<SheetConnecter>();
services.AddSingleton<CoincheckLogger>();


services.AddDbContext<CryptoDbContext>(options => options.UseMySql(CryptoConnectionString, ServerVersion.AutoDetect(CryptoConnectionString)));


var serviceProvider = services.BuildServiceProvider();
var coincheckConnecter = serviceProvider.GetRequiredService<CoincheckConnecter>();
var balanceRepository = serviceProvider.GetRequiredService<MysqlBalanceRepository>();
var publicTradeRepository = serviceProvider.GetRequiredService<MysqlPublicTradeRepository>();
var orderBookRepository = serviceProvider.GetRequiredService<MysqlOrderBookRepository>();
var publicTradeSheetRepository = serviceProvider.GetRequiredService<PublicTradeSheetRepository>();
var sheetConnecter = serviceProvider.GetRequiredService<SheetConnecter>();
var coinCheckLogger = serviceProvider.GetService<CoincheckLogger>();

coinCheckLogger.Log("start");
Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
try
{
    var aaa = await coincheckConnecter.GetTradeHistory();
    await publicTradeRepository.UpsertPublicTradeAsync(aaa);
    coinCheckLogger.Log("updatePublicTrade");

    var publicTrades = await publicTradeRepository.GetTopRecordsAsync();
    publicTradeSheetRepository.UpdateValues(publicTrades);
    coinCheckLogger.Log("UpdateSheetValues");


    return;


    var orderBooks = await coincheckConnecter.GetAllOrderBooks();
    await orderBookRepository.InsertOrderBookAsync(orderBooks);

    return;

    var xxx = await coincheckConnecter.GetBalance();
    await balanceRepository.UpsertBalanceAsync(new List<BalanceEntity>() { xxx });
}
catch(Exception ex)
{
    Console.WriteLine($@"[{DateTime.Now}][error]{ex.Message}");
    coinCheckLogger.Log(ex.Message);
}