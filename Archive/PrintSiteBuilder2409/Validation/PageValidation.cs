using PrintSiteBuilder2409.IRepository;
using PrintSiteBuilder2409.IValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder2409.Validation
{
    public class PageValidation : IPageValidation
    {
        private readonly IPageRepository MasterRepository;
        public PageValidation(IPageRepository MasterRepository)
        {
            this.MasterRepository = MasterRepository;
        }
        public async Task<bool> HasRecordByKeys(string PrintId, int PageNumber)
        {
            var QueryResult = await MasterRepository.GetByKeysAsync(PrintId,PageNumber);
            return QueryResult != null;
        }
    }
}
