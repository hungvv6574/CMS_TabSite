using System.Linq;
using System.Web.Mvc;
using CMSSolutions.DisplayManagement;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Themes;
using CMSSolutions.Websites.Extensions;
using CMSSolutions.Websites.Models;
using CMSSolutions.Websites.Services;

namespace CMSSolutions.Websites.Controllers
{
    [Themed(IsDashboard = false)]
    public class HomeFineDiningRestaurantsController : BaseHomeController
    {
        private readonly ICategoriesService categoryService;
        private readonly dynamic shapeFactory;
        public HomeFineDiningRestaurantsController(IWorkContextAccessor workContextAccessor,
            ICategoriesService categoryService, IShapeFactory shapeFactory)
            : base(workContextAccessor)
        {
            this.categoryService = categoryService;
            this.shapeFactory = shapeFactory;
        }

        [Url("our-businesses/{cateRootAlias}.html")]
        public ActionResult Index(string cateRootAlias)
        {
            categoryService.LanguageCode = WorkContext.CurrentCulture;
            var category = categoryService.GetByAlias(cateRootAlias, WorkContext.CurrentCulture);
            if (category != null)
            {
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoTitle] = category.Name;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoKeywords] = category.Tags;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoDescription] = category.Description;

                BuildPage(category.Id);

                BuildModule(category.Id, category.Id);
            }

            return View("Index");
        }

        [Url("our-businesses/{cateRootAlias}/{alias}.html")]
        public ActionResult FDRCategory(string cateRootAlias, string alias)
        {
            categoryService.LanguageCode = WorkContext.CurrentCulture;
            var categoryRoot = categoryService.GetByAlias(cateRootAlias, WorkContext.CurrentCulture);
            var category = categoryService.GetByAlias(alias, WorkContext.CurrentCulture);
            if (category != null && categoryRoot != null)
            {
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoTitle] = category.Name;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoKeywords] = category.Tags;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoDescription] = category.Description;

                BuildPage(category.Id);

                BuildModule(categoryRoot.Id, category.Id);
            }

            return View("Index");
        }

        private void BuildModule(int rootId, int id)
        {
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
            if (rootId == id)
            {
                modelSectionPageContent.ListCategories = categoryService.GetChildenByParentId(categoryId);
                modelSectionPageContent.CategoryId = rootId;
                BuildBreadcrumb(modelSectionPageContent, rootId);
            }
            else
            {
                var cateParent = categoryService.GetByIdCache(rootId);
                modelSectionPageContent.ListCategories = categoryService.GetChildenByParentId(cateParent.RefId);
                modelSectionPageContent.CategoryId = id;
                BuildBreadcrumb(modelSectionPageContent, id);
            }

            modelSectionPageContent.Articles = articlesService.GetByCategoryId(categoryId);
            if (modelSectionPageContent.Articles != null)
            {
                modelSectionPageContent.ListImages = imageService.GetRecords(x => x.ArticlesId == modelSectionPageContent.Articles.RefId).ToList();
            }
            var viewSectionPageContent = viewRenderer.RenderPartialView(Extensions.Constants.ViewFineDiningRestaurants, modelSectionPageContent);
            WorkContext.Layout.SectionPageContent.Add(new MvcHtmlString(viewSectionPageContent));
            #endregion
        }
    }
}
