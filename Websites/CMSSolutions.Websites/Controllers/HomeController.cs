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
    public class HomeController : BaseHomeController
    {
        private readonly dynamic shapeFactory;
        public HomeController(IWorkContextAccessor workContextAccessor, IShapeFactory shapeFactory) 
            : base(workContextAccessor)
        {
            this.shapeFactory = shapeFactory;
        }

        [Url("", Priority = 10)]
        public ActionResult Index()
        {
            var categoryService = WorkContext.Resolve<ICategoriesService>();
            categoryService.LanguageCode = WorkContext.CurrentCulture;
            var categoryHome = categoryService.GetHomePage();
            if (categoryHome != null)
            {
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoTitle] = categoryHome.Name;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoKeywords] = categoryHome.Tags;
                ViewData[CMSSolutions.Websites.Extensions.Constants.SeoDescription] = categoryHome.Description;
            }

            BuildPage(0, true);

            BuildModule();

            return View();
        }

        private void BuildModule()
        {
            var widget = WorkContext.Resolve<IWidgetService>();
            var viewRenderer = new ViewRenderer { Context = ControllerContext };
            var categoryService = WorkContext.Resolve<ICategoriesService>();
            categoryService.LanguageCode = WorkContext.CurrentCulture;

            #region SectionBannerSlider
            var modelSectionBannerSlider = new DataViewerModel();
            var sectionBannerSliderService = WorkContext.Resolve<ISlidersService>();
            modelSectionBannerSlider.ListSliderImages = sectionBannerSliderService.GetRecords(x => x.Type == (int)PageSlider.HomeBannerSlider).OrderBy(x => x.SortOrder).ToList();
            var viewSectionBannerSlider = viewRenderer.RenderPartialView(Extensions.Constants.ViewSectionBannerSlider, modelSectionBannerSlider);
            WorkContext.Layout.SectionBannerSlider.Add(new MvcHtmlString(viewSectionBannerSlider));
            #endregion

            #region SectionAbout

            var settings = WorkContext.Resolve<CustomSettings>();
            var cateAboutUs = categoryService.GetRecords(x => x.LanguageCode == WorkContext.CurrentCulture && x.RefId == settings.CategoryAboutUs).FirstOrDefault();
            var modelSectionAbout = new DataViewerModel();
            if (cateAboutUs != null)
            {
                modelSectionAbout.CategoryInfo = cateAboutUs;
            }
            var viewSectionAbout = viewRenderer.RenderPartialView(Extensions.Constants.ViewSectionAbout, modelSectionAbout);
            WorkContext.Layout.SectionAbout.Add(new MvcHtmlString(viewSectionAbout));
            #endregion

            #region SectionBusinesses
            var modelSectionBusinesses= new DataViewerModel();
            var cateBusinesses = categoryService.GetRecords(x => x.LanguageCode == WorkContext.CurrentCulture && x.RefId == settings.CategoryBusinesses).FirstOrDefault();
            if (cateBusinesses != null)
            {
                modelSectionBusinesses.CategoryInfo = cateBusinesses;
            }
            modelSectionBusinesses.ListCategoryImages = new Dictionary<ImageInfo, List<CategoryInfo>>();

            var settingsCust = WorkContext.Resolve<CustomSettings>();
            var listIds = settingsCust.ListCategoriesBusinesses;
            var imageService = WorkContext.Resolve<IImagesService>();
            var list = imageService.GetCategoriesBusinesses(listIds);

            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(item.ListCategory))
                {
                    continue;
                }
                var cates = categoryService.GetByListId(item.ListCategory, false);
                if (cates != null)
                {
                    var listCates = new List<CategoryInfo>();
                    foreach (var categoryInfo in cates)
                    {
                        var cate = categoryService.GetRecords(x => x.RefId == categoryInfo.Id && x.LanguageCode == WorkContext.CurrentCulture).FirstOrDefault();
                        if (cate != null)
                        {
                            listCates.Add(cate);
                        }
                    }
                    modelSectionBusinesses.ListCategoryImages.Add(item, listCates);
                }
            }
            var viewSectionBusinesses = viewRenderer.RenderPartialView(Extensions.Constants.ViewSectionBusinesses, modelSectionBusinesses);
            WorkContext.Layout.SectionBusinesses.Add(new MvcHtmlString(viewSectionBusinesses));
            #endregion
        }
    }
}
