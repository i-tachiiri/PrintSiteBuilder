
using BlogDomain.Config;

namespace BlogGenerator.Services
{
    public class DeployServices
    {
        private string sourceDir = DomainConstants.Explorer.DebugFolder;
        private string targetDir = DomainConstants.Explorer.ReleaseFolder;
        private List<string> excludedDirectories = DomainConstants.Explorer.ExcludeFolders;
        public async Task Sync()
        {
            await SyncDirectoriesAsync(sourceDir, targetDir);
        }
        public async Task DeployTestAssets()
        {
            await SyncDirectoriesAsync(DomainConstants.Explorer.TestAssetsFolder, DomainConstants.Explorer.AssetsFolder);
        }
        public async Task SyncDirectoriesAsync(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(sourceDir))
            {
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDir}");
            }

            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            await SyncFilesAsync(sourceDir, targetDir);
            await SyncSubDirectoriesAsync(sourceDir, targetDir);
        }

        // 非同期でファイルを同期
        private async Task SyncFilesAsync(string sourceDir, string targetDir)
        {
            var sourceFiles = Directory.GetFiles(sourceDir);
            var targetFiles = Directory.GetFiles(targetDir);

            // 新規または更新されたファイルを同期先にコピー
            foreach (var sourceFile in sourceFiles)
            {
                string fileName = Path.GetFileName(sourceFile);
                string targetFile = Path.Combine(targetDir, fileName);

                if (!File.Exists(targetFile) || File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(targetFile))
                {
                    using var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read);
                    using var targetStream = new FileStream(targetFile, FileMode.Create, FileAccess.Write);

                    await sourceStream.CopyToAsync(targetStream);
                }
            }

            // 不要なファイルを削除
            foreach (var targetFile in targetFiles)
            {
                string fileName = Path.GetFileName(targetFile);
                string sourceFile = Path.Combine(sourceDir, fileName);

                if (!File.Exists(sourceFile))
                {
                    File.Delete(targetFile);
                }
            }
        }

        // 非同期でサブディレクトリを同期
        private async Task SyncSubDirectoriesAsync(string sourceDir, string targetDir)
        {
            var sourceSubDirs = Directory.GetDirectories(sourceDir);
            var targetSubDirs = Directory.GetDirectories(targetDir);

            // 新しいサブディレクトリを作成し同期
            foreach (var sourceSubDir in sourceSubDirs)
            {
                string dirName = Path.GetFileName(sourceSubDir);

                // 除外ディレクトリに含まれているか確認
                if (excludedDirectories.Contains(dirName))
                {
                    continue; // このディレクトリはスキップ
                }

                string targetSubDir = Path.Combine(targetDir, dirName);
                await SyncDirectoriesAsync(sourceSubDir, targetSubDir); // サブディレクトリごとに再帰的に呼び出す
            }

            // 同期先に存在して、同期元に存在しないサブディレクトリを削除
            foreach (var targetSubDir in targetSubDirs)
            {
                string dirName = Path.GetFileName(targetSubDir);
                string sourceSubDir = Path.Combine(sourceDir, dirName);

                if (!Directory.Exists(sourceSubDir))
                {
                    Directory.Delete(targetSubDir, true);
                }
            }
        }
    }

}
