using System.Linq;

namespace CMSSolutions.Websites.Services
{
    using CMSSolutions;
    using CMSSolutions.Websites.Entities;
    using CMSSolutions.Events;
    using CMSSolutions.Services;
    using CMSSolutions.Data;
    
    
    public interface IEmailsService : IGenericService<EmailInfo, int>, IDependency
    {
        bool CheckEmailExist(string email);
    }
    
    public class EmailsService : GenericService<EmailInfo, int>, IEmailsService
    {
        
        public EmailsService(IEventBus eventBus, IRepository<EmailInfo, int> repository) : 
                base(repository, eventBus)
        {
        }

        public bool CheckEmailExist(string email)
        {
            var status = Repository.Table.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
            if (status != null && status.Id > 0)
            {
                return true;
            }

            return false;
        }
    }
}
