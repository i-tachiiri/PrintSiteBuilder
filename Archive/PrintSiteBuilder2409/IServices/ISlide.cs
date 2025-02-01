using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintSiteBuilder2409.IServices
{
    public interface ISlide
    {
        public Task CreateSlideMaster(string PrintId);
    }
}
