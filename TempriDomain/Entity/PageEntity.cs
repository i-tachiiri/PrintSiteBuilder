namespace TempriDomain.Entity
{
    public class PageEntity
    {
        public string PrintId { get; set; }
        public string PrintType { get; set; }
        public int PrintNumber { get; set; }
        public int PageIndex { get; set; }
        public PrintEntity printEntity { get; set; }
        public List<PageTableEntity> printTableEntity { get; set; }
    }
}
