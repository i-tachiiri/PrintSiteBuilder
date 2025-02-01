using DocumentLibrary.Repository;
using DocumentLibrary.Config;
using LolipopLibrary;

using Microsoft.Extensions.DependencyInjection;
using System.Text;

var services = new ServiceCollection();
services.AddScoped<DocumentConnector>();
services.AddScoped<LolipopService>();

services.AddScoped<労働法Repository>();
services.AddScoped<労働基準法Repository>();
services.AddScoped<労働安全衛生法Repository>();

services.AddScoped<LolipopService>();
var serviceProvider = services.BuildServiceProvider();

var 労働法Service = serviceProvider.GetRequiredService<労働法Repository>();
var 労働基準法Service = serviceProvider.GetRequiredService<労働基準法Repository>();
var 労働安全衛生法Service = serviceProvider.GetRequiredService<労働安全衛生法Repository>();
var lolipopService = serviceProvider.GetRequiredService<LolipopService>();

try
{
    労働法Service.ExportHtml();
    労働基準法Service.ExportHtml();
    労働安全衛生法Service.ExportHtml();
    await lolipopService.UploadDirectoryAsync(@"C:\drive\work\web\Release\exam", "/exam");
    File.WriteAllText(@$"C:\drive\work\solution\PrintSiteBuilder2\LawMp3Generator\bin\Release\net8.0\log\[success]{DateTime.Now.ToString("yyyyMMdd_hhmmss")}.txt", "");

}
catch (Exception ex)
{
    File.WriteAllText(@$"C:\drive\work\solution\PrintSiteBuilder2\LawMp3Generator\bin\Release\net8.0\log\[error]{DateTime.Now.ToString("yyyyMMdd_hhmmss")}.txt", $"Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
}




