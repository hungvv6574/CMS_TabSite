using System.Linq;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Widgets.Services;
using CMSSolutions.DisplayManagement;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Themes;
using CMSSolutions.Websites.Extensions;
using CMSSolutions.Websites.Models;
using CMSSolutions.Websites.Services;

namespace CMSSolutions.Websites.Controllers
{
    [Themed(IsDashboard = false)]
    public class HomeOurPartnerController : BaseHomeController
    {
        private readonly ICategoriesService categoryService;
        private readonly dynamic shapeFactory;
        public HomeOurPartnerController(IWorkContextAccessor workContextAccessor,
            ICategoriesService categoryService, IShapeFactory shapeFactory)
            : base(workContextAccessor)
        {
            this.categoryService = categoryService;
            this.shapeFactory = shapeFactory;
        }

        [Url("customers/{cateAlias}.html")]
        public ActionResult Index(string cateAlias)
        {
            categoryService.LanguageCode = WorkContext.CurrentCulture;
            var category = categoryService.GetByAlias(cateAlias, WorkContext.CurrentCulture);
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

            var sectionBannerSliderService = WorkContext.Resolve<ISlidersService>();
            var articlesService = WorkContext.Resolve<IArticlesService>();
            articlesService.LanguageCode = WorkContext.CurrentCulture;
            var partnerService = WorkContext.Resolve<IPartnerService>();

            #region SectionBannerSlider
            var modelSectionBannerSlider = new DataViewerModel();
            modelSectionBannerSlider.BannerImages = sectionBannerSliderService.GetRecords(x => x.Type == (int)PageSlider.Partner).FirstOrDefault();
            var viewSectionBannerSlider = viewRenderer.RenderPartialView(Extensions.Constants.ViewPageBannerImage, modelSectionBannerSlider);
            WorkContext.Layout.SectionBannerSlider.Add(new MvcHtmlString(viewSectionBannerSlider));
            #endregion

            #region SectionPageContent
            var modelSectionPageContent = new DataViewerModel();
            modelSectionPageContent.CategoryInfo = categoryService.GetByIdCache(id);
            BuildBreadcrumb(modelSectionPageContent);
            modelSectionPageContent.Articles = articlesService.GetByCategoryId(modelSectionPageContent.CategoryInfo.RefId);
            modelSectionPageContent.ListPartners = partnerService.GetRecords(x => x.LanguageCode == WorkContext.CurrentCulture && !x.IsDeleted);
            var viewSectionPageContent = viewRenderer.RenderPartialView(Extensions.Constants.ViewOurPartner, modelSectionPageContent);
            WorkContext.Layout.SectionPageContent.Add(new MvcHtmlString(viewSectionPageContent));
            #endregion
        }
    }
}
