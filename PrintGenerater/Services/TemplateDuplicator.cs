using TempriDomain.Interfaces;
using TempriDomain.ValueObject;
using TempriDomain.Config;
namespace PrintGenerater.Services
{
    public class TemplateDuplicator
    {
        IPrint print;
        FolderPathValue folderPathValue;
        public TemplateDuplicator(IPrint print, FolderPathValue folderPathValue)
        {
            this.print = print;
            this.folderPathValue = folderPathValue;
        }
        public void SetPrintDirectory()
        {
            DuplicateTemplate();
        }
        private void DuplicateTemplate()
        {
            var SourceFolder = TempriConstants.TemplateDir;
            var TergetFolder = Path.Combine(TempriConstants.BaseDir, print.PrintId.ToString());
            Directory.CreateDirectory(TergetFolder);
            foreach (var file in Directory.EnumerateFiles(SourceFolder, "*", SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(SourceFolder, file); // Get relative path
                string destFile = Path.Combine(TergetFolder, relativePath); // Construct full destination path
                Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                File.Copy(file, destFile, true);
            }
        }
    }
}
