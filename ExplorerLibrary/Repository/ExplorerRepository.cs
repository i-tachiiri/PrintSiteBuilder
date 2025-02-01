using BlogDomain.Config;

namespace ExplorerLibrary.Repository
{
    public class ExplorerRepository
    {
        public List<string> GetAllVectors()
        {
            return Directory.GetFiles(DomainConstants.Explorer.BlogPdfFolder).ToList();
        }
    }
}
