namespace CMSSolutions.Websites.Permissions
{
    using System.Collections.Generic;
    using CMSSolutions.Web.Security.Permissions;
    
    
    public class ImagesPermissions : IPermissionProvider
    {
        
        public static readonly Permission ManagerImages = new Permission
        {
            Name = "ManagerImages", 
            Category = "Management", 
            Description = "Manager Images", 
        };

        
        public virtual IEnumerable<Permission> GetPermissions()
        {
            yield return ManagerImages;
        }
    }
}
