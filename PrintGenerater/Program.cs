using Microsoft.Extensions.DependencyInjection;
using PrintGenerater.Factories;
using PrintGenerater.Services;

var services = new ServiceCollection();

services.AddSingleton<PrintFactory>();
services.AddSingleton<PdfGenerator>();
services.AddSingleton<SvgDownloader>();
services.AddSingleton<PrintController>();



var serviceProvider = services.BuildServiceProvider();
var pdfGenerator = serviceProvider.GetRequiredService<PdfGenerator>();
var svgDownloader = serviceProvider.GetRequiredService<SvgDownloader>();
