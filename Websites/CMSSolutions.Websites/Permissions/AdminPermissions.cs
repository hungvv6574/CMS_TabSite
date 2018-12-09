using System;
using System.Collections.Generic;
using CMSSolutions.Web.Security.Permissions;

namespace CMSSolutions.Websites.Permissions
{
    public class AdminPermissions : IPermissionProvider
    {
        public static readonly Permission ManagerAdmin = new Permission
        {
            Name = "ManagerAdmin",
            Category = "Management",
            Description = "Bảng điều khiển"
        };

        public static readonly Permission ManagerRecruitment = new Permission
        {
            Name = "ManagerRecruitment",
            Category = "Management",
            Description = "Manager Recruitment",

        };
        public IEnumerable<Permission> GetPermissions()
        {
            yield return ManagerAdmin;
            yield return ManagerRecruitment;
        }
    }
}