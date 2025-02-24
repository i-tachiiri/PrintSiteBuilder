using Google.Apis.Slides.v1.Data;
using QRCoder;
using System.Drawing;
using TempriDomain.Interfaces;
namespace PrintGenerater.Services
{
    public class QrGenerator
    {
        public void ExportQrCodes(IPrintEntity print)
        {
            foreach (var page in print.Pages)
            {
                var url = page.GetUrlWithExtention("html","html");
                var exportPath = page.GetFilePathWithExtension("qr","svg");
                var svg = GenerateSvgQrCode(url);
                ExportQrCode(exportPath, svg);
            }

        }
        private string GenerateSvgQrCode(string url)
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new SvgQRCode(qrCodeData);
            int dotSize = 10;
            return qrCode.GetGraphic(dotSize, "#4D4D4D", "transparent", false);
        }

        private void ExportQrCode(string exportPath,string svg)
        {
            File.WriteAllText(exportPath, svg);
        }
    }
}
