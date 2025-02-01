using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TempriDomain.Services;
using ExplorerLibrary.Repository;   
using MysqlLibrary.Config;
using NotionLibrary.External;
using NotionLibrary.Repository;
using NotionLibrary.Services;
using MysqlLibrary.Repository;
using TempriDomain.Entity;

var services = new ServiceCollection();
var connectionString = "Server=localhost;Database=printsitebuilder;User ID=root;Password=@dmin1239;";

services.AddSingleton<NotionTemplateRepository>();
services.AddSingleton<SvgGenerator>();
services.AddSingleton<NotionConnecter>();
services.AddSingleton<NotionPropertyConverter>();
services.AddSingleton<NotionDatabaseConverter>();
services.AddSingleton<ExplorerTemplateRepository>();
services.AddSingleton<MysqlTemplateRepository>();

services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var serviceProvider = services.BuildServiceProvider();
var explorerRepository = serviceProvider.GetService<ExplorerTemplateRepository>();
var svgGenerator = serviceProvider.GetService<SvgGenerator>();
var notionTemplateRepository = serviceProvider.GetService<NotionTemplateRepository>();
var mysqlTemplateRepository = serviceProvider.GetService<MysqlTemplateRepository>();


//ファイル→DB(Insert)→Notion(Insert)→DB(Update)

var Entities = new List<TemplateEntity>();
Entities = explorerRepository.GetAllTemplates();
await mysqlTemplateRepository.InsertTemplateListAsync(Entities);
Entities = await mysqlTemplateRepository.GetAllAsync();
await notionTemplateRepository.InsertUniqueTemplateAsync(Entities);
Entities = await notionTemplateRepository.GetTemplatesAsync();
await mysqlTemplateRepository.UpsertTemplateListAsync(Entities);