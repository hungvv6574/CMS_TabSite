using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using CMSSolutions.Collections;
using CMSSolutions.Localization;
using CMSSolutions.Web.Routing;
using CMSSolutions.Websites.Extensions;
using CMSSolutions.Websites.Services;

namespace CMSSolutions.Websites.Controllers
{
    using System;
    using System.Web.Mvc;
    using CMSSolutions;
    using Web.Mvc;
    using Web.Themes;
    using Web.UI.ControlForms;
    using Entities;
    using Models;
    using Web;
    using Web.UI.Navigation;

    [Authorize]
    [Themed(IsDashboard=true)]
    public class AdminArticlesController : BaseAdminController
    {
        public AdminArticlesController(IWorkContextAccessor workContextAccessor) : 
                base(workContextAccessor)
        {
            this.TableName = "tblArticles";
        }
        
        [Url("admin/articles")]
        public ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Quản lý bài viết"), Url = "#" });
            var result = new ControlGridFormResult<ArticlesInfo>();
            var siteSettings = WorkContext.Resolve<SiteSettings>();

            result.Title = this.T("Quản lý bài viết");
            result.CssClass = "table table-bordered table-striped";
            result.UpdateActionName = "Update";
            result.IsAjaxSupported = true;
            result.DefaultPageSize = siteSettings.DefaultPageSize;
            result.EnablePaginate = true;
            result.FetchAjaxSource = this.GetArticles;
            result.GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml;
            result.GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml;
            result.ClientId = TableName;
            result.ActionsColumnWidth = 180;

            result.AddCustomVar(Extensions.Constants.CategoryId, "$('#" + Extensions.Constants.CategoryId + "').val();", true);
            result.AddCustomVar(Extensions.Constants.SearchText, "$('#" + Extensions.Constants.SearchText + "').val();", true);

            result.AddColumn(x => x.Id, T("ID")).AlignCenter().HasWidth(100);
            result.AddColumn(x => x.Image, T("Ảnh đại diện"))
                .AlignCenter()
                .HasWidth(100)
                .RenderAsImage(y => y.Image, Extensions.Constants.CssThumbsSize);
            result.AddColumn(x => x.Title, T("Tiêu đề"));
            result.AddColumn(x => x.CategoryName, T("Chuyên mục"));
            result.AddColumn(x => x.CreateDate.ToString(Extensions.Constants.DateTimeFomat), T("Ngày tạo"));
            result.AddColumn(x => x.IsPublished)
                .HasHeaderText(T("Đã đăng"))
                .AlignCenter()
                .HasWidth(100)
                .RenderAsStatusImage();

            result.AddRowAction().HasText(T("Images")).HasUrl(x => Url.Action("Edit", "AdminImages", new { id = 0, cateId = 0, articlesId = x.Id })).HasButtonStyle(ButtonStyle.Info).HasButtonSize(ButtonSize.ExtraSmall);
            result.AddAction().HasText(this.T("Create")).HasUrl(this.Url.Action("Create")).HasButtonStyle(ButtonStyle.Primary).HasBoxButton(false).HasCssClass(Constants.RowLeft).HasRow(true);
            result.AddRowAction().HasText(this.T("Edit")).HasUrl(x => Url.Action("Edit", new { id = x.Id })).HasButtonStyle(ButtonStyle.Default).HasButtonSize(ButtonSize.ExtraSmall);
            result.AddRowAction(true).HasText(this.T("Delete")).HasName("Delete").HasValue(x => Convert.ToString(x.Id)).HasButtonStyle(ButtonStyle.Danger).HasButtonSize(ButtonSize.ExtraSmall).HasConfirmMessage(this.T(Constants.Messages.ConfirmDeleteRecord));

            result.AddAction(new ControlFormHtmlAction(() => BuildCategories())).HasParentClass(Constants.ContainerCssClassCol3);
            result.AddAction(new ControlFormHtmlAction(BuildSearchText)).HasParentClass(Constants.ContainerCssClassCol3);

            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");
            return result;
        }
        
        private ControlGridAjaxData<ArticlesInfo> GetArticles(ControlGridFormRequest options)
        {
            var searchText = string.Empty;
            if (Utilities.IsNotNull(Request.Form[Extensions.Constants.SearchText]))
            {
                searchText = Request.Form[Extensions.Constants.SearchText];
            }

            var categoryId = 0;
            if (Utilities.IsNotNull(Request.Form[Extensions.Constants.CategoryId]))
            {
                categoryId = Convert.ToInt32(Request.Form[Extensions.Constants.CategoryId]);
            }

            var status = -1;

            var userId = 0;

            int totals;
            var service = WorkContext.Resolve<IArticlesService>();
            service.CategoryId = categoryId;
            var records = service.SearchPaged(searchText, userId, status, options.PageIndex, options.PageSize, out totals);

            return new ControlGridAjaxData<ArticlesInfo>(records, totals);
        }

        [Url("admin/articles/create")]
        public ActionResult Create()
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Quản lý bài viết"), Url = "#" });
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Thêm thông tin bài viết"), Url = Url.Action("Index") });
            var model = new ArticlesModel();
            
            var result = new ControlFormResult<ArticlesModel>(model);
            result.Title = this.T("Thêm thông tin bài viết");
            result.FormMethod = FormMethod.Post;
            result.UpdateActionName = "Update";
            result.ShowCancelButton = false;
            result.ShowBoxHeader = false;
            result.FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml;
            result.FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml;

            result.RegisterFileUploadOptions("Image.FileName", new ControlFileUploadOptions
            {
                AllowedExtensions = "jpg,jpeg,png,gif"
            });

            result.AddAction().HasText(this.T("Thêm mới")).HasUrl(this.Url.Action("Create")).HasButtonStyle(ButtonStyle.Success);
            result.AddAction().HasText(this.T("Back")).HasUrl(this.Url.Action("Index")).HasButtonStyle(ButtonStyle.Danger);

            result.RegisterExternalDataSource(x => x.CategoryId, BindCategories());

            return result;
        }

        [Url("admin/articles/edit/{id}")]
        public ActionResult Edit(int id)
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Quản lý bài viết"), Url = "#" });
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Sửa thông tin bài viết"), Url = Url.Action("Index") });

            var service = WorkContext.Resolve<IArticlesService>();
            var records = service.GetRecords(x => x.Id == id || x.RefId == id);
            ArticlesModel model = records.First(x => x.Id == id);
            var modelType = model.GetType();

            var result = new ControlFormResult<ArticlesModel>(model);
            result.Title = this.T("Sửa thông tin bài viết");
            result.FormMethod = FormMethod.Post;
            result.Layout = ControlFormLayout.Tab;
            result.UpdateActionName = "Update";
            result.ShowCancelButton = false;
            result.ShowBoxHeader = false;

            result.RegisterFileUploadOptions("Image.FileName", new ControlFileUploadOptions
            {
                AllowedExtensions = "jpg,jpeg,png,gif"
            });

            result.ExcludeProperty(x => x.Alias);
            result.ExcludeProperty(x => x.Contents);
            result.ExcludeProperty(x => x.Description);
            result.ExcludeProperty(x => x.Summary);
            result.ExcludeProperty(x => x.Tags);
            result.ExcludeProperty(x => x.Title);

            var mainTab = result.AddTabbedLayout("Thông tin chính");
            var mainGroup = mainTab.AddGroup();
            var allFields = modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var controlAttributes = new Dictionary<string, ControlFormAttribute>();
            foreach (var propertyInfo in allFields)
            {
                var controlAttribute = propertyInfo.GetCustomAttribute<ControlFormAttribute>(true);
                if (controlAttribute == null) continue;
                mainGroup.Add(propertyInfo.Name);
                controlAttribute.PropertyInfo = propertyInfo;
                controlAttribute.PropertyType = propertyInfo.PropertyType;
                controlAttributes.Add(propertyInfo.Name, controlAttribute);
            }

            var languageManager = WorkContext.Resolve<ILanguageManager>();
            var languages = languageManager.GetActiveLanguages(Constants.ThemeDefault, false);
            var listHidden = new List<string>
            {
                "Id", "CategoryId", "Image", "IsDeleted", "IsPublished", "LanguageCode", "VideoUrl", "Year"
            };

            if (languages.Count > 0)
            {
                foreach (var language in languages)
                {
                    var languageTab = result.AddTabbedLayout(language.Name);
                    var languageGroup = languageTab.AddGroup();
                    var widgetForLanguage = records.FirstOrDefault(x => x.LanguageCode == language.CultureCode) ?? model;

                    foreach (var controlAttribute in controlAttributes)
                    {
                        if (listHidden.Contains(controlAttribute.Key))
                        {
                            continue;
                        }

                        var key = controlAttribute.Key + "." + language.CultureCode;
                        var value = controlAttribute.Value.PropertyInfo.GetValue(widgetForLanguage);
                        result.AddProperty(key, controlAttribute.Value.ShallowCopy(), value);
                        languageGroup.Add(key);
                    }
                }
            }

            result.AddAction().HasText(T("Add Images")).HasUrl(Url.Action("Edit", "AdminImages", new { id = 0, cateId = 0, articlesId = model.Id })).HasButtonStyle(ButtonStyle.Info);
            result.AddAction().HasText(this.T("Thêm mới")).HasUrl(this.Url.Action("Create")).HasButtonStyle(ButtonStyle.Success);
            result.AddAction().HasText(this.T("Back")).HasUrl(this.Url.Action("Index")).HasButtonStyle(ButtonStyle.Danger);

            result.RegisterExternalDataSource(x => x.CategoryId, BindCategories());

            return result;
        }

        private long GetImageId(int articlesId)
        {
            var imageService = WorkContext.Resolve<IImagesService>();
            var images = imageService.GetRecords(x => x.ArticlesId == articlesId && x.LanguageCode == WorkContext.CurrentCulture).FirstOrDefault();
            if (images != null)
            {
                return images.Id;
            }
            return 0;
        }

        [HttpPost()]
        [FormButton("Save")]
        [ValidateInput(false), Transaction]
        [Url("admin/articles/update")]
        public ActionResult Update(ArticlesModel model)
        {
            if (!ModelState.IsValid)
            {
                return new AjaxResult().Alert(T(Constants.Messages.InvalidModel));
            }

            ArticlesInfo articlesTemp = null;
            var service = WorkContext.Resolve<IArticlesService>();
            service.CategoryId = model.CategoryId;
            IList<ArticlesInfo> records = new List<ArticlesInfo> { new ArticlesInfo() };
            if (model.Id != 0)
            {
                records = service.GetRecords(x => x.Id == model.Id || x.RefId == model.Id);
                articlesTemp = service.GetRecords(x => x.Id == model.Id).FirstOrDefault();
            }

            var languageManager = WorkContext.Resolve<ILanguageManager>();
            var languages = languageManager.GetActiveLanguages(Constants.ThemeDefault, false);

            if (model.Id != 0)
            {
                if (languages.Count > 0)
                {
                    foreach (var language in languages)
                    {
                        var languageRecord = records.FirstOrDefault(x => x.LanguageCode == language.CultureCode);
                        if (languageRecord == null)
                        {
                            languageRecord = new ArticlesInfo
                            {
                                LanguageCode = language.CultureCode,
                                RefId = model.Id
                            };
                            records.Add(languageRecord);
                        }
                    }
                }
            }

            foreach (var record in records)
            {
                record.CategoryId = model.CategoryId;
                record.CreateDate = DateTime.Now.Date;
                record.CreateByUser = WorkContext.CurrentUser.Id;
                record.IsPublished = model.IsPublished;
                record.PublishedDate = DateTime.Now;
                record.Year = model.Year;
                record.StartDate = DateTime.Now.Date;
                record.EndDate = DateTime.Now.AddYears(10);
                record.VideoUrl = model.VideoUrl;
                record.Image = model.Image;
                if (string.IsNullOrEmpty(model.Image) || model.Image == "#" || model.Image == "/")
                {
                    record.Image = Extensions.Constants.ImageDefault;
                }
                record.ViewCount = model.ViewCount;
                record.IsDeleted = false;

                if (string.IsNullOrEmpty(record.LanguageCode))
                {
                    if (articlesTemp == null)
                    {
                        var alias = model.Alias;
                        if (string.IsNullOrEmpty(model.Alias))
                        {
                            alias = Utilities.GetAlias(model.Title);
                        }
                        alias = GetAlias(record.Id, alias, alias, "");

                        record.Alias = alias;
                        record.Title = model.Title;
                        record.Summary = model.Summary;
                        record.Contents = model.Contents;
                        record.Description = model.Description;
                        record.Tags = model.Tags;
                    }
                    else
                    {
                        record.Alias = articlesTemp.Alias;
                        record.Title = articlesTemp.Title;
                        record.Summary = articlesTemp.Summary;
                        record.Contents = articlesTemp.Contents;
                        record.Description = articlesTemp.Description;
                        record.Tags = articlesTemp.Tags;
                    }
                }
                else
                {
                    if (languages.Count(x => x.CultureCode.Equals(record.LanguageCode)) == 0)
                    {
                        continue;
                    }

                    var values = Request.Form.AllKeys.ToDictionary(key => key, key => Request.Form[key]);
                    var localizedValues = values.Keys.Where(key => key.Contains("." + record.LanguageCode)).ToDictionary(key => key.Replace("." + record.LanguageCode, ""), key => Utilities.FixCheckboxValue(values[key]));
                    var nameValueCollection = localizedValues.ToNameValueCollection();

                    var title = nameValueCollection.Get("Title");
                    record.Title = title;

                    var alias = nameValueCollection.Get("Alias");
                    if (string.IsNullOrEmpty(alias))
                    {
                        alias = Utilities.GetAlias(title);
                    }
                    alias = GetAlias(record.RefId, alias, alias, record.LanguageCode);
                    record.Alias = alias;
                    record.Title = nameValueCollection.Get("Title");
                    record.Summary = nameValueCollection.Get("Summary");
                    record.Contents = nameValueCollection.Get("Contents");
                    record.Description = nameValueCollection.Get("Description");
                    record.Tags = nameValueCollection.Get("Tags");
                }

                service.Save(record);

                try
                {
                    if (!string.IsNullOrEmpty(record.LanguageCode))
                    {
                        var serviceCategories = WorkContext.Resolve<ICategoriesService>();
                        serviceCategories.LanguageCode = record.LanguageCode;
                        var searchService = WorkContext.Resolve<ISearchService>();
                        searchService.LanguageCode = record.LanguageCode;
                        searchService.SearchType = (int)SearchType.Articles;
                        if (record.IsDeleted || !record.IsPublished)
                        {
                            if (record.Id > 0)
                            {
                                var obj = searchService.GetByItem(record.Id);
                                if (obj != null)
                                {
                                    searchService.Delete(obj);
                                }
                            }
                        }
                        else
                        {
                            var cate = serviceCategories.GetByIdCache(record.CategoryId);
                            var parentId = Utilities.GetCategoryId(CategoriesType.News, record.LanguageCode);
                            var url = string.Empty;
                            if (cate != null)
                            {
                                if (cate.ParentId == parentId || cate.Id == parentId)
                                {
                                    url = Url.Action("MediaDetails", "HomeMediaNews", new { alias = record.Alias });
                                }
                                else
                                {
                                    if (cate.ParentId == 0)
                                    {
                                        url = string.Format("/{0}/{1}.html", cate.Alias, record.Alias);
                                    }
                                    else
                                    {
                                        var cateParent = serviceCategories.GetByIdCache(cate.ParentId);
                                        if (cateParent != null)
                                        {
                                            url = string.Format("/{0}/{1}/{2}.html", cateParent.Alias, cate.Alias, record.Alias);
                                        }
                                    }
                                }
                            }

                            SearchInfo search = (record.Id > 0 ? searchService.GetByItem(record.Id) : new SearchInfo()) ?? new SearchInfo();
                            search.Alias = record.Alias;
                            search.CategoryId = record.CategoryId;
                            search.CreateDate = DateTime.Now;
                            search.Images = record.Image;
                            search.LanguageCode = record.LanguageCode;
                            search.SearchId = record.Id.ToString();
                            search.Sumary = record.Summary;
                            search.Tags = record.Tags;
                            search.Title = record.Title;
                            search.VideoUrl = record.VideoUrl;
                            search.Type = (int)SearchType.Articles;
                            search.Url = url;
                            searchService.Save(search);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").Alert(ex.Message);
                }
            }

            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").Alert(T("Đã cập nhật thành công."));
        }

        private string GetAlias(int id, string alias, string aliasSource, string languageCode)
        {
            var service = WorkContext.Resolve<IArticlesService>();
            if (service.CheckAlias(id, alias))
            {
                if (!string.IsNullOrEmpty(languageCode))
                {
                    alias = aliasSource + "-" + languageCode;
                }

                return alias;
            }

            return alias;
        }

        [ActionName("Update")]
        [FormButton("Delete")]
        public ActionResult Delete(int id)
        {
            var service = WorkContext.Resolve<IArticlesService>();
            var model = service.GetRecords(x => x.RefId == id || x.Id == id);
            service.DeleteMany(model);

            return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE").Alert(T("Đã xóa bài viết thành công."));
        }
    }
}
