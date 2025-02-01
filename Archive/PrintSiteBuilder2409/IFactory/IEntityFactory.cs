using PrintSiteBuilder2409.Entities;
using PrintSiteBuilder2409.Factory;
namespace PrintSiteBuilder2409.IFactory
{
    public interface IEntityFactory 
    { 
        SlideMaster CreateSlideMasterInstance();
        PageMaster CreatePageMasterInstance();
    }
}
