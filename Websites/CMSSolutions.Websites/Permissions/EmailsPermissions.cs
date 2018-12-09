namespace CMSSolutions.Websites.Permissions
{
    using System.Collections.Generic;
    using CMSSolutions.Web.Security.Permissions;
    
    
    public class EmailsPermissions : IPermissionProvider
    {
        
        public static readonly Permission ManagerEmails = new Permission
        {
            Name = "ManagerEmails", 
            Category = "Management", 
            Description = "Manager Emails", 
        };

        
        public virtual IEnumerable<Permission> GetPermissions()
        {
            yield return ManagerEmails;
        }
    }
}
