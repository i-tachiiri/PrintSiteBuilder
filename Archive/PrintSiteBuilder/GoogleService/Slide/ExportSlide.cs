using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Slides.v1.Data;
using Newtonsoft.Json;
using PrintSiteBuilder.Interfaces;
using PrintSiteBuilder.Models.General;
using PrintSiteBuilder.Models.Print;
using PrintSiteBuilder.Utilities;
using PrintSiteBuilder.GoogleService.Drive;

namespace PrintSiteBuilder.GoogleService.Slide
{
    public class ExportSlide
    {
        private int count = 0;
        private string gasAppUrl = "https://script.google.com/macros/s/AKfycbwG38lZ8zP5jftyAYL20qBbtLunnZHlqtY0onLQJ1Zyo-PcxjhvUy6FNY02cP5iQ-OuCQ/exec";
        public async Task GetSvgsFromAllSlides()  //全プリントに対するエクスポート。通常使わない
        {
            foreach (string key in PrintFactory.ClassNameWithClass.Keys)  //PrintFactoryの各クラスに対して
            {
                Console.WriteLine($@"[{DateTime.Now.ToString("HH:mm:ss")}][{Directory.GetFiles($@"{GlobalConfig.SvgGroupDir}", $"{key}-*").Length}/200][{key}]Expoting SVG ...");


                if (Directory.GetFiles($@"{GlobalConfig.SvgGroupDir}", $"{key}-*").Length >= 200) continue;
                var iPrint = await PrintFactory2.GetPrintClass(key);
                var slidePages = new SlidePages(iPrint.PrintSlideId);
                var HeaderConfigs = iPrint.PrintType.GetHeaderConfigs();
                if (HeaderConfigs.Count == 0) continue;
                var drive = new GoogleDrive();
                drive.PermitReadToPublic(iPrint.PrintSlideId);
                foreach (var headerConfig in HeaderConfigs)  //各PrintのConfigに対して
                {
                    count++;
                    Console.WriteLine($@"[{DateTime.Now.ToString("HH:mm:ss")}][{count}/{HeaderConfigs.Count}][{key}]Export Svg...");
                    if (slidePages.presentation.Slides.Count < headerConfig.PageIndex) continue;
                    var Url = $"https://docs.google.com/presentation/d/{iPrint.PrintSlideId}/export/svg?pageid={slidePages.presentation.Slides[headerConfig.PageIndex].ObjectId}";
                    var OutpuPath = $@"{GlobalConfig.SvgGroupDir}\{headerConfig.PrintName}-{headerConfig.PrintType}.svg";
                    if (System.IO.File.Exists(OutpuPath)) continue;
                    await ExportImage(Url, OutpuPath, iPrint.PrintSlideId);
                }
                drive.DenyPublicAccess(iPrint.PrintSlideId);
                count = 0;
            }
        }
        public async Task ExportPrintImages(IPrint2 iPrint, string Extention)
        {
            var slidePages = new SlidePages(iPrint.PrintSlideId);
            var PrintConfigs = iPrint.PrintType.GetPrintConfigs();
            count = 1;
            var drive = new GoogleDrive();
            drive.PermitReadToPublic(iPrint.PrintSlideId);
            foreach (var printConfig in PrintConfigs)  //各PrintのConfigに対して
            {
                Console.WriteLine($@"[{DateTime.Now.ToString("HH:mm:ss")}][{count}/{PrintConfigs.Count}]Export {Extention}...");
                count++;

                var Url = $"https://docs.google.com/presentation/d/{iPrint.PrintSlideId}/export/{Extention}?pageid={slidePages.presentation.Slides[printConfig.headerConfig.PageIndex].ObjectId}";
                var printType = printConfig.headerConfig.PrintType == "問題" ? "q" : "a";
                var OutpuPath = $@"{iPrint.path.PrintSlideDir}\{printConfig.PrintId}-{printConfig.headerConfig.PrintNumber.ToString("D3")}-{printType}.{Extention}";
                await ExportImage(Url, OutpuPath, iPrint.PrintSlideId);
            }
            drive.DenyPublicAccess(iPrint.PrintSlideId);
        }
        public async Task ExportAmazonImages(IPrint2 iPrint, string Extention)
        {
            var slidePages = new SlidePages(iPrint.AmazonSlideId);
            var drive = new GoogleDrive();
            drive.PermitReadToPublic(iPrint.AmazonSlideId);
            for (var i = 0; i < slidePages.presentation.Slides.Count; i++)  //各PrintのConfigに対して
            {
                Console.WriteLine($@"[{DateTime.Now.ToString("HH:mm:ss")}][{i}/{slidePages.presentation.Slides.Count}]Export {Extention}...");
                var Url = $"https://docs.google.com/presentation/d/{iPrint.AmazonSlideId}/export/{Extention}?pageid={slidePages.presentation.Slides[i].ObjectId}";
                var OutpuPath = $@"{iPrint.path.PrintAmazonDir}\{iPrint.PrintId}-amazon-{i.ToString("D3")}.{Extention}";
                await ExportImage(Url, OutpuPath, iPrint.PrintSlideId);
            }
            drive.DenyPublicAccess(iPrint.PrintSlideId);
        }
        public async Task ExportCoverPdf(Presentation presentation, IPrint2 iPrint)
        {
            var drive = new GoogleDrive();
            drive.PermitReadToPublic(iPrint.PrintSlideId);
            var Url = $"https://docs.google.com/presentation/d/{presentation.PresentationId}/export/pdf";
            var OutputPath = $@"{iPrint.path.PrintCoverDir}\{iPrint.PrintId}-cover.pdf";
            await ExportImage(Url, OutputPath, iPrint.PrintSlideId);
            drive.DenyPublicAccess(iPrint.PrintSlideId);

        }

        public async Task ExportImage(string ExportUrl, string OutputPath, string SlideId)
        {
            try
            {

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(ExportUrl);
                    response.EnsureSuccessStatusCode();

                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                           fileStream = new FileStream(OutputPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        await contentStream.CopyToAsync(fileStream);
                    }
                }

            }
            catch (Exception ex)
            {
                return;
            }

        }


        public async Task<List<string>> GetSplitedSlideIds(List<List<string>> SlideInfoLists)
        {
            var SplitedSlideIds = new List<string>();
            foreach (var SlideInfo in SlideInfoLists)
            {
                var SlideId = await GetSplitedSlideId(SlideInfo);
                SplitedSlideIds.Add(SlideId);
            }
            return SplitedSlideIds;
        }
        public async Task<string> GetSplitedSlideId(List<string> SlideInfoList)
        {
            var data = new
            {
                pageNumber = SlideInfoList[0],
                fileName = SlideInfoList[1],
                slideId = SlideInfoList[2]
            };

            string json = JsonConvert.SerializeObject(data);

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(400);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(gasAppUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                else
                {
                    return $"Error: {response.ReasonPhrase}";
                }
            }
        }
    }
}
