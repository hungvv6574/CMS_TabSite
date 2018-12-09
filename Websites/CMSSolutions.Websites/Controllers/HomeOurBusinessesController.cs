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
    public class HomeOurBusinessesController : BaseHomeController
    {
        private readonly ICategoriesService categoryService;
        private readonly dynamic shapeFactory;
        public HomeOurBusinessesController(IWorkContextAccessor workContextAccessor, 
            ICategoriesService categoryService, IShapeFactory shapeFactory)
            : base(workContextAccessor)
        {
            this.categoryService = categoryService;
            this.shapeFactory = shapeFactory;
        }

        [Url("our-businesses.html")]
        public ActionResult Index()
        {
            var settings = WorkContext.Resolve<CustomSettings>();
            var cateId = settings.CategoryBusinesses;
            categoryService.LanguageCode = WorkContext.CurrentCulture;
            var cate = categoryService.GetById(cateId);
            if (cate != null)
            {
                var category = categoryService.GetRecords(x => x.RefId == cate.Id && x.LanguageCode == WorkContext.CurrentCulture).FirstOrDefault();
                if (category != null)
                {
                    ViewData[CMSSolutions.Websites.Extensions.Constants.SeoTitle] = category.Name;
                    ViewData[CMSSolutions.Websites.Extensions.Constants.SeoKeywords] = category.Tags;
                    ViewData[CMSSolutions.Websites.Extensions.Constants.SeoDescription] = category.Description;

                    BuildPage(category.Id);

                    BuildModule(cateId, category.Id);
                }
            }

            return View("Index");
        }

        private void BuildModule(int parentId, int id)
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
            modelSectionPageContent.Articles = articlesService.GetByCategoryId(parentId);
            modelSectionPageContent.ListCategoryImages = new Dictionary<ImageInfo, List<CategoryInfo>>();

            var settingsCust = WorkContext.Resolve<CustomSettings>();
            var listIds = settingsCust.ListCategoriesBusinesses;
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
                    modelSectionPageContent.ListCategoryImages.Add(item, listCates);
                }
            }

            var viewSectionPageContent = viewRenderer.RenderPartialView(Extensions.Constants.ViewOurBusinesses, modelSectionPageContent);
            WorkContext.Layout.SectionPageContent.Add(new MvcHtmlString(viewSectionPageContent));
            #endregion
        }
    }
}
