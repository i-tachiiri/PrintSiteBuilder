using BlogGenerator.Application.Services;
using BlogGenerator.Services;
using ExplorerLibrary.Repository;
using GoogleTtsLibrary;
using LolipopLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MysqlLibrary.Config;
using MysqlLibrary.Repository;
using NotionLibrary.External;
using NotionLibrary.Repository;
using NotionLibrary.Services;
using BlogDomain.Services;
using GoogleSearchConsoleLibrary;
using System.Diagnostics;
using MysqlLibrary.Repository.Print;


var services = new ServiceCollection();
var connectionString = "Server=localhost;Database=printsitebuilder;User ID=root;Password=@dmin1239;";

services.AddSingleton<NotionDatabaseConverter>();
services.AddSingleton<NotionPageConverter>();
services.AddSingleton<HtmlGenerator>();
services.AddScoped<MysqlBlogRepository>();
services.AddScoped<MysqlCosineRepository>();
services.AddScoped<DeployServices>();
services.AddScoped<LolipopService>();
services.AddScoped<GoogleTtsService>();
services.AddScoped<CoverGenerator>();
services.AddScoped<NotionFileDownloader>();
services.AddScoped<BlogEntityMapper>();
services.AddScoped<FtpUploader>();
services.AddScoped<Mp3Generator>();
services.AddScoped<NotionBlogRepostory>();
services.AddScoped<SitemapGenerator>();
services.AddScoped<HtaccessGenerator>();
services.AddScoped<IndexHtmlGenerator>();
services.AddScoped<NotionConnecter>();
services.AddScoped<NotionPageRepository>();
services.AddScoped<NotionPropertyConverter>();
services.AddScoped<ExplorerRepository>();
services.AddScoped<TextVectorCalculator>();
services.AddScoped<Logger>();
services.AddScoped<CosineEntityMapper>();
services.AddScoped<GoogleSearchConsoleService>();
services.AddScoped<SearchEntityMapper>();
services.AddScoped<MysqlSearchRepository>();


services.AddDbContext<PrintDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var serviceProvider = services.BuildServiceProvider();

var Mp3Service = serviceProvider.GetRequiredService<Mp3Generator>();
var NotionDatabaseService = serviceProvider.GetRequiredService<NotionDatabaseConverter>();
var NotionBlogRepository = serviceProvider.GetRequiredService<NotionBlogRepostory>();
var MysqlBlogRepository = serviceProvider.GetRequiredService<MysqlBlogRepository>();
var NotionFileService = serviceProvider.GetRequiredService<NotionFileDownloader>();
var HtmlService = serviceProvider.GetRequiredService<HtmlGenerator>();
var SitemapService = serviceProvider.GetRequiredService<SitemapGenerator>();
var HtaccessService = serviceProvider.GetRequiredService<HtaccessGenerator>();
var IndexHtmlService = serviceProvider.GetRequiredService<IndexHtmlGenerator>();
var BlogMapperService = serviceProvider.GetRequiredService<BlogEntityMapper>();
var CoverService = serviceProvider.GetRequiredService<CoverGenerator>();
var DeployService = serviceProvider.GetRequiredService<DeployServices>();
var FtpService = serviceProvider.GetRequiredService<FtpUploader>();
var LogService = serviceProvider.GetRequiredService<Logger>();
var CosineService = serviceProvider.GetRequiredService<CosineEntityMapper>();
var ConsoleService = serviceProvider.GetRequiredService<GoogleSearchConsoleService>();
var SearchMapperService = serviceProvider.GetRequiredService<SearchEntityMapper>();

//await ConsoleService.GetSearchAnalyticsDataAsync();
//return;

#if DEBUG
//await LogService.TaskLog(SearchMapperService.UpsertSearchEntities);
//await LogService.TaskLog(CosineService.Calc);
//await LogService.TaskLog(NotionFileService.DownloadNotionFiles);
await LogService.TaskLog(Mp3Service.GenerateMp3);
await LogService.TaskLog(CoverService.ConvertToWebp);
await LogService.TaskLog(BlogMapperService.UpsertBlogEntitiesAsync);
await LogService.TaskLog(HtmlService.GenerateHtml);
await LogService.TaskLog(SitemapService.GenerateSitemapText);
await LogService.TaskLog(HtaccessService.GenerateHtaccess);
await LogService.TaskLog(IndexHtmlService.GenerateIndexHtml);
await LogService.TaskLog(DeployService.Sync);
//await LogService.TaskLog(FtpService.Upload);
Console.WriteLine("file:///C:/drive/work/web/Test/Debug/blog/thoughts/115d9f5c-28f5-80bc-a121-f21c20eb4d58.html");

#elif RELEASE
Console.WriteLine(Debugger.IsAttached);
if (Debugger.IsAttached) await LogService.TaskLog(DeployService.DeployTestAssets);
await LogService.TaskLog(SearchMapperService.UpsertSearchEntities);
await LogService.TaskLog(CosineService.Calc);
await LogService.TaskLog(NotionFileService.DownloadNotionFiles);
await LogService.TaskLog(Mp3Service.GenerateMp3);
await LogService.TaskLog(CoverService.ConvertToWebp);
await LogService.TaskLog(BlogMapperService.UpsertBlogEntitiesAsync);
await LogService.TaskLog(HtmlService.GenerateHtml);
await LogService.TaskLog(SitemapService.GenerateSitemapText);
await LogService.TaskLog(HtaccessService.GenerateHtaccess);
await LogService.TaskLog(IndexHtmlService.GenerateIndexHtml);
await LogService.TaskLog(DeployService.Sync);
if (!Debugger.IsAttached) await LogService.TaskLog(FtpService.Upload);
#endif