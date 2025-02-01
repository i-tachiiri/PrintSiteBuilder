using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder2409.IValidation
{
    public interface IPageValidation
    {
        public Task<bool> HasRecordByKeys(string PrintId,int PageNumber);
        //public Task<bool> IsRecordSet(string PrintId);

    }
}
