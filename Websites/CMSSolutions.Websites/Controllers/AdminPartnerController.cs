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

    [Authorize()]
    [Themed(IsDashboard=true)]
    public class AdminPartnerController : BaseAdminController
    {
        public AdminPartnerController(IWorkContextAccessor workContextAccessor) : 
                base(workContextAccessor)
        {
            this.TableName = "tblPartner";
        }
        
        [Url("admin/partners")]
        public ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Quản lý đối tác"), Url = "#" });
            var result = new ControlGridFormResult<PartnerInfo>();
            var siteSettings = WorkContext.Resolve<SiteSettings>();

            result.Title = this.T("Quản lý đối tác");
            result.CssClass = "table table-bordered table-striped";
            result.UpdateActionName = "Update";
            result.IsAjaxSupported = true;
            result.DefaultPageSize = siteSettings.DefaultPageSize;
            result.EnablePaginate = true;
            result.FetchAjaxSource = this.GetPartner;
            result.GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml;
            result.GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml;
            result.ClientId = TableName;
            result.ActionsColumnWidth = 120;

            result.AddColumn(x => x.SortOrder, T("Thứ tự")).HasWidth(80).AlignCenter();
            result.AddColumn(x => x.Logo, T("Logo"))
                .AlignCenter()
                .HasWidth(100)
                .RenderAsImage(y => y.Logo, Extensions.Constants.CssThumbsSize);
            result.AddColumn(x => x.FullName, T("Tên đối tác"));
            result.AddColumn(x => x.PhoneNumber, T("Số điện thoại"));

            result.AddAction().HasText(this.T("Create")).HasUrl(this.Url.Action("Create")).HasButtonStyle(ButtonStyle.Primary).HasBoxButton(false).HasCssClass(Constants.RowLeft).HasRow(true);
            result.AddRowAction().HasText(this.T("Edit")).HasUrl(x => Url.Action("Edit", new { id = x.Id })).HasButtonStyle(ButtonStyle.Default).HasButtonSize(ButtonSize.ExtraSmall);
            result.AddRowAction(true).HasText(this.T("Delete")).HasName("Delete").HasValue(x => Convert.ToString(x.Id)).HasButtonStyle(ButtonStyle.Danger).HasButtonSize(ButtonSize.ExtraSmall).HasConfirmMessage(this.T(Constants.Messages.ConfirmDeleteRecord));

            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");
            return result;
        }
        
        private ControlGridAjaxData<PartnerInfo> GetPartner(ControlGridFormRequest options)
        {
            int totals;
            var service = WorkContext.Resolve<IPartnerService>();
            var items = service.GetRecords(options, out totals, x => x.RefId == 0);
            var result = new ControlGridAjaxData<PartnerInfo>(items, totals);
            return result;
        }

        [Url("admin/categories/create")]
        public ActionResult Create()
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Quản lý đối tác"), Url = "#" });
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Thêm thông tin đối tác"), Url = Url.Action("Index") });
            var model = new PartnerModel();

            var result = new ControlFormResult<PartnerModel>(model);
            result.Title = this.T("Thêm thông tin đối tác");
            result.FormMethod = FormMethod.Post;
            result.UpdateActionName = "Update";
            result.ShowCancelButton = false;
            result.ShowBoxHeader = false;
            result.FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml;
            result.FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml;

            result.RegisterFileUploadOptions("Logo.FileName", new ControlFileUploadOptions
            {
                AllowedExtensions = "jpg,jpeg,png,gif"
            });

            result.AddAction().HasText(this.T("Thêm mới")).HasUrl(this.Url.Action("Create")).HasButtonStyle(ButtonStyle.Success);
            result.AddAction().HasText(this.T("Back")).HasUrl(this.Url.Action("Index")).HasButtonStyle(ButtonStyle.Danger);
            return result;
        }

        [Url("admin/partners/edit/{id}")]
        public ActionResult Edit(int id)
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Quản lý đối tác"), Url = "#" });
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Sửa thông tin đối tác"), Url = Url.Action("Index") });

            var service = WorkContext.Resolve<IPartnerService>();
            var records = service.GetRecords(x => x.Id == id || x.RefId == id);
            PartnerModel model = records.First(x => x.Id == id);
            var modelType = model.GetType();

            var result = new ControlFormResult<PartnerModel>(model);
            result.Title = this.T("Sửa thông tin đối tác");
            result.FormMethod = FormMethod.Post;
            result.Layout = ControlFormLayout.Tab;
            result.UpdateActionName = "Update";
            result.ShowCancelButton = false;
            result.ShowBoxHeader = false;

            result.RegisterFileUploadOptions("Logo.FileName", new ControlFileUploadOptions
            {
                AllowedExtensions = "jpg,jpeg,png,gif"
            });

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
                "Id", "SortOrder", "Logo", "PhoneNumber", "Email", "Website"
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
            return result;
        }
        
        [HttpPost()]
        [FormButton("Save")]
        [ValidateInput(false), Transaction]
        [Url("admin/partners/update")]
        public ActionResult Update(PartnerModel model)
        {
            if (!ModelState.IsValid)
            {
                return new AjaxResult().Alert(T(Constants.Messages.InvalidModel));
            }

            PartnerInfo partnerTemp = null;
            var service = WorkContext.Resolve<IPartnerService>();
            IList<PartnerInfo> records = new List<PartnerInfo> { new PartnerInfo() };
            if (model.Id != 0)
            {
                records = service.GetRecords(x => x.Id == model.Id || x.RefId == model.Id);
                partnerTemp = service.GetRecords(x => x.Id == model.Id).FirstOrDefault();
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
                            languageRecord = new PartnerInfo
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
                record.Logo = model.Logo;
                record.PhoneNumber = model.PhoneNumber;
                record.Website = model.Website;
                record.CreateDate = DateTime.Now.Date;
                record.SortOrder = model.SortOrder;
                record.IsDeleted = false;

                if (string.IsNullOrEmpty(record.LanguageCode))
                {
                    record.ShortName = model.ShortName;
                    record.FullName = model.FullName;
                    record.Address = model.Address;
                    record.Description = model.Description;
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

                    record.ShortName = nameValueCollection.Get("ShortName");
                    record.FullName = nameValueCollection.Get("FullName");
                    record.Address = nameValueCollection.Get("Address");
                    record.Description = nameValueCollection.Get("Description");
                }

                service.Save(record);
            }
            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").Alert(T("Đã cập nhật thành công."));
        }
        
        [ActionName("Update")]
        [FormButton("Delete")]
        public ActionResult Delete(int id)
        {
            var service = WorkContext.Resolve<IPartnerService>();
            var model = service.GetRecords(x => x.RefId == id || x.Id == id);
            service.DeleteMany(model);

            return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE").Alert(T("Đã xóa thành công."));
        }
    }
}
