namespace CMSSolutions.Websites.Permissions
{
    using System.Collections.Generic;
    using CMSSolutions.Web.Security.Permissions;
    
    
    public class PartnerPermissions : IPermissionProvider
    {
        
        public static readonly Permission ManagerPartner = new Permission
        {
            Name = "ManagerPartner", 
            Category = "Management", 
            Description = "Manager Partner", 
        };

        
        public virtual IEnumerable<Permission> GetPermissions()
        {
            yield return ManagerPartner;
        }
    }
}
