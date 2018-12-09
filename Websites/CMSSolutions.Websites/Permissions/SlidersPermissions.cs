namespace CMSSolutions.Websites.Permissions
{
    using System.Collections.Generic;
    using CMSSolutions.Web.Security.Permissions;
    
    
    public class SlidersPermissions : IPermissionProvider
    {
        
        public static readonly Permission ManagerSliders = new Permission
        {
            Name = "ManagerSliders", 
            Category = "Management", 
            Description = "Manager Sliders", 
        };

        
        public virtual IEnumerable<Permission> GetPermissions()
        {
            yield return ManagerSliders;
        }
    }
}
