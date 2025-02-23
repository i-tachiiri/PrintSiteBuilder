using System.Diagnostics;
using System.Text;

namespace ExplorerLibrary.Services
{
    public class BatService
    {
        public void ExecuteBat(string batFilePath)
        {
            if (!File.Exists(batFilePath))
            {
                Console.WriteLine($"[{DateTime.Now:hh:mm:ss}] Error: バッチファイルが見つかりません -> {batFilePath}");
                return;
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = batFilePath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true, // バッチウィンドウを開かない
                WorkingDirectory = Path.GetDirectoryName(batFilePath) // カレントディレクトリをバッチの場所に設定
            };

            using (Process process = new Process { StartInfo = processStartInfo })
            {
                process.OutputDataReceived += (sender, e) => { if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine($"[{DateTime.Now:hh:mm:ss}] {e.Data}"); };
                process.ErrorDataReceived += (sender, e) => { if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine($"[{DateTime.Now:hh:mm:ss}] ERROR: {e.Data}"); };

                try
                {
                    process.Start();
                    process.BeginOutputReadLine(); // 非同期に出力を受け取る
                    process.BeginErrorReadLine(); // 非同期にエラーを受け取る
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.Now:hh:mm:ss}] 実行中にエラーが発生しました: {ex.Message}");
                }
            }
        }


        public void GenerateAndExecuteBat(string batTemplateName,int printId)
        {
            try
            {
                // テンプレートファイルのパス
                string templateFilePath = $@"C:\drive\work\task-schedular\{batTemplateName}";
                if (!File.Exists(templateFilePath))
                {
                    Console.WriteLine($"[{DateTime.Now:hh:mm:ss}] Error: テンプレートバッチファイルが見つかりません -> {templateFilePath}");
                    return;
                }

                // 一時的なバッチファイルのパス
                string tempDir = @"C:\drive\work\task-schedular\temp";
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                string newFilePath = Path.Combine(tempDir, $"{DateTime.Now:yyyyMMddHHmmss}.bat");

                // テンプレートバッチを読み込んでプレースホルダーを置換
                string scriptContent = File.ReadAllText(templateFilePath);
                scriptContent = scriptContent.Replace("{ID}", printId.ToString());
                File.WriteAllText(newFilePath, scriptContent, Encoding.UTF8);

                // バッチを実行（ExecuteBat を呼び出す）
                ExecuteBat(newFilePath);

                // 実行後にバッチファイルを削除
                File.Delete(newFilePath);
                Console.WriteLine($"[{DateTime.Now:hh:mm:ss}] 一時バッチファイル削除: {newFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:hh:mm:ss}] 実行中にエラーが発生しました: {ex.Message}");
            }
        }
    }
}
