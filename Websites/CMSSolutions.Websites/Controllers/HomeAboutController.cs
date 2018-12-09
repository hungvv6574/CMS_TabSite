using System.Linq;
using System.Text;
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
    public class HomeAboutController : BaseHomeController
    {
        private readonly ICategoriesService categoryService;
        private readonly dynamic shapeFactory;
        public HomeAboutController(IWorkContextAccessor workContextAccessor, 
            IShapeFactory shapeFactory, 
            ICategoriesService categoryService)
            : base(workContextAccessor)
        {
            this.shapeFactory = shapeFactory;
            this.categoryService = categoryService;
        }

        [Url("we/{cateAlias}.html")]
        public ActionResult Index(string cateAlias)
        {
            var category = categoryService.GetByAlias(cateAlias, WorkContext.CurrentCulture);
            if (category != null)
            {
                ViewData[Extensions.Constants.SeoTitle] = category.Name;
                ViewData[Extensions.Constants.SeoKeywords] = category.Tags;
                ViewData[Extensions.Constants.SeoDescription] = category.Description;

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
            modelSectionBannerSlider.BannerImages = sectionBannerSliderService.GetRecords(x => x.Type == (int)PageSlider.AboutBanner).FirstOrDefault();
            var viewSectionBannerSlider = viewRenderer.RenderPartialView(Extensions.Constants.ViewPageBannerImage, modelSectionBannerSlider);
            WorkContext.Layout.SectionBannerSlider.Add(new MvcHtmlString(viewSectionBannerSlider));
            #endregion

            #region SectionPageContent
            var modelSectionPageContent = new DataViewerModel();
            modelSectionPageContent.CategoryInfo = categoryService.GetByIdCache(id);
            BuildBreadcrumb(modelSectionPageContent);
            modelSectionPageContent.Articles = articlesService.GetByCategoryId(modelSectionPageContent.CategoryInfo.RefId);
            if (modelSectionPageContent.Articles != null)
            {
                modelSectionPageContent.ListImages = imageService.GetRecords(x => x.ArticlesId == modelSectionPageContent.Articles.RefId).ToList();
            }
            var viewSectionPageContent = viewRenderer.RenderPartialView(Extensions.Constants.ViewAboutUs, modelSectionPageContent);
            WorkContext.Layout.SectionPageContent.Add(new MvcHtmlString(viewSectionPageContent));
            #endregion
        }
    }
}
