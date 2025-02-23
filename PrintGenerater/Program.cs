using Microsoft.Extensions.DependencyInjection;
using PrintGenerater.Factories;
using PrintGenerater.Services;

var services = new ServiceCollection();

services.AddSingleton<PrintFactory>();
services.AddSingleton<PdfGenerator>();
services.AddSingleton<SvgDownloader>();
services.AddSingleton<PrintController>();

var serviceProvider = services.BuildServiceProvider();
var controller = serviceProvider.GetRequiredService<PrintController>();

var PrintId = 100007;
await controller.GeneratePrint(PrintId);


