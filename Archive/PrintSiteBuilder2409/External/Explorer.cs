using PrintSiteBuilder2409.IExternal;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder2409.External
{
    public class Explorer : IExplorer
    {
        public async Task DownloadFromUrl(string Url, string outputPath)
        {
            try
            {
                using (Stream contentStream = await GetContentStreamAsync(Url))
                {
                    await SaveStreamToFileAsync(contentStream, outputPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
        }
        private async Task<Stream> GetContentStreamAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStreamAsync();
            }
        }
        private async Task SaveStreamToFileAsync(Stream contentStream, string outputPath)
        {
            using (FileStream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
            {
                await contentStream.CopyToAsync(fileStream);
            }
        }
        public void CreateQr(string Url,string OutputPath)//string PrintId,int PageNumber,string QrDir,string Uuid)
        {
            var Url = $@"{Config.Constants.Web.RootUrl}/print/{PrintId}/php/{PrintId}-{PageNumber.ToString("D3")}.php?uuid={Uuid}";
            var outputPath = $@"{QrDir}\{itemConfig.ItemKey}.svg";
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(Url, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new SvgQRCode(qrCodeData);
                var dotColor = Color.FromArgb(77, 77, 77); ; // ドットの色を黒に設定
                var backgroundColor = Color.Transparent; // 背景色を白に設定
                int dotSize = 10;
                var qrCodeSvg = qrCode.GetGraphic(dotSize, dotColor, backgroundColor, false);
                File.WriteAllText(outputPath, qrCodeSvg);
            }
        }
    }
}
