using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using ImageMagick;
using PrintSiteBuilder.Models.General;

public class ExportItems
{
    public void ExportWebpFromSvg(string fileName)
    {
        // 3つの異なるサイズを定義
        var sizes = new[]
        {
            new { Size = new MagickGeometry(1280, 0) { IgnoreAspectRatio = false }, Directory = GlobalConfig.WebpDir },
            new { Size = new MagickGeometry(640, 0) { IgnoreAspectRatio = false }, Directory = GlobalConfig.WebpSmallDir },
            new { Size = new MagickGeometry(320, 0) { IgnoreAspectRatio = false }, Directory = GlobalConfig.WebpMobileDir }
        };

        string svgPath = $@"{GlobalConfig.SvgDir}\{fileName}.svg";

        // SVGファイルを1回だけ読み込む
        using (var originalImage = new MagickImage(svgPath))
        {
            foreach (var size in sizes)
            {
                // オリジナル画像からクローンを作成し、サイズごとに処理
                using (var resizedImage = (MagickImage)originalImage.Clone())
                {
                    resizedImage.Resize(size.Size);
                    string outputPath = $@"{size.Directory}\{fileName}.webp";
                    resizedImage.Write(outputPath, MagickFormat.WebP);
                }
            }
        }
    }


    static async Task Main(string SlideId,string itemName)
    {
        string url = $@"https://docs.google.com/presentation/d/{SlideId}/export/svg";
        string savePath = $@"{GlobalConfig.SvgDir}\{itemName}.svg";

        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    // レスポンスの内容（SVGデータ）を取得
                    var svgData = await response.Content.ReadAsByteArrayAsync();
                    // SVGデータをファイルに保存
                    await File.WriteAllBytesAsync(savePath, svgData);
                    Console.WriteLine("SVGファイルを保存しました。");
                }
                else
                {
                    Console.WriteLine("エラー: " + response.StatusCode);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("エラーが発生しました: " + ex.Message);
        }
    }
}
