

using System.Net;
namespace LolipopLibrary
{
    public class LolipopService
    {
        // 非同期でディレクトリをFTPにアップロード
        public async Task UploadDirectoryAsync(string localDirectory, string remoteDirectory)
        {
            // リモートディレクトリを作成
            await CreateFtpDirectoryAsync(remoteDirectory);

            // ローカルディレクトリ内のファイルをアップロード
            foreach (var file in Directory.GetFiles(localDirectory))
            {
                string fileName = Path.GetFileName(file);
                await UploadFileAsync(file, $"{remoteDirectory}/{fileName}");
            }

            // サブディレクトリも再帰的にアップロード
            foreach (var directory in Directory.GetDirectories(localDirectory))
            {
                string dirName = Path.GetFileName(directory);
                await UploadDirectoryAsync(directory, $"{remoteDirectory}/{dirName}");
            }
        }

        // 非同期でファイルをFTPにアップロード
        private async Task UploadFileAsync(string localFilePath, string remoteFilePath)
        {
            var request = (FtpWebRequest)WebRequest.Create($"{LolipopConstants.FtpServer}{remoteFilePath}");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(LolipopConstants.FtpUser, LolipopConstants.FtpPassword);

            // ファイルを読み込み
            byte[] fileContents;
            using (var sourceStream = new StreamReader(localFilePath))
            {
                fileContents = System.Text.Encoding.UTF8.GetBytes(await sourceStream.ReadToEndAsync());
            }

            request.ContentLength = fileContents.Length;

            // ファイルをアップロード
            using (var requestStream = await request.GetRequestStreamAsync())
            {
                await requestStream.WriteAsync(fileContents, 0, fileContents.Length);
            }

            using (var response = (FtpWebResponse)await request.GetResponseAsync())
            {
                //Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
            }
        }

        // 非同期でFTPディレクトリを作成
        private async Task CreateFtpDirectoryAsync(string remoteDirectory)
        {
            var request = (FtpWebRequest)WebRequest.Create($"{LolipopConstants.FtpServer}{remoteDirectory}");
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(LolipopConstants.FtpUser, LolipopConstants.FtpPassword);
            if (remoteDirectory == "/")
            {
                // ルートディレクトリには新しいディレクトリを作成できないため、処理をスキップ
                return;
            }
            try
            {
                using (var response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    //Console.WriteLine($"Create Directory Complete, status {response.StatusDescription}");
                }
            }
            catch (WebException ex)
            {
                var response = (FtpWebResponse)ex.Response;
                if (response.StatusCode != FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    throw;
                }
                // ディレクトリが既に存在している場合は無視
            }
        }
    }
}
