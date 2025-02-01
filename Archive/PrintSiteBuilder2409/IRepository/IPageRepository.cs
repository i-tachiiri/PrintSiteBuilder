using PrintSiteBuilder2409.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder2409.IRepository
{
    public interface IPageRepository
    {
        Task<PageMaster> GetByKeysAsync(string PrintId, int PageNumber);
        Task SetRecordAsync(PageMaster master);
        public Task<List<PageMaster>> GetByIdAsync(string PrintId);

    }
}
