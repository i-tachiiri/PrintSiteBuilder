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

//sync page number in slide. print entity refers to page number. 
//create print slides by entity specifications
//download svgs
//transform svgs to pdf
//transform svgs to png or webp(webp is light but png is available more general purpose.)
//generate html or php(To Authorication php is needed but no authorication is one way)
//replace images in cover and export it(this may be ok to operate manually.Operations in amazon are sometimes hard to automate.)
//check all files that means svg,pdf,png exist
//ftp upload

