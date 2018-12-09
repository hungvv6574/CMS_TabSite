using System.Collections.Generic;
using System.Globalization;
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


    [Authorize()]
    [Themed(IsDashboard=true)]
    public class AdminRecruitmentController : BaseAdminController
    {
        private readonly IRecruitmentService service;
        public AdminRecruitmentController(IWorkContextAccessor workContextAccessor, IRecruitmentService service) : 
                base(workContextAccessor)
        {
            this.service = service;
            this.TableName = "tblRecruitment";
        }
        
        [Url("admin/recruitments")]
        public ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Quản lý tuyển dụng"), Url = "#" });
            var result = new ControlGridFormResult<RecruitmentInfo>();
            var siteSettings = WorkContext.Resolve<SiteSettings>();
            result.Title = this.T("Quản lý tuyển dụng");

            result.CssClass = "table table-bordered table-striped";
            result.UpdateActionName = "Update";
            result.IsAjaxSupported = true;
            result.DefaultPageSize = siteSettings.DefaultPageSize;
            result.EnablePaginate = true;
            result.FetchAjaxSource = this.GetRecruitment;
            result.GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml;
            result.GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml;
            result.ClientId = TableName;
            result.ActionsColumnWidth = 120;

            result.AddColumn(x => x.Id, T("ID")).AlignCenter().HasWidth(100);
            result.AddColumn(x => x.Title, T("Tiêu đề"));
            result.AddColumn(x => x.Position, T("Vị trí tuyển dụng"));
            result.AddColumn(x => x.TimeWork, T("Thời gian làm việc"));
            result.AddColumn(x => x.FinishDate.ToString(Extensions.Constants.DateTimeFomat), T("Hạn nộp hồ sơ"));

            result.AddAction().HasText(this.T("Create")).HasUrl(this.Url.Action("Create")).HasButtonStyle(ButtonStyle.Primary).HasBoxButton(false).HasCssClass(Constants.RowLeft).HasRow(true);
            result.AddRowAction().HasText(this.T("Edit")).HasUrl(x => Url.Action("Edit", new { id = x.Id })).HasButtonStyle(ButtonStyle.Default).HasButtonSize(ButtonSize.ExtraSmall);
            result.AddRowAction(true).HasText(this.T("Delete")).HasName("Delete").HasValue(x => Convert.ToString(x.Id)).HasButtonStyle(ButtonStyle.Danger).HasButtonSize(ButtonSize.ExtraSmall).HasConfirmMessage(this.T(Constants.Messages.ConfirmDeleteRecord));

            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");
            return result;
        }
        
        private ControlGridAjaxData<RecruitmentInfo> GetRecruitment(ControlGridFormRequest options)
        {
            var categoryId = 0;
            var status = -1;
            
            service.CategoryId = categoryId;
            int totals;
            var items = this.service.SearchPaged(status, options.PageIndex, options.PageSize, out totals);

            return new ControlGridAjaxData<RecruitmentInfo>(items, totals);
        }

        [Url("admin/recruitments/create")]
        public ActionResult Create()
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Quản lý tuyển dụng"), Url = "#" });
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Thêm thông tin tuyển dụng"), Url = Url.Action("Index") });
            var model = new RecruitmentModel();

            var result = new ControlFormResult<RecruitmentModel>(model);
            result.Title = this.T("Thêm thông tin tuyển dụng");
            result.FormMethod = FormMethod.Post;
            result.UpdateActionName = "Update";
            result.ShowCancelButton = false;
            result.ShowBoxHeader = false;
            result.FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml;
            result.FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml;

            result.AddAction().HasText(this.T("Thêm mới")).HasUrl(this.Url.Action("Create")).HasButtonStyle(ButtonStyle.Success);
            result.AddAction().HasText(this.T("Back")).HasUrl(this.Url.Action("Index")).HasButtonStyle(ButtonStyle.Danger);

            result.RegisterExternalDataSource(x => x.CategoryId, BindCategories());

            return result;
        }

        [Url("admin/recruitments/edit/{id}")]
        public ActionResult Edit(int id)
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Quản lý tuyển dụng"), Url = "#" });
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Sửa thông tin tuyển dụng"), Url = Url.Action("Index") });

            var records = service.GetRecords(x => x.Id == id || x.RefId == id);
            RecruitmentModel model = records.First(x => x.Id == id);
            var modelType = model.GetType();

            if (string.IsNullOrEmpty(model.Contents) || model.Contents.Trim() == "<br />")
            {
                var viewRenderer = new ViewRenderer { Context = ControllerContext };
                var viewTemplate = viewRenderer.RenderPartialView(string.Format(Extensions.Constants.ViewRecruitmentTemplate, WorkContext.CurrentCulture), null);
                model.Contents = viewTemplate;
            }
            
            var result = new ControlFormResult<RecruitmentModel>(model);
            result.Title = this.T("Sửa thông tin tuyển dụng");
            result.FormMethod = FormMethod.Post;
            result.Layout = ControlFormLayout.Tab;
            result.UpdateActionName = "Update";
            result.ShowCancelButton = false;
            result.ShowBoxHeader = false;

            result.ExcludeProperty(x => x.Alias);
            result.ExcludeProperty(x => x.Contents);
            result.ExcludeProperty(x => x.Position);
            result.ExcludeProperty(x => x.Summary);
            result.ExcludeProperty(x => x.Title);
            result.ExcludeProperty(x => x.TimeWork);

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
                "Id", "CategoryId", "IsDeleted", "FinishDate"
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
            result.AddAction().HasText(this.T("Thêm mới")).HasUrl(this.Url.Action("Create")).HasButtonStyle(ButtonStyle.Success);
            result.AddAction().HasText(this.T("Back")).HasUrl(this.Url.Action("Index")).HasButtonStyle(ButtonStyle.Danger);

            result.RegisterExternalDataSource(x => x.CategoryId, y => BindCategories());

            return result;
        }
        
        [HttpPost()]
        [FormButton("Save")]
        [ValidateInput(false)]
        [Url("admin/recruitments/update")]
        public ActionResult Update(RecruitmentModel model)
        {
            if (!ModelState.IsValid)
            {
                return new AjaxResult().Alert(T(Constants.Messages.InvalidModel));
            }

            RecruitmentInfo recruitmentTemp = null;
            IList<RecruitmentInfo> records = new List<RecruitmentInfo> { new RecruitmentInfo() };
            if (model.Id != 0)
            {
                records = service.GetRecords(x => x.Id == model.Id || x.RefId == model.Id);
                recruitmentTemp = service.GetRecords(x => x.Id == model.Id).FirstOrDefault();
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
                            languageRecord = new RecruitmentInfo
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
                record.FinishDate = DateTime.ParseExact(model.FinishDate, Extensions.Constants.DateTimeFomat, CultureInfo.InvariantCulture);
                record.CreateDate = DateTime.Now.Date;
                record.CreateByUser = WorkContext.CurrentUser.Id;
                record.Company = string.Empty;
                record.ContactName = string.Empty;
                record.ContactAddress = string.Empty;
                record.ContactEmail = string.Empty;
                record.ContactMobile = string.Empty;
                record.IsDeleted = false;

                if (string.IsNullOrEmpty(record.LanguageCode))
                {
                    if (recruitmentTemp == null)
                    {
                        var alias = model.Alias;
                        if (string.IsNullOrEmpty(model.Alias))
                        {
                            alias = Utilities.GetAlias(model.Title);
                        }
                        alias = GetAlias(record.Id, alias, alias, "");

                        record.Title = model.Title;
                        record.Position = model.Position;
                        record.Alias = alias;
                        record.Summary = model.Summary;
                        record.Contents = model.Contents;
                        record.TimeWork = model.TimeWork;
                    }
                    else
                    {
                        record.Title = recruitmentTemp.Title;
                        record.Position = recruitmentTemp.Position;
                        record.Alias = recruitmentTemp.Alias;
                        record.Summary = recruitmentTemp.Summary;
                        record.Contents = recruitmentTemp.Contents;
                        record.TimeWork = recruitmentTemp.TimeWork;
                    }
                }
                else
                {
                    if (languages.Count(x => x.CultureCode.Equals(record.LanguageCode)) == 0)
                    {
                        continue;
                    }

                    var values = Request.Form.AllKeys.ToDictionary(key => key, key => Request.Form[key]);
                    var localizedValues =
                        values.Keys.Where(key => key.Contains("." + record.LanguageCode))
                            .ToDictionary(key => key.Replace("." + record.LanguageCode, ""),
                                key => Utilities.FixCheckboxValue(values[key]));
                    var nameValueCollection = localizedValues.ToNameValueCollection();

                    record.Title = nameValueCollection.Get("Title");

                    var alias = nameValueCollection.Get("Alias");
                    if (string.IsNullOrEmpty(alias))
                    {
                        alias = Utilities.GetAlias(record.Title);
                    }
                    alias = GetAlias(record.Id, alias, alias, record.LanguageCode);
                    record.Alias = alias;
                    record.Position = nameValueCollection.Get("Position");
                    record.Summary = nameValueCollection.Get("Summary");
                    record.Contents = nameValueCollection.Get("Contents");
                    record.TimeWork = nameValueCollection.Get("TimeWork");
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
                        searchService.SearchType = (int)SearchType.Recruitment;
                        if (record.IsDeleted)
                        {
                            if (record.Id > 0)
                            {
                                var obj = searchService.GetByItem(record.Id);
                                searchService.Delete(obj);
                            }
                        }
                        else
                        {
                            serviceCategories.LanguageCode = record.LanguageCode;
                            var cate = serviceCategories.GetRecords(x => x.RefId == record.RefId && x.LanguageCode == record.LanguageCode).FirstOrDefault();
                            var url = string.Empty;
                            if (cate != null)
                            {
                                url = Url.Action("RecruitmentDetails", "HomeRecruitment", new { cateAlias = cate.Alias, alias = record.Alias });
                            }
                            SearchInfo search = (record.Id > 0 ? searchService.GetByItem(record.Id) : new SearchInfo()) ?? new SearchInfo();
                            search.Alias = record.Alias;
                            search.CategoryId = record.CategoryId;
                            search.CreateDate = DateTime.Now;
                            search.Images = Extensions.Constants.ImageDefault;
                            search.LanguageCode = record.LanguageCode;
                            search.SearchId = record.Id.ToString();
                            search.Sumary = record.Summary;
                            search.Tags = record.Title;
                            search.Title = record.Title;
                            search.VideoUrl = string.Empty;
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
            var service = WorkContext.Resolve<IRecruitmentService>();
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
            var model = service.GetRecords(x => x.RefId == id || x.Id == id);
            service.DeleteMany(model);

            return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE").Alert(T("Đã xóa tin đăng tuyển."));
        }
    }
}
