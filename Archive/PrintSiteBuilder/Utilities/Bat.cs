using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder.Models.General;


namespace PrintSiteBuilder.Utilities
{
    public class Bat
    {

        public void RunBat(string fileName, bool IsWinScp)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = $@"C:\drive\work\task-schedular\{fileName}";
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.CreateNoWindow = false;
            if (IsWinScp)
            {
                processStartInfo.StandardOutputEncoding = Encoding.UTF8;
                processStartInfo.StandardErrorEncoding = Encoding.UTF8;
            }
            using (Process process = Process.Start(processStartInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                if (!string.IsNullOrEmpty(error))
                {
                    output = error;
                }
                var directory = $@"{GlobalConfig.LogDir}\{fileName}";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText($@"{directory}\{DateTime.Now.ToString("yyyyMMdd-hhmmss")}.txt", output);
            }
        }
        public void RunBat2(string BatName,bool isWinScp, string printId)
        {
            // テンプレートファイルのパス
            string templateFilePath = $@"C:\drive\work\task-schedular\{BatName}";

            // 新しいバッチファイルのパス
            string newFilePath = $@"C:\drive\work\task-schedular\template\{DateTime.Now.ToString("yyyyMMddHHmmss")}.bat";

            // テンプレートファイルを読み込み
            string scriptContent = File.ReadAllText(templateFilePath);

            // プレースホルダーを置換
            scriptContent = scriptContent.Replace("{ID}", printId);

            // 新しいバッチファイルを書き込み
            File.WriteAllText(newFilePath, scriptContent);

            // プロセスの設定
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = newFilePath;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.CreateNoWindow = false;
            if (isWinScp)
            {
                processStartInfo.StandardOutputEncoding = Encoding.UTF8;
                processStartInfo.StandardErrorEncoding = Encoding.UTF8;
            }

            // プロセスの実行
            using (Process process = Process.Start(processStartInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                if (!string.IsNullOrEmpty(error))
                {
                    output = error;
                }

                var directory = $@"{GlobalConfig.LogDir}\{printId}";

            }
        }
    }
}
