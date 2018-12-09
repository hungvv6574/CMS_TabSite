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
    public class HomeSearchResultsController : BaseHomeController
    {
        private readonly ICategoriesService categoryService;
        private readonly dynamic shapeFactory;
        public HomeSearchResultsController(IWorkContextAccessor workContextAccessor, 
            IShapeFactory shapeFactory, ICategoriesService categoryService) 
            : base(workContextAccessor)
        {
            this.shapeFactory = shapeFactory;
            this.categoryService = categoryService;
            PageIndex = 1;
        }

        [HttpGet]
        [Url("search-results.html")]
        public ActionResult Index()
        {
            string keyword = Request.QueryString["keyword"];
            categoryService.LanguageCode = WorkContext.CurrentCulture;
            ViewData[Extensions.Constants.SeoTitle] = T("Từ khóa ") + keyword + T(" trang ") + PageIndex;
            ViewData[Extensions.Constants.SeoKeywords] = "ket qua tim kiem, website tab, tab";
            ViewData[Extensions.Constants.SeoDescription] = T("Từ khóa ") + keyword + T(" trang ") + PageIndex;

            BuildPage(0);

            BuildModule(keyword);

            return View("Index");
        }

        private void BuildModule(string keyword)
        {
            if (Request.QueryString["page"] != null)
            {
                PageIndex = int.Parse(Request.QueryString["page"]);
            }
            PageSize = 10;

            var widget = WorkContext.Resolve<IWidgetService>();
            var viewRenderer = new ViewRenderer { Context = ControllerContext };

            var sectionBannerSliderService = WorkContext.Resolve<ISlidersService>();
            var searchService = WorkContext.Resolve<ISearchService>();
            searchService.LanguageCode = WorkContext.CurrentCulture;

            #region SectionBannerSlider
            var modelSectionBannerSlider = new DataViewerModel();
            modelSectionBannerSlider.BannerImages = sectionBannerSliderService.GetRecords(x => x.Type == (int)PageSlider.Media).FirstOrDefault();
            var viewSectionBannerSlider = viewRenderer.RenderPartialView(Extensions.Constants.ViewPageBannerImage, modelSectionBannerSlider);
            WorkContext.Layout.SectionBannerSlider.Add(new MvcHtmlString(viewSectionBannerSlider));
            #endregion

            #region SectionPageContent
            var modelSectionPageContent = new DataViewerModel();
            BuildBreadcrumb(modelSectionPageContent, -1);
            var condition = new List<SearchCondition>
            {
                new SearchCondition(new[]
                {
                    SearchField.Title.ToString(),
                    SearchField.Keyword.ToString(),
                    SearchField.Sumary.ToString()
                }, keyword)
            };
            var total = 0;
            modelSectionPageContent.PageSize = PageSize;
            modelSectionPageContent.PageIndex = PageIndex;
            modelSectionPageContent.ListSearch = searchService.Search(condition, PageIndex, PageSize, ref total);
            modelSectionPageContent.TotalRow = total;
            modelSectionPageContent.Keyword = keyword;
            var viewSectionPageContent = viewRenderer.RenderPartialView(Extensions.Constants.ViewSearchResults, modelSectionPageContent);
            WorkContext.Layout.SectionPageContent.Add(new MvcHtmlString(viewSectionPageContent));
            #endregion
        }
    }
}
