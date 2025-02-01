

using TempriDomain.Entity;

namespace TempriDomain.Services
{
    public class SvgGenerator
    {
        public void GroupImage()
        {

        }
        public void GenerateSvg(List<TemplateEntity> Entities,string TemplateFilePath)
        {
            foreach(var entity in Entities) 
            {
                var template = File.ReadAllLines(TemplateFilePath);
                foreach(var line in template)
                {
                    line
                        .Replace("{title}", entity.PrintName)
                        .Replace("{summary}", entity.Summary)
                        .Replace("placeholder", "embed-image");

                }
            }
        }
    }
}
