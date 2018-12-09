namespace CMSSolutions.Websites.Permissions
{
    using System.Collections.Generic;
    using CMSSolutions.Web.Security.Permissions;
    
    
    public class ArticlesPermissions : IPermissionProvider
    {
        
        public static readonly Permission ManagerArticles = new Permission
        {
            Name = "ManagerArticles", 
            Category = "Management", 
            Description = "Manager Articles", 
        };

        
        public virtual IEnumerable<Permission> GetPermissions()
        {
            yield return ManagerArticles;
        }
    }
}
