using PrintSiteBuilder2409.IFactory;
using PrintSiteBuilder2409.Entities;

namespace PrintSiteBuilder2409.Factory
{
    public class EntityFactory : IEntityFactory 
    { 
        public SlideMaster CreateSlideMasterInstance() { return new SlideMaster(); }
        public PageMaster CreatePageMasterInstance() { return new PageMaster(); }
    }
}
