using CMSSolutions.Localization;
using CMSSolutions.Web.UI.Navigation;
using CMSSolutions.Websites.Permissions;

namespace CMSSolutions.Websites.Menus
{
    public class NavigationProvider : INavigationProvider
    {
        public Localizer T { get; set; }

        public NavigationProvider()
        {
            T = NullLocalizer.Instance;
        }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Home"), "0", BuildHomeMenu);
            builder.Add(T("Quản lý"), "1", BuildManager);
        }

        private void BuildHomeMenu(NavigationItemBuilder builder)
        {
            builder.IconCssClass("fa-home").Action("Index", "Admin", new { area = "" })
                .Permission(AdminPermissions.ManagerAdmin);
        }

        private void BuildManager(NavigationItemBuilder builder)
        {
            builder.IconCssClass("fa-th");

            builder.Add(T("Chuyên mục"), "0", b => b.Action("Index", "AdminCategories", new { area = "" })
                .Permission(CategoriesPermissions.ManagerCategories));

            builder.Add(T("Đối tác"), "1", b => b.Action("Index", "AdminPartner", new { area = "" })
                .Permission(PartnerPermissions.ManagerPartner));

            builder.Add(T("Banners"), "2", b => b.Action("Index", "AdminSliders", new { area = "" })
                .Permission(SlidersPermissions.ManagerSliders));

            builder.Add(T("Bài viết"), "3", b => b.Action("Index", "AdminArticles", new { area = "" })
                .Permission(ArticlesPermissions.ManagerArticles));

            //builder.Add(T("Ảnh bài viết"), "4", b => b.Action("Index", "AdminImages", new { area = "" })
            //    .Permission(ImagesPermissions.ManagerImages));

            builder.Add(T("Tuyển dụng"), "5", b => b.Action("Index", "AdminRecruitment", new { area = "" })
                .Permission(AdminPermissions.ManagerRecruitment));

            builder.Add(T("Liên hệ"), "6", b => b.Action("Index", "AdminEmails", new { area = "" })
               .Permission(EmailsPermissions.ManagerEmails));
        }
    }
}