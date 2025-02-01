

using System;
using System.IO;

namespace CoincheckDomain.Services
{
    public class CoincheckLogger
    {
        private readonly string logDirectoryPath;
        private readonly string logFilePath;

        public CoincheckLogger()
        {
            // 現在の日付を使ってログファイル名を生成
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            logDirectoryPath = $@"C:\task\public_trade\log";//Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
            logFilePath = Path.Combine(logDirectoryPath, $"{timestamp}_log.txt");

            // フォルダとファイルを作成（存在しない場合のみ）
            CreateLogDirectoryAndFile();
        }

        private void CreateLogDirectoryAndFile()
        {
            // フォルダが存在しなければ作成
            if (!Directory.Exists(logDirectoryPath))
            {
                Directory.CreateDirectory(logDirectoryPath);
            }

            // ファイルが存在しなければ作成
            if (!File.Exists(logFilePath))
            {
                using (File.Create(logFilePath)) { }
            }
        }

        public void Log(string message)
        {
            try
            {
                // ログメッセージにタイムスタンプを追加してファイルに書き込み
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]{message}";
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // エラー処理（必要に応じて）
                Console.WriteLine($"ログの書き込みに失敗しました: {ex.Message}");
            }
        }
    }
}

