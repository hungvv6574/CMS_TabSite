using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CMSSolutions.Collections;
using CMSSolutions.Localization;
using CMSSolutions.Web.Routing;
using CMSSolutions.Websites.Extensions;

namespace CMSSolutions.Websites.Controllers
{
    using System;
    using System.Web.Mvc;
    using CMSSolutions;
    using CMSSolutions.Web.Mvc;
    using CMSSolutions.Web.Themes;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;
    using CMSSolutions.Websites.Models;
    using CMSSolutions.Websites.Services;
    using CMSSolutions.Web;
    using CMSSolutions.Web.UI.Navigation;

    [Authorize]
    [Themed(IsDashboard=true)]
    public class AdminCategoriesController : BaseAdminController
    {
        public AdminCategoriesController(IWorkContextAccessor workContextAccessor) : 
                base(workContextAccessor)
        {
            this.TableName = "tblCategories";
        }
        
        [Url("admin/categories")]
        public ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Quản lý chuyên mục"), Url = "#" });
            var result = new ControlGridFormResult<CategoryInfo>();
            var siteSettings = WorkContext.Resolve<SiteSettings>();

            result.Title = this.T("Quản lý chuyên mục");
            result.CssClass = "table table-bordered table-striped";
            result.UpdateActionName = "Update";
            result.IsAjaxSupported = true;
            result.DefaultPageSize = siteSettings.DefaultPageSize;
            result.EnablePaginate = true;
            result.FetchAjaxSource = GetCategories;
            result.GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml;
            result.GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml;
            result.ClientId = TableName;
            result.ActionsColumnWidth = 180;

            result.AddColumn(x => x.Id, T("Mã"))
                .AlignCenter()
                .HasWidth(60);
            result.AddColumn(x => x.ParentName, T("Chuyên mục cha")).HasWidth(150);
            result.AddColumn(x => x.ShortName, T("Tên chuyên mục")).HasWidth(150);
            result.AddColumn(x => x.Url, T("Đường dẫn Url"));
            result.AddColumn(x => x.IsActived)
                .HasHeaderText(T("Hiển thị"))
                .AlignCenter()
                .HasWidth(100)
                .RenderAsStatusImage();

            result.AddAction().HasText(this.T("Create")).HasUrl(this.Url.Action("Create")).HasButtonStyle(ButtonStyle.Primary).HasBoxButton(false).HasCssClass(Constants.RowLeft).HasRow(true);
            result.AddRowAction().HasText(T("Images")).HasUrl(x => Url.Action("Edit", "AdminImages", new { id = 0, cateId = x.Id, articlesId = 0 })).HasButtonStyle(ButtonStyle.Info).HasButtonSize(ButtonSize.ExtraSmall);
            result.AddRowAction().HasText(this.T("Edit")).HasUrl(x => Url.Action("Edit", new { id = x.Id })).HasButtonStyle(ButtonStyle.Default).HasButtonSize(ButtonSize.ExtraSmall);
            result.AddRowAction(true).HasText(this.T("Delete")).HasName("Delete").HasValue(x => Convert.ToString(x.Id)).HasButtonStyle(ButtonStyle.Danger).HasButtonSize(ButtonSize.ExtraSmall).HasConfirmMessage(this.T(Constants.Messages.ConfirmDeleteRecord));

            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");
            return result;
        }

        private long GetImageId(int categoryId)
        {
            var imageService = WorkContext.Resolve<IImagesService>();
            var images = imageService.GetRecords(x => x.CategoryId == categoryId && x.ArticlesId == 0 && x.LanguageCode == WorkContext.CurrentCulture).FirstOrDefault();
            if (images != null)
            {
                return images.Id;
            }
            return 0;
        }

        private ControlGridAjaxData<CategoryInfo> GetCategories(ControlGridFormRequest options)
        {
            int totals;
            var service = WorkContext.Resolve<ICategoriesService>();

            bool status = false;

            var list = service.GetPaged(status, options.PageIndex, options.PageSize, out totals);
            return new ControlGridAjaxData<CategoryInfo>(list, totals);
        }

        [Url("admin/categories/create")]
        public ActionResult Create()
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Quản lý chuyên mục"), Url = "#" });
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Thêm thông tin chuyên mục"), Url = Url.Action("Index") });
            var model = new CategoriesModel();

            var result = new ControlFormResult<CategoriesModel>(model);
            result.Title = this.T("Thêm thông  chuyên mục");
            result.FormMethod = FormMethod.Post;
            result.UpdateActionName = "Update";
            result.ShowCancelButton = false;
            result.ShowBoxHeader = false;
            result.FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml;
            result.FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml;

            result.RegisterExternalDataSource(x => x.ParentId, BindCategories());

            result.AddAction().HasText(this.T("Thêm mới")).HasUrl(this.Url.Action("Create")).HasButtonStyle(ButtonStyle.Success);
            result.AddAction().HasText(this.T("Back")).HasUrl(this.Url.Action("Index")).HasButtonStyle(ButtonStyle.Danger);

            return result;
        }

        [Url("admin/categories/edit/{id}")]
        public ActionResult Edit(int id)
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Quản lý chuyên mục"), Url = "#" });
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Sửa thông tin chuyên mục"), Url = Url.Action("Index") });

            var service = WorkContext.Resolve<ICategoriesService>();
            var records = service.GetRecords(x => x.Id == id || x.RefId == id);
            CategoriesModel model = records.First(x => x.Id == id);
            var modelType = model.GetType();

            var result = new ControlFormResult<CategoriesModel>(model);
            result.Title = this.T("Sửa thông chuyên mục");
            result.FormMethod = FormMethod.Post;
            result.Layout = ControlFormLayout.Tab;
            result.UpdateActionName = "Update";
            result.ShowCancelButton = false;
            result.ShowBoxHeader = false;

            result.ExcludeProperty(x => x.Alias);
            result.ExcludeProperty(x => x.Name);
            result.ExcludeProperty(x => x.ShortName);
            result.ExcludeProperty(x => x.Url);
            result.ExcludeProperty(x => x.Tags);
            result.ExcludeProperty(x => x.Notes);
            result.ExcludeProperty(x => x.Description);
            result.ExcludeProperty(x => x.Tags);

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
                "Id", "OrderBy", "MenuOrderBy", "ParentId",
                "HasChilden", "IsHome", "IsActived",
                "IsDeleted", "IsDisplayMenu", "IsDisplayFooter"
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
            result.RegisterExternalDataSource(x => x.ParentId, BindCategories());

            result.AddAction().HasText(T("Add Images")).HasUrl(Url.Action("Edit", "AdminImages", new { id = 0, cateId = model.Id, articlesId = 0 })).HasButtonStyle(ButtonStyle.Info);
            result.AddAction().HasText(this.T("Clear")).HasUrl(this.Url.Action("Edit", RouteData.Values.Merge(new { id = 0 }))).HasButtonStyle(ButtonStyle.Success);
            result.AddAction().HasText(this.T("Back")).HasUrl(this.Url.Action("Index")).HasButtonStyle(ButtonStyle.Danger);
            
            return result;
        }

        [HttpPost]
        [Url("admin/categories/get-url")]
        public ActionResult GetCategoriesUrl(int CateId, int Id)
        {
            var service = WorkContext.Resolve<ICategoriesService>();

            var languageManager = WorkContext.Resolve<ILanguageManager>();
            var languages = languageManager.GetActiveLanguages(Constants.ThemeDefault, false);
            var result = new List<SelectListItem>();
            if (languages != null && languages.Count > 0)
            {
                foreach (var language in languages)
                {
                    var child = service.GetRecords(x => x.RefId == Id && x.LanguageCode == language.CultureCode).FirstOrDefault();
                    var record = service.GetRecords(x => x.RefId == CateId && x.LanguageCode == language.CultureCode).FirstOrDefault();
                    if (record != null && child != null)
                    {
                        var item = new SelectListItem();
                        item.Value = "Url_" + language.CultureCode;
                        item.Text = record.Url + "/" + child.Alias;
                        result.Add(item);
                    }
                }
            }

            var recordNone = service.GetById(CateId);
            if (recordNone != null)
            {
                var itemNone = new SelectListItem();
                itemNone.Value = "Url";
                itemNone.Text = recordNone.Url;
                result.Add(itemNone);
            }

            return Json(result);
        }

        [Url("admin/category/get-categories-parent")]
        public ActionResult GetCategoriesParent()
        {
            var languageCode = WorkContext.CurrentCulture;
            if (Utilities.IsNotNull(Request.Form[Extensions.Constants.LanguageCode]))
            {
                languageCode = Request.Form[Extensions.Constants.LanguageCode];
            }

            var service = WorkContext.Resolve<ICategoriesService>();
            service.LanguageCode = languageCode;
            var items = service.GetTree();
            var result = new List<SelectListItem>();

            result.AddRange(items.Select(item => new SelectListItem
            {
                Text = item.ChildenName,
                Value = item.Id.ToString()
            }));

            result.Insert(0, new SelectListItem { Text = T("--- Không chọn ---"), Value = "0" });

            return Json(result);
        }

        [HttpPost()]
        [FormButton("Save")]
        [ValidateInput(false), Transaction]
        [Url("admin/categories/update")]
        public ActionResult Update(CategoriesModel model)
        {
            if (!ModelState.IsValid)
            {
                return new AjaxResult().Alert(T(Constants.Messages.InvalidModel));
            }

            CategoryInfo categoryTemp = null;
            var service = WorkContext.Resolve<ICategoriesService>();
            IList<CategoryInfo> records = new List<CategoryInfo> { new CategoryInfo() };
            if (model.Id != 0)
            {
                records = service.GetRecords(x => x.Id == model.Id || x.RefId == model.Id);
                categoryTemp = service.GetRecords(x => x.Id == model.Id).FirstOrDefault();
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
                            languageRecord = new CategoryInfo
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
                record.ParentId = model.ParentId;
                record.IsHome = model.IsHome;
                record.HasChilden = model.HasChilden;
                record.CreateDate = DateTime.Now.Date;
                record.IsActived = model.IsActived;
                record.OrderBy = model.OrderBy;
                record.MenuOrderBy = model.MenuOrderBy;
                record.IsDisplayMenu = model.IsDisplayMenu;
                record.IsDisplayFooter = model.IsDisplayFooter;
                record.IsDeleted = false;

                if (string.IsNullOrEmpty(record.LanguageCode))
                {
                    if (categoryTemp == null)
                    {
                        var alias = model.Alias;
                        if (!model.IsHome)
                        {
                            if (string.IsNullOrEmpty(model.Alias))
                            {
                                alias = Utilities.GetAlias(model.ShortName);
                            }
                            alias = GetAlias(record.Id, alias, alias, "");
                        }
                        

                        var url = model.Url;
                        if (string.IsNullOrEmpty(url))
                        {
                            url = alias;
                        }

                        record.ShortName = model.ShortName;
                        record.Name = model.Name;
                        record.Alias = alias;
                        record.Notes = model.Notes;
                        record.Description = model.Description;
                        record.Tags = model.Tags;
                        record.Url = url;
                    }
                    else
                    {
                        record.ShortName = categoryTemp.ShortName;
                        record.Name = categoryTemp.Name;
                        record.Alias = categoryTemp.Alias;
                        record.Notes = categoryTemp.Notes;
                        record.Description = categoryTemp.Description;
                        record.Tags = categoryTemp.Tags;
                        record.Url = categoryTemp.Url;
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

                    var shortName = nameValueCollection.Get("ShortName");
                    record.ShortName = shortName;

                    var alias = nameValueCollection.Get("Alias");
                    if (!record.IsHome)
                    {
                        if (string.IsNullOrEmpty(alias))
                        {
                            alias = Utilities.GetAlias(shortName);
                        }
                        alias = GetAlias(record.Id, alias, alias, record.LanguageCode);
                    }
                    record.Alias = alias;

                    record.Name = nameValueCollection.Get("Name");
                    record.Notes = nameValueCollection.Get("Notes");
                    record.Description = nameValueCollection.Get("Description");
                    record.Tags = nameValueCollection.Get("Tags");
                    record.Url = nameValueCollection.Get("Url");
                }

                service.Save(record);
            }

            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").Alert(T("Đã cập nhật thành công."));
        }

        private string GetAlias(int id, string alias, string aliasSource, string languageCode)
        {
            var service = WorkContext.Resolve<ICategoriesService>();
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
            var service = WorkContext.Resolve<ICategoriesService>();
            var model = service.GetRecords(x => x.RefId == id || x.Id == id);
            service.DeleteMany(model);

            return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE").Alert(T("Đã xóa chuyên mục thành công."));
        }
    }
}
