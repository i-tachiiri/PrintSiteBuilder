using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintSiteBuilder2409.Entities;

namespace PrintSiteBuilder2409.IRepository
{
    public interface ITemplateRepository
    {
        Task<TemplateMaster> GetByIdAsync(string TemplateId);
    }
}
