using FikaAmazonAPI.AmazonSpApiSDK.Models.VendorOrders;
using PrintSiteBuilder2409.IExternal;
using PrintSiteBuilder2409.IRepository;
using PrintSiteBuilder2409.IValidation;


namespace PrintSiteBuilder2409.Validation
{
    public class SlideValidation : ISlideValidation
    {
        private readonly ISlideRepository MasterRepository;
        public SlideValidation(ISlideRepository MasterRepository) 
        {
            this.MasterRepository = MasterRepository;
        }
        public async Task<bool> HasRecordByPrintId(string PrintId)
        {
            var QueryResult = await MasterRepository.GetByIdAsync(PrintId);
            return QueryResult != null;
        }
        public async Task<bool> IsRecordSet(string PrintId)
        {
            var QueryResult = await MasterRepository.GetByIdAsync(PrintId);
            return QueryResult != null
                && string.IsNullOrEmpty(QueryResult.Attribute)
                && string.IsNullOrEmpty(QueryResult.FolderId)
                && string.IsNullOrEmpty(QueryResult.PrntSlideId)
                && string.IsNullOrEmpty(QueryResult.CoverSlideId)
                && string.IsNullOrEmpty(QueryResult.AmazonSlideId)
                && string.IsNullOrEmpty(QueryResult.Uuid)
                && QueryResult.PageCount > 0;
        }
    }
}
