using CMSSolutions.Web.Routing;
using CMSSolutions.Websites.Services;

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
    using CMSSolutions.Web;
    using CMSSolutions.Web.UI.Navigation;

    [Authorize()]
    [Themed(IsDashboard=true)]
    public class AdminEmailsController : BaseAdminController
    {
        public AdminEmailsController(IWorkContextAccessor workContextAccessor) : 
                base(workContextAccessor)
        {
            this.TableName = "tblEmails";
        }
        
        [Url("admin/emails")]
        public ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Quản lý thông tin liên hệ"), Url = "#" });
            var result = new ControlGridFormResult<EmailInfo>();
            var siteSettings = WorkContext.Resolve<SiteSettings>();

            result.Title = this.T("Quản lý thông tin liên hệ");
            result.CssClass = "table table-bordered table-striped";
            result.UpdateActionName = "Update";
            result.IsAjaxSupported = true;
            result.DefaultPageSize = siteSettings.DefaultPageSize;
            result.EnablePaginate = true;
            result.FetchAjaxSource = this.GetEmails;
            result.GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml;
            result.GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml;
            result.ClientId = TableName;
            result.ActionsColumnWidth = 120;

            result.AddColumn(x => x.FullName, T("Họ và tên"));
            result.AddColumn(x => x.Email, T("Địa chỉ email"));
            result.AddColumn(x => x.IsBlocked)
                .HasHeaderText(T("Đã khóa"))
                .AlignCenter()
                .HasWidth(100)
                .RenderAsStatusImage();

            result.AddAction().HasText(this.T("Create")).HasUrl(this.Url.Action("Edit", new { id = 0 })).HasButtonStyle(ButtonStyle.Primary).HasBoxButton(false).HasCssClass(Constants.RowLeft).HasRow(true);
            result.AddRowAction().HasText(this.T("Edit")).HasUrl(x => Url.Action("Edit", new { id = x.Id })).HasButtonStyle(ButtonStyle.Default).HasButtonSize(ButtonSize.ExtraSmall);
            result.AddRowAction(true).HasText(this.T("Delete")).HasName("Delete").HasValue(x => Convert.ToString(x.Id)).HasButtonStyle(ButtonStyle.Danger).HasButtonSize(ButtonSize.ExtraSmall).HasConfirmMessage(this.T(Constants.Messages.ConfirmDeleteRecord));
            
            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");
            return result;
        }
        
        private ControlGridAjaxData<EmailInfo> GetEmails(ControlGridFormRequest options)
        {
            int totals;
            var service = WorkContext.Resolve<IEmailsService>();
            var items = service.GetRecords(options, out totals);
            var result = new ControlGridAjaxData<EmailInfo>(items, totals);
            return result;
        }
        
        [Url("admin/emails/edit/{id}")]
        public ActionResult Edit(int id)
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Quản lý thông tin liên hệ"), Url = "#" });
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Thông tin liên hệ"), Url = Url.Action("Index") });
            var model = new EmailsModel();
            if (id > 0)
            {
                var service = WorkContext.Resolve<IEmailsService>();
                model = service.GetById(id);
            }
            var result = new ControlFormResult<EmailsModel>(model);
            result.Title = this.T("Thông tin liên hệ");
            result.FormMethod = FormMethod.Post;
            result.UpdateActionName = "Update";
            result.ShowCancelButton = false;
            result.ShowBoxHeader = false;
            result.FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml;
            result.FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml;

            result.AddAction().HasText(this.T("Clear")).HasUrl(this.Url.Action("Edit", RouteData.Values.Merge(new { id = 0 }))).HasButtonStyle(ButtonStyle.Success);
            result.AddAction().HasText(this.T("Back")).HasUrl(this.Url.Action("Index")).HasButtonStyle(ButtonStyle.Danger);
            return result;
        }
        
        [HttpPost()]
        [FormButton("Save")]
        [ValidateInput(false)]
        [Url("admin/emails/update")]
        public ActionResult Update(EmailsModel model)
        {
            if (!ModelState.IsValid)
            {
                return new AjaxResult().Alert(T(Constants.Messages.InvalidModel));
            }

            var service = WorkContext.Resolve<IEmailsService>();
            EmailInfo item = model.Id == 0 ? new EmailInfo() : service.GetById(model.Id);
            item.FullName = model.FullName;
            item.PhoneNumber = model.PhoneNumber;
            item.Email = model.Email;
            item.Notes = model.Notes;
            item.IsBlocked = model.IsBlocked;
            service.Save(item);

            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").Alert(T("Đã cập nhật thành công."));
        }
        
        [ActionName("Update")]
        [FormButton("Delete")]
        public ActionResult Delete(int id)
        {
            var service = WorkContext.Resolve<IEmailsService>();
            var model = service.GetById(id);
            model.IsBlocked = true;
            service.Update(model);

            return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE").Alert(T("Đã xóa tạm thời bài viết."));
        }
    }
}
