using TempriDomain.Interfaces;
using TempriDomain.ValueObject;
using TempriDomain.Config;
namespace PrintGenerater.Services
{
    public class TemplateDuplicator
    {
        public void SetPrintDirectory(IPrintEntity print)
        {
            DuplicateTemplate(print.PrintId.ToString());
        }
        private void DuplicateTemplate(string PrintId)
        {
            var SourceFolder = TempriConstants.TemplateDir;
            var TergetFolder = Path.Combine(TempriConstants.BaseDir, PrintId);
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
