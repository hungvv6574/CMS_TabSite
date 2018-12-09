using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Widgets.Services;
using CMSSolutions.DisplayManagement;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Themes;
using CMSSolutions.Websites.Entities;
using CMSSolutions.Websites.Extensions;
using CMSSolutions.Websites.Models;
using CMSSolutions.Websites.Services;

namespace CMSSolutions.Websites.Controllers
{
    [Themed(IsDashboard = false)]
    public class HomeConsultingServicesController : BaseHomeController
    {
        private readonly ICategoriesService categoryService;
        private readonly dynamic shapeFactory;
        public HomeConsultingServicesController(IWorkContextAccessor workContextAccessor,
            ICategoriesService categoryService, 
            IShapeFactory shapeFactory)
            : base(workContextAccessor)
        {
            this.categoryService = categoryService;
            this.shapeFactory = shapeFactory;
        }

        [Url("our-businesses/training-consultancy.html")]
        public ActionResult Index()
        {
            categoryService.LanguageCode = WorkContext.CurrentCulture;
            var category = categoryService.GetByAlias("training-consultancy", WorkContext.CurrentCulture);
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

        [Url("our-businesses/view-category/{alias}.html")]
        public ActionResult CSCategory(string alias)
        {
            categoryService.LanguageCode = WorkContext.CurrentCulture;
            var category = categoryService.GetByAlias(alias, WorkContext.CurrentCulture);
            if (category != null)
            {
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoTitle] = category.Name;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoKeywords] = category.Tags;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoDescription] = category.Description;

                BuildPage(category.Id);

                BuildModuleCategory(category.Id);
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
            BuildBreadcrumb(modelSectionPageContent);
            modelSectionPageContent.Articles = articlesService.GetByCategoryId(id);
            if (modelSectionPageContent.Articles != null)
            {
                modelSectionPageContent.ListImages = imageService.GetRecords(x => x.ArticlesId == modelSectionPageContent.Articles.RefId).ToList();
            }
            var viewSectionPageContent = viewRenderer.RenderPartialView(Extensions.Constants.ViewConsultingServices, modelSectionPageContent);
            WorkContext.Layout.SectionPageContent.Add(new MvcHtmlString(viewSectionPageContent));
            #endregion
        }

        private void BuildModuleCategory(int id)
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
            modelSectionBannerSlider.ListImages = imageService.GetRecords(x => x.LanguageCode == WorkContext.CurrentCulture && x.CategoryId == id).ToList();
            var viewSectionBannerSlider = viewRenderer.RenderPartialView(Extensions.Constants.ViewPageBannerImage, modelSectionBannerSlider);
            WorkContext.Layout.SectionBannerSlider.Add(new MvcHtmlString(viewSectionBannerSlider));
            #endregion

            #region SectionPageContent
            var modelSectionPageContent = new DataViewerModel();
            modelSectionPageContent.CategoryInfo = categoryService.GetByIdCache(id);
            modelSectionPageContent.CategoryId = id;
            BuildBreadcrumb(modelSectionPageContent, id);

            modelSectionPageContent.Articles = articlesService.GetByCategoryId(id);
            if (modelSectionPageContent.Articles != null)
            {
                modelSectionPageContent.ListImages = imageService.GetRecords(x => x.ArticlesId == modelSectionPageContent.Articles.RefId).ToList();
            }
            var viewSectionPageContent = viewRenderer.RenderPartialView(Extensions.Constants.ViewFranchising, modelSectionPageContent);
            WorkContext.Layout.SectionPageContent.Add(new MvcHtmlString(viewSectionPageContent));
            #endregion
        }
    }
}
