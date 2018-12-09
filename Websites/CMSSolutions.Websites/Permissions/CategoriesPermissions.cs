namespace CMSSolutions.Websites.Permissions
{
    using System.Collections.Generic;
    using CMSSolutions.Web.Security.Permissions;
    
    
    public class CategoriesPermissions : IPermissionProvider
    {
        
        public static readonly Permission ManagerCategories = new Permission
        {
            Name = "ManagerCategories", 
            Category = "Management", 
            Description = "Manager Categories", 
        };

        
        public virtual IEnumerable<Permission> GetPermissions()
        {
            yield return ManagerCategories;
        }
    }
}
