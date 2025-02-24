using GoogleSlideLibrary.Services;
using TempriDomain.Entity;


namespace GoogleSlideLibrary.Repository.Page
{
    public class Page100007 : PageEntity
    {
        public Page100007(PrintEntity printEntity, string pageObjectId, int pageNumber, int pageIndex, bool isAnswerPage)
            : base(printEntity, pageObjectId, pageNumber, pageIndex, isAnswerPage)
        {
        }
    }
}
