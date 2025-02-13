namespace TempriDomain.Entity
{
    public class PrintEntity
    {
        public string PresentationId { get; set; }
        public string PrintId {  get; set; }
        public string PrintName { get; set; }

        public int PagesCount { get; set; }
        public int Score { get; set; }

        public List<PageEntity> pageEntityList { get; set; }
    }
}
