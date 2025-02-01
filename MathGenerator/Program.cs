using MathGenerator.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MysqlLibrary.Config;
using MysqlLibrary.Repository;
using NotionLibrary.External;
using NotionLibrary.Repository;
using NotionLibrary.Services;
using MathDomain.Services;
using Org.BouncyCastle.Asn1.Crmf;
using MysqlLibrary.Repository.Print;

var services = new ServiceCollection();
var connectionString = "Server=localhost;Database=printsitebuilder;User ID=root;Password=@dmin1239;";

services.AddSingleton<NotionMathRepository>();
services.AddSingleton<NotionDatabaseConverter>();
services.AddSingleton<MysqlMathRepository>();
services.AddSingleton<NotionConnecter>();
services.AddSingleton<MathLogger>();
services.AddSingleton<NotionMathRepository>();
services.AddSingleton<NotionDatabaseConverter>();
services.AddSingleton<MysqlMathRepository>();
services.AddSingleton<MathEntityMapper>();
services.AddSingleton<Addition>();
services.AddSingleton<Subtraction>();
services.AddSingleton<MysqlQuestionRepository>();
services.AddSingleton<MysqlSQuestionRepository>();
services.AddSingleton<QuestionSummarizer>();
services.AddSingleton<Multiplication>();
services.AddSingleton<QuestionEntityMapper>();
services.AddSingleton<SampleGetter>();

services.AddDbContext<PrintDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var serviceProvider = services.BuildServiceProvider();
var MathMapperService = serviceProvider.GetRequiredService<MathEntityMapper>();
var LogService = serviceProvider.GetRequiredService<MathLogger>();
var AdditionService = serviceProvider.GetRequiredService<Addition>();
var SubtractionService = serviceProvider.GetRequiredService<Subtraction>();
var MultiplicationService = serviceProvider.GetRequiredService<Multiplication>();


var QuestionSummerizeService = serviceProvider.GetRequiredService<QuestionSummarizer>();
var QuestionMasterCreator = serviceProvider.GetRequiredService<Multiplication>();

await LogService.TaskLog(MathMapperService.UpsertMathEntitiesAsync);
await LogService.TaskLog(AdditionService.InsertQuestionEntity);
await LogService.TaskLog(SubtractionService.InsertQuestionEntity);
await LogService.TaskLog(MultiplicationService.InsertQuestionEntity);
await LogService.TaskLog(QuestionSummerizeService.SummarizeQuestion);
await LogService.LogMessage("Done");