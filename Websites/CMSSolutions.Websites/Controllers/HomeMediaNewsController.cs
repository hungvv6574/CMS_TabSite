using System;
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
    public class HomeMediaNewsController : BaseHomeController
    {
        private readonly ICategoriesService categoryService;
        private readonly dynamic shapeFactory;
        public HomeMediaNewsController(IWorkContextAccessor workContextAccessor,
            ICategoriesService categoryService, IShapeFactory shapeFactory)
            : base(workContextAccessor)
        {
            PageIndex = 1;
            this.categoryService = categoryService;
            this.shapeFactory = shapeFactory;
        }

        [HttpGet]
        [Url("news/{cateRoot}/{alias}.html")]
        public ActionResult Index(string cateRoot, string alias)
        {
            categoryService.LanguageCode = WorkContext.CurrentCulture;
            var categoryRoot = categoryService.GetByAlias(cateRoot, WorkContext.CurrentCulture);
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

        [HttpGet]
        [Url("news/{cateRoot}/{year}/{alias}.html")]
        public ActionResult MediaCategory(string cateRoot, string alias, int year)
        {
            categoryService.LanguageCode = WorkContext.CurrentCulture;
            var categoryRoot = categoryService.GetByAlias(cateRoot, WorkContext.CurrentCulture);
            var category = categoryService.GetByAlias(alias, WorkContext.CurrentCulture);
            if (category != null && categoryRoot != null)
            {
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoTitle] = category.Name;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoKeywords] = category.Tags;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoDescription] = category.Description;

                BuildPage(category.Id);

                BuildModule(categoryRoot.Id, category.Id, year);
            }

            return View("Index");
        }

        private void BuildModule(int cateRootId, int categoryId, int year = 0)
        {
            if (Request.QueryString["page"] != null)
            {
                PageIndex = int.Parse(Request.QueryString["page"]);
            }
            PageSize = 10;

            var widget = WorkContext.Resolve<IWidgetService>();
            var viewRenderer = new ViewRenderer { Context = ControllerContext };

            var sectionBannerSliderService = WorkContext.Resolve<ISlidersService>();
            var articlesService = WorkContext.Resolve<IArticlesService>();
            articlesService.LanguageCode = WorkContext.CurrentCulture;

            #region SectionBannerSlider
            var modelSectionBannerSlider = new DataViewerModel();
            modelSectionBannerSlider.BannerImages = sectionBannerSliderService.GetRecords(x => x.Type == (int)PageSlider.Media).FirstOrDefault();
            var viewSectionBannerSlider = viewRenderer.RenderPartialView(Extensions.Constants.ViewPageBannerImage, modelSectionBannerSlider);
            WorkContext.Layout.SectionBannerSlider.Add(new MvcHtmlString(viewSectionBannerSlider));
            #endregion

            #region SectionPageContent
            var modelSectionPageContent = new DataViewerModel();
            modelSectionPageContent.CategoryInfo = categoryService.GetByIdCache(cateRootId);
            var category = categoryService.GetByIdCache(categoryId);
            modelSectionPageContent.ListCategories = categoryService.GetChildenByParentId(modelSectionPageContent.CategoryInfo.RefId);
            articlesService.CategoryId = category.RefId;
            modelSectionPageContent.CategoryId = categoryId;
            modelSectionPageContent.ListYear = articlesService.GetAllYear().Select(x => x.Year).ToList();
            if (year == 0 && modelSectionPageContent.ListYear.Count > 0)
            {
                year = modelSectionPageContent.ListYear.FirstOrDefault();
            }
            modelSectionPageContent.Year = year;
            BuildBreadcrumb(modelSectionPageContent, categoryId, year);
            modelSectionPageContent.CategoryChilden = categoryService.GetByIdCache(categoryId);
            var total = 0;
            modelSectionPageContent.PageSize = PageSize;
            modelSectionPageContent.PageIndex = PageIndex;
            modelSectionPageContent.ListArticles = articlesService.GetMediaByCategoryId(year, PageIndex, PageSize, out total);
            modelSectionPageContent.TotalRow = total;
            var viewSectionPageContent = viewRenderer.RenderPartialView(Extensions.Constants.ViewMedia, modelSectionPageContent);
            WorkContext.Layout.SectionPageContent.Add(new MvcHtmlString(viewSectionPageContent));
            #endregion
        }

        [Url("media-details/{alias}.html")]
        public ActionResult MediaDetails(string alias)
        {
            var articlesService = WorkContext.Resolve<IArticlesService>();
            articlesService.LanguageCode = WorkContext.CurrentCulture;
            var articles = articlesService.GetByAlias(alias, WorkContext.CurrentCulture);
            if (articles != null)
            {
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoTitle] = articles.Title;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoKeywords] = articles.Tags;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoDescription] = articles.Description;

                BuildPage(articles.CategoryId);

                ViewMediaDetails(articles);
            }

            return View("Index");
        }

        private void ViewMediaDetails(ArticlesInfo articles)
        {
            var widget = WorkContext.Resolve<IWidgetService>();
            var viewRenderer = new ViewRenderer { Context = ControllerContext };

            var sectionBannerSliderService = WorkContext.Resolve<ISlidersService>();
            var articlesService = WorkContext.Resolve<IArticlesService>();
            articlesService.LanguageCode = WorkContext.CurrentCulture;

            #region SectionBannerSlider
            var modelSectionBannerSlider = new DataViewerModel();
            modelSectionBannerSlider.BannerImages = sectionBannerSliderService.GetRecords(x => x.Type == (int)PageSlider.MediaDetails).FirstOrDefault();
            var viewSectionBannerSlider = viewRenderer.RenderPartialView(Extensions.Constants.ViewPageBannerImage, modelSectionBannerSlider);
            WorkContext.Layout.SectionBannerSlider.Add(new MvcHtmlString(viewSectionBannerSlider));
            #endregion

            #region SectionPageContent
            var modelSectionPageContent = new DataViewerModel();
            var custSetting = WorkContext.Resolve<CustomSettings>();
            modelSectionPageContent.CategoryInfo = categoryService.GetRecords(x => x.RefId == custSetting.CategoryRootMedia && x.LanguageCode == WorkContext.CurrentCulture).FirstOrDefault();
            if (modelSectionPageContent.CategoryInfo != null)
            {
                modelSectionPageContent.ListCategories = categoryService.GetChildenByParentId(modelSectionPageContent.CategoryInfo.RefId);
            }

            var cate = categoryService.GetRecords(x => x.RefId == articles.CategoryId && x.LanguageCode == WorkContext.CurrentCulture).FirstOrDefault();
            if (cate != null)
            {
                articlesService.CategoryId = cate.Id;
                modelSectionPageContent.CategoryId = cate.Id;
                BuildBreadcrumb(modelSectionPageContent, cate.Id, articles.Year);
                modelSectionPageContent.CategoryInfo = cate;
            }
            
            modelSectionPageContent.Year = articles.Year;
            modelSectionPageContent.Articles = articlesService.GetById(articles.Id);
            modelSectionPageContent.ListArticles = articlesService.GetMediaCountTop(articles.Year, articles.Id, 5);
            var viewSectionPageContent = viewRenderer.RenderPartialView(Extensions.Constants.ViewMediaDetails, modelSectionPageContent);
            WorkContext.Layout.SectionPageContent.Add(new MvcHtmlString(viewSectionPageContent));
            #endregion
        }
    }
}
