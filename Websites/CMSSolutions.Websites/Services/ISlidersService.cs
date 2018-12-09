namespace CMSSolutions.Websites.Services
{
    using CMSSolutions;
    using CMSSolutions.Websites.Entities;
    using CMSSolutions.Events;
    using CMSSolutions.Services;
    using CMSSolutions.Data;
    
    
    public interface ISlidersService : IGenericService<SliderInfo, int>, IDependency
    {
    }
    
    public class SlidersService : GenericService<SliderInfo, int>, ISlidersService
    {
        
        public SlidersService(IEventBus eventBus, IRepository<SliderInfo, int> repository) : 
                base(repository, eventBus)
        {

        }
    }
}
