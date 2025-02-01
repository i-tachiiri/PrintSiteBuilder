using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder2409.IValidation
{
    public interface ISlideValidation
    {
        public Task<bool> HasRecordByPrintId(string PrintId);      
        public Task<bool> IsRecordSet(string PrintId);

    }
}
