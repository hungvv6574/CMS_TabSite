using System.Linq;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Widgets.Services;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Themes;
using CMSSolutions.Websites.Extensions;
using CMSSolutions.Websites.Models;
using CMSSolutions.Websites.Services;

namespace CMSSolutions.Websites.Controllers
{
    [Themed(IsDashboard = false)]
    public class HomeContactController : BaseHomeController
    {
        private readonly ICategoriesService categoryService;
        public HomeContactController(IWorkContextAccessor workContextAccessor,
            ICategoriesService categoryService) : base(workContextAccessor)
        {
            this.categoryService = categoryService;
        }

        [Url("contact.html")]
        public ActionResult Index()
        {
            var settings = WorkContext.Resolve<CustomSettings>();
            categoryService.LanguageCode = WorkContext.CurrentCulture;
            var category = categoryService.GetRecords(x => x.RefId == settings.CategoryContact && x.LanguageCode == WorkContext.CurrentCulture).FirstOrDefault();
            if (category != null)
            {
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoTitle] = category.Name;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoKeywords] = category.Tags;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoDescription] = category.Description;
                BuildPage(category.Id, false);

                BuildModule(category.Id);

            }

            return View("Index");
        }

        private void BuildModule(int id)
        {
            var widget = WorkContext.Resolve<IWidgetService>();
            var viewRenderer = new ViewRenderer { Context = ControllerContext };

            #region SectionBannerSlider
            var viewSectionBannerSlider = viewRenderer.RenderPartialView(Extensions.Constants.ViewGoogleMap, null);
            WorkContext.Layout.SectionBannerSlider.Add(new MvcHtmlString(viewSectionBannerSlider));
            #endregion

            #region SectionPageContent
            var modelSectionPageContent = new DataViewerModel();
            modelSectionPageContent.CategoryInfo = categoryService.GetByIdCache(id);
            BuildBreadcrumb(modelSectionPageContent);
            var viewSectionPageContent = viewRenderer.RenderPartialView(Extensions.Constants.ViewContact, modelSectionPageContent);
            WorkContext.Layout.SectionPageContent.Add(new MvcHtmlString(viewSectionPageContent));
            #endregion
        }
    }
}
