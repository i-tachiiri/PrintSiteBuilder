using TempriDomain.Entity;
using TempriDomain.Interfaces;

namespace GoogleSlideLibrary.Repository.Print
{
    public class Print100007 : PrintEntity
    {
        public Print100007()
        {
            PresentationId = "1BE4fkQXXKzXHjxSTwmK3MAg9ZBEJ6qB5U0siIP9VoG4";
            PrintId = 100007;
            PagesCount = 20;
            PrintName = "接尾辞";
            Score = 10;
            Pages = new List<IPageEntity>();
        }

        protected override PrintEntity CreateInstanceWithPages(List<IPageEntity> pages)
        {
            return new Print100007
            {
                PresentationId = PresentationId,
                PrintId = PrintId,
                PrintName = PrintName,
                PagesCount = PagesCount,
                Score = Score,
                Pages = pages
            };
        }
    }
}
