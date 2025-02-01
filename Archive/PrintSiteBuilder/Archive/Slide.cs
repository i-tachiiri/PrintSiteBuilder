/*using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PrintSiteBuilder.Interfaces;
using PrintSiteBuilder.Models;

namespace PrintSiteBuilder.Utilities
{
    public class Slide
    {
        private int count = 0;
        private string gasAppUrl = "https://script.google.com/macros/s/AKfycbwG38lZ8zP5jftyAYL20qBbtLunnZHlqtY0onLQJ1Zyo-PcxjhvUy6FNY02cP5iQ-OuCQ/exec";
        public async Task GetSvgsFromAllSlides()
        {
            foreach (string key in PrintFactory.ClassNameWithClass.Keys)  //PrintFactoryの各クラスに対して
            {
                Console.WriteLine($@"[{DateTime.Now.ToString("HH:mm:ss")}][{Directory.GetFiles($@"{GlobalConfig.SvgGroupDir}", $"{key}-*").Length}/200][{key}]Expoting SVG ...");

                if (Directory.GetFiles($@"{GlobalConfig.SvgGroupDir}", $"{key}-*").Length >= 200) continue;
                var iPrint = PrintFactory.GetPrintClass(key);
                var slidePages = new SlidePages(iPrint.PresentationId);
                var HeaderConfigs = iPrint.GetHeaderConfigs();
                if (HeaderConfigs.Count == 0) continue;
                foreach(var headerConfig in HeaderConfigs)  //各PrintのConfigに対して
                {
                    count++;
                    Console.WriteLine($@"[{DateTime.Now.ToString("HH:mm:ss")}][{count}/{HeaderConfigs.Count}][{key}]Export Svg...");
                    if (slidePages.presentation.Slides.Count < headerConfig.PageIndex) continue;
                    var Url = $"https://docs.google.com/presentation/d/{iPrint.PresentationId}/export/svg?pageid={slidePages.presentation.Slides[headerConfig.PageIndex].ObjectId}";
                    var OutpuPath = $@"{GlobalConfig.SvgGroupDir}\{headerConfig.PrintName}-{headerConfig.PrintType}.svg";
                    if (File.Exists(OutpuPath)) continue;
                    await ExportSvg(Url, OutpuPath);
                }
                count = 0;
            }
        }
        public async Task GetSvgsFromSlide(string key)
        {
            var iPrint = PrintFactory2.GetPrintClass(key);
            var slidePages = new SlidePages(iPrint.PrintSlideId);
            var PrintConfigs = iPrint.GetPrintConfigs();
            count = 1;
            foreach (var printConfig in PrintConfigs)  //各PrintのConfigに対して
            {
                Console.WriteLine($@"[{DateTime.Now.ToString("HH:mm:ss")}][{count}/{PrintConfigs.Count}]Export Svg...");
                count++;

                var Url = $"https://docs.google.com/presentation/d/{iPrint.PrintSlideId}/export/svg?pageid={slidePages.presentation.Slides[printConfig.headerConfig.PageIndex].ObjectId}";
                var OutpuPath = $@"{GlobalConfig.SvgGroupDir}\{printConfig.headerConfig.PrintName}-{printConfig.headerConfig.PrintType}.svg";
                if (File.Exists(OutpuPath)) continue;
                await ExportSvg(Url, OutpuPath);
            }            
        }

        public async Task ExportSvg(string ExportUrl,string OutputPath)
        {
            if (File.Exists(OutputPath)) return;
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

        public async Task GetSvgFromSlide(Label ProgressLabel, List<List<string>> SlideInfoLists)
        {
            int i = 1;
            foreach (var SlideInfoList in SlideInfoLists)
            {
                ProgressLabel.Text = $"[{i}/{SlideInfoLists.Count()}] SVGをエクスポートしています...";
                ProgressLabel.Update();
                i++;

                string outputPath = $@"{GlobalConfig.SvgGroupDir}\{SlideInfoList[1]}.svg";
                if (File.Exists(outputPath)) continue;

                var SlideId = await GetSplitedSlideId(SlideInfoList);  //GASでスライドを分割→分割後のスライドIDを取得
                string exportUrl = $"https://docs.google.com/presentation/d/{SlideId}/export/svg";
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(exportUrl);
                    response.EnsureSuccessStatusCode();

                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                           fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        await contentStream.CopyToAsync(fileStream);
                    }
                }
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
}*/
