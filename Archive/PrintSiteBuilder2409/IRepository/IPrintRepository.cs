using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder2409.Entities;

namespace PrintSiteBuilder2409.IRepository
{
    public interface IPrintRepository
    {
        Task<PrintMaster> GetByIdAsync(string PrintId);
    }
}
