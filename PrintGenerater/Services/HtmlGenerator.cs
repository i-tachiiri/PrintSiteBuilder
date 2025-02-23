

using TempriDomain.Interfaces;
using TempriDomain.Config;
namespace PrintGenerater.Services
{
    public class HtmlGenerator
    {
        IPrint print;
        public HtmlGenerator(IPrint print)
        {
            this.print = print;
        }
        public void GenerateHtml()
        {
            var template = GetTemplate();
            foreach (var page in print.Pages) 
            {
                var html = ReplaceTags(page, template);
                ExportHtml(page, template);
            }
        }
        private string GetTemplate()
        {
            var path = Path.Combine(TempriConstants.TemplateDir,"html","template.html");
            return File.ReadAllText(path);
        }
        private string ReplaceTags(IPage page,string template) 
        {
            var PrintType = page.IsAnswerPage ? "a" : "q";
            return template
                .Replace("{PrintName}", print.PrintName)
                .Replace("{PrintId}", print.PrintId.ToString())
                .Replace("{PrintNumber}", page.PageNumber.ToString())
                .Replace("{PrintType}", PrintType);
        }
        private void ExportHtml(IPage page, string html)
        {
            var PrintType = page.IsAnswerPage ? "a" : "q";
            var fileName = $"{print.PrintId}-{page.PageNumber}-{PrintType}.html";
            var ExportPath = Path.Combine(TempriConstants.BaseDir, "html", fileName);
            File.WriteAllText(ExportPath,html);
        }
    }
}
