using PrintSiteBuilder.Interfaces;
using ZXing.Rendering;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;

namespace PrintSiteBuilder.SiteItem
{
    public class barcode
    {
        public void GenerateBarcode(IPrint2 iPrint)
        {
            // バーコードリーダー/ライターオプションの設定
            var barcodeWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = 300,
                    Height = 100,
                    Margin = 10
                },
                Renderer = new PixelDataRenderer() // ここでPixelDataRendererを設定
            };

            // バーコードの生成
            var pixelData = barcodeWriter.Write(iPrint.FnSku);

            // PixelDataをBitmapに変換
            using (var barcodeBitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                var bitmapData = barcodeBitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, barcodeBitmap.PixelFormat);
                try
                {
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    barcodeBitmap.UnlockBits(bitmapData);
                }

                // テキストを追加するために新しいBitmapを作成
                int newHeight = barcodeBitmap.Height + 30; // 30ピクセルの余白を追加
                using (var finalBitmap = new Bitmap(barcodeBitmap.Width, newHeight))
                {
                    using (Graphics g = Graphics.FromImage(finalBitmap))
                    {
                        g.Clear(Color.White);
                        g.DrawImage(barcodeBitmap, 0, 0);
                        using (Font font = new Font("Arial", 16))
                        {
                            SizeF textSize = g.MeasureString(iPrint.FnSku, font);
                            float textX = (finalBitmap.Width - textSize.Width) / 2;
                            float textY = barcodeBitmap.Height + 5; // 5ピクセルのマージンを追加
                            g.DrawString(iPrint.FnSku, font, Brushes.Black, new PointF(textX, textY));
                        }
                    }

                    // 生成されたBitmapを保存
                    finalBitmap.Save($@"C:\drive\work\www\item\print\{iPrint.PrintId}\cover\code128.png", ImageFormat.Png);
                }
            }
        }
    }
}
