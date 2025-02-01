using BlogDomain.Config;
using GoogleTtsLibrary;


namespace BlogGenerator.Services
{
    public class Mp3Generator
    {
        private GoogleTtsService googleTtsService;
        public Mp3Generator(GoogleTtsService googleTtsService)
        {
            this.googleTtsService = googleTtsService;
        }
        public async Task GenerateMp3()
        {
            var TempFiles = Directory.GetDirectories(Path.Combine(DomainConstants.Explorer.BlogTtsFolder, "temp"));
            foreach (var tempFile in TempFiles)
            {
                Directory.Delete(tempFile, true);
            }
            var TextFiles = Directory.GetFiles(DomainConstants.Explorer.BlogTtsFolder, "*.txt", SearchOption.AllDirectories);
            foreach (var TextFile in TextFiles)
            {
                var ExportPath = Path.Combine(DomainConstants.Explorer.BlogTtsFolder, "mp3", $"{Path.GetFileNameWithoutExtension(TextFile)}.mp3");
                if (!File.Exists(ExportPath))
                {
                    await googleTtsService.ExportTextToMp3(TextFile, ExportPath);
                }
            }
        }
    }
}
