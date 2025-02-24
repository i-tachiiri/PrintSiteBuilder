

using TempriDomain.Interfaces;
using TempriDomain.Config;
namespace PrintGenerater.Services
{
    public class HtmlGenerator
    {
        public void GenerateHtml(IPrintEntity print)
        {
            var template = GetTemplate();
            foreach (var page in print.Pages) 
            {
                var html = ReplaceTags(page, template);
                ExportHtml(page, html);
            }
        }
        private string GetTemplate()
        {
            var path = Path.Combine(TempriConstants.TemplateDir,"html","template.html");
            return File.ReadAllText(path);
        }
        private string ReplaceTags(IPageEntity page,string template) 
        {
            var PrintType = page.IsAnswerPage ? "a" : "q";
            return template
                .Replace("{PrintName}", page.PrintEntity.PrintName)
                .Replace("{PrintId}", page.PrintEntity.ToString())
                .Replace("{PrintNumber}", page.PageNumber.ToString())
                .Replace("{PrintType}", PrintType);
        }
        private void ExportHtml(IPageEntity page, string html)
        {
            var PrintType = page.IsAnswerPage ? "a" : "q";
            var fileName = $"{page.PrintEntity.PrintId}-{page.PageNumber}-{PrintType}.html";
            var ExportPath = Path.Combine(TempriConstants.BaseDir, "html", fileName);
            File.WriteAllText(ExportPath,html);
        }
    }
}
