using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder2409.IValidation
{
    public interface IDriveValidation
    {
        public Task<bool> IsPrintFolderCreated(string PrintId);
        public Task<bool> IsFileCreated(string PrintId, string SearchWord);
    }
}
