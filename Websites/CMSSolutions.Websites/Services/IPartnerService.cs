namespace CMSSolutions.Websites.Services
{
    using CMSSolutions;
    using CMSSolutions.Websites.Entities;
    using CMSSolutions.Events;
    using CMSSolutions.Services;
    using CMSSolutions.Data;
    
    
    public interface IPartnerService : IGenericService<PartnerInfo, int>, IDependency
    {
    }
    
    public class PartnerService : GenericService<PartnerInfo, int>, IPartnerService
    {
        
        public PartnerService(IEventBus eventBus, IRepository<PartnerInfo, int> repository) : 
                base(repository, eventBus)
        {

        }
    }
}
