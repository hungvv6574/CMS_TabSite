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
    public class HomeCKCController : BaseHomeController
    {
        private readonly ICategoriesService categoryService;
        private readonly dynamic shapeFactory;
        public HomeCKCController(IWorkContextAccessor workContextAccessor,
            ICategoriesService categoryService, IShapeFactory shapeFactory)
            : base(workContextAccessor)
        {
            this.categoryService = categoryService;
            this.shapeFactory = shapeFactory;
        }

        [Url("our-businesses/ckc.html")]
        public ActionResult Index()
        {
            var id = Utilities.GetCategoryId(CategoriesType.CKC, WorkContext.CurrentCulture);
            categoryService.LanguageCode = WorkContext.CurrentCulture;
            var category = categoryService.GetByIdCache(id);
            if (category != null)
            {
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoTitle] = category.Name;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoKeywords] = category.Tags;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoDescription] = category.Description;

                BuildPage(id);

                BuildModule(id);
            }

            return View("Index");
        }

        [Url("our-businesses/ckc/{alias}.html")]
        public ActionResult CKCCategory(string alias)
        {
            categoryService.LanguageCode = WorkContext.CurrentCulture;
            var category = categoryService.GetByAlias(alias, WorkContext.CurrentCulture);
            if (category != null)
            {
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoTitle] = category.Name;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoKeywords] = category.Tags;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoDescription] = category.Description;

                BuildPage(category.Id);

                BuildModule(category.Id);
            }

            return View("Index");
        }

        private void BuildModule(int id)
        {
            var widget = WorkContext.Resolve<IWidgetService>();
            var viewRenderer = new ViewRenderer { Context = ControllerContext };

            var sectionBannerSliderService = WorkContext.Resolve<ISlidersService>();
            var imageService = WorkContext.Resolve<IImagesService>();
            var articlesService = WorkContext.Resolve<IArticlesService>();
            articlesService.LanguageCode = WorkContext.CurrentCulture;

            #region SectionBannerSlider
            var modelSectionBannerSlider = new DataViewerModel();
            modelSectionBannerSlider.BannerImages = sectionBannerSliderService.GetRecords(x => x.Type == (int)PageSlider.Businesses).FirstOrDefault();
            var viewSectionBannerSlider = viewRenderer.RenderPartialView(Extensions.Constants.ViewPageBannerImage, modelSectionBannerSlider);
            WorkContext.Layout.SectionBannerSlider.Add(new MvcHtmlString(viewSectionBannerSlider));
            #endregion

            #region SectionPageContent
            var modelSectionPageContent = new DataViewerModel();
            modelSectionPageContent.CategoryInfo = categoryService.GetByIdCache(id);
            var categoryId = modelSectionPageContent.CategoryInfo.RefId;
            modelSectionPageContent.CategoryId = id;
            BuildBreadcrumb(modelSectionPageContent, id);

            modelSectionPageContent.Articles = articlesService.GetByCategoryId(categoryId);
            if (modelSectionPageContent.Articles != null)
            {
                modelSectionPageContent.ListImages = imageService.GetRecords(x => x.ArticlesId == modelSectionPageContent.Articles.RefId).ToList();
            }
            var viewSectionPageContent = viewRenderer.RenderPartialView(Extensions.Constants.ViewCKC, modelSectionPageContent);
            WorkContext.Layout.SectionPageContent.Add(new MvcHtmlString(viewSectionPageContent));
            #endregion
        }
    }
}
