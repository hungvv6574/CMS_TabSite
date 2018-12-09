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
    public class HomeRecruitmentController : BaseHomeController
    {
        private readonly ICategoriesService categoryService;
        private readonly dynamic shapeFactory;
        public HomeRecruitmentController(IWorkContextAccessor workContextAccessor,
            ICategoriesService categoryService, IShapeFactory shapeFactory)
            : base(workContextAccessor)
        {
            this.categoryService = categoryService;
            this.shapeFactory = shapeFactory;
        }

        [Url("jobs/{cateAlias}.html")]
        public ActionResult Index(string cateAlias)
        {
            categoryService.LanguageCode = WorkContext.CurrentCulture;
            var category = categoryService.GetByAlias(cateAlias, WorkContext.CurrentCulture);
            if (category != null)
            {
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoTitle] = category.Name;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoKeywords] = category.Tags;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoDescription] = category.Description;

                PageSize = 9999;
                PageIndex = 1;

                BuildPage(category.Id);

                BuildModule(category);
            }

            return View("Index");
        }

        private void BuildModule(CategoryInfo cate)
        {

            var widget = WorkContext.Resolve<IWidgetService>();
            var viewRenderer = new ViewRenderer { Context = ControllerContext };

            var sectionBannerSliderService = WorkContext.Resolve<ISlidersService>();
            var recruitmentService = WorkContext.Resolve<IRecruitmentService>();
            recruitmentService.LanguageCode = WorkContext.CurrentCulture;
            recruitmentService.CategoryId = cate.RefId;

            #region SectionBannerSlider
            var modelSectionBannerSlider = new DataViewerModel();
            modelSectionBannerSlider.BannerImages = sectionBannerSliderService.GetRecords(x => x.Type == (int)PageSlider.Recruitment).FirstOrDefault();
            var viewSectionBannerSlider = viewRenderer.RenderPartialView(Extensions.Constants.ViewPageBannerImage, modelSectionBannerSlider);
            WorkContext.Layout.SectionBannerSlider.Add(new MvcHtmlString(viewSectionBannerSlider));
            #endregion

            #region SectionPageContent
            var modelSectionPageContent = new DataViewerModel();
            modelSectionPageContent.CategoryInfo = cate;
            modelSectionPageContent.CategoryId = cate.Id;
            BuildBreadcrumb(modelSectionPageContent, cate.Id);
            var total = 0;
            modelSectionPageContent.PageSize = PageSize;
            modelSectionPageContent.PageIndex = PageIndex;
            modelSectionPageContent.ListRecruitments = recruitmentService.GetByCategoryId(0, PageIndex, PageSize, out total);
            modelSectionPageContent.TotalRow = total;
            var viewSectionPageContent = viewRenderer.RenderPartialView(Extensions.Constants.ViewRecruitment, modelSectionPageContent);
            WorkContext.Layout.SectionPageContent.Add(new MvcHtmlString(viewSectionPageContent));
            #endregion
        }

        [Url("jobs/{cateAlias}/{alias}.html")]
        public ActionResult RecruitmentDetails(string cateAlias, string alias)
        {
            var recruitmentService = WorkContext.Resolve<IRecruitmentService>();
            categoryService.LanguageCode = WorkContext.CurrentCulture;
            var cate = categoryService.GetByAlias(cateAlias, WorkContext.CurrentCulture);
            recruitmentService.LanguageCode = WorkContext.CurrentCulture;
            var recruitment = recruitmentService.GetByAlias(alias, WorkContext.CurrentCulture);
            if (recruitment != null && cate != null)
            {
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoTitle] = recruitment.Title;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoKeywords] = recruitment.Title;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoDescription] = recruitment.Position;

                BuildPage(cate.Id, false);

                ViewMediaDetails(cate, recruitment);
            }

            return View("Index");
        }

        private void ViewMediaDetails(CategoryInfo cate, RecruitmentInfo recruitment)
        {
            var widget = WorkContext.Resolve<IWidgetService>();
            var viewRenderer = new ViewRenderer { Context = ControllerContext };

            var sectionBannerSliderService = WorkContext.Resolve<ISlidersService>();
            var recruitmentService = WorkContext.Resolve<IRecruitmentService>();
            recruitmentService.LanguageCode = WorkContext.CurrentCulture;
            recruitmentService.CategoryId = cate.Id;

            #region SectionBannerSlider
            var modelSectionBannerSlider = new DataViewerModel();
            modelSectionBannerSlider.BannerImages = sectionBannerSliderService.GetRecords(x => x.Type == (int)PageSlider.Recruitment).FirstOrDefault();
            var viewSectionBannerSlider = viewRenderer.RenderPartialView(Extensions.Constants.ViewPageBannerImage, modelSectionBannerSlider);
            WorkContext.Layout.SectionBannerSlider.Add(new MvcHtmlString(viewSectionBannerSlider));
            #endregion

            #region SectionPageContent
            var modelSectionPageContent = new DataViewerModel();
            modelSectionPageContent.CategoryInfo = categoryService.GetByIdCache(recruitment.CategoryId);
            modelSectionPageContent.CategoryId = cate.Id;
            BuildBreadcrumb(modelSectionPageContent, cate.Id);
            var total = 0;
            PageIndex = 1;
            PageSize = 999;
            modelSectionPageContent.PageSize = PageSize;
            modelSectionPageContent.PageIndex = PageIndex;
            modelSectionPageContent.ListRecruitments = recruitmentService.GetByCategoryId(cate.Id, PageIndex, PageSize, out total);
            modelSectionPageContent.TotalRow = total;
            modelSectionPageContent.Recruitments = recruitment;
            var viewSectionPageContent = viewRenderer.RenderPartialView(Extensions.Constants.ViewRecruitmentDetails, modelSectionPageContent);
            WorkContext.Layout.SectionPageContent.Add(new MvcHtmlString(viewSectionPageContent));
            #endregion
        }
    }
}
