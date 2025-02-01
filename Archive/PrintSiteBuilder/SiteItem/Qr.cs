using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRCoder;
using System.Drawing;
using ImageMagick;
using PrintSiteBuilder.Interfaces;

namespace PrintSiteBuilder.SiteItem
{
    public class Qr
    {

        public Bitmap GenerateQRCode(string url)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new QRCode(qrCodeData))
                {
                    var darkGray = Color.FromArgb(77, 77, 77); // RGBで#4d4d4dに相当
                    var qrCodeImage = qrCode.GetGraphic(10, darkGray, Color.Transparent, false);
                    return qrCodeImage;
                }
            }
        }
        public string GenerateQRCodeSvg(string url)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new SvgQRCode(qrCodeData);
                var dotColor = Color.FromArgb(77, 77, 77); ; // ドットの色を黒に設定
                var backgroundColor = Color.Transparent; // 背景色を白に設定
                int dotSize = 10;
                var qrCodeSvg = qrCode.GetGraphic(dotSize, dotColor, backgroundColor, false);
                return qrCodeSvg;
            }
        }

    }
}
