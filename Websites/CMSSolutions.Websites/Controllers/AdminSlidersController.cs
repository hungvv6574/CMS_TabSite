using System.Linq;
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
    public class AdminSlidersController : BaseAdminController
    {
        public AdminSlidersController(IWorkContextAccessor workContextAccessor) : 
                base(workContextAccessor)
        {
            this.TableName = "tblSliders";
        }
        
        [Url("admin/sliders")]
        public ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Quản lý banner"), Url = "#" });
            var result = new ControlGridFormResult<SliderInfo>();
            var siteSettings = WorkContext.Resolve<SiteSettings>();

            result.Title = this.T("Quản lý banner");
            result.CssClass = "table table-bordered table-striped";
            result.UpdateActionName = "Update";
            result.IsAjaxSupported = true;
            result.DefaultPageSize = siteSettings.DefaultPageSize;
            result.EnablePaginate = true;
            result.FetchAjaxSource = this.GetSliders;
            result.GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml;
            result.GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml;
            result.ClientId = TableName;
            result.ActionsColumnWidth = 120;
            
            result.AddCustomVar(Extensions.Constants.PageType, "$('#" + Extensions.Constants.PageType + "').val();", true);

            result.AddColumn(x => x.SortOrder, T("Thứ tự")).HasWidth(80).AlignCenter();
            result.AddColumn(x => x.ImageUrl, T("Ảnh"))
                .AlignCenter()
                .HasWidth(200)
                .RenderAsImage(y => y.ImageUrl, Extensions.Constants.CssThumbsSize);
            result.AddColumn(x => x.Caption, T("Mô tả"));

            result.AddAction().HasText(this.T("Create")).HasUrl(this.Url.Action("Edit", new { id = 0 })).HasButtonStyle(ButtonStyle.Primary).HasBoxButton(false).HasCssClass(Constants.RowLeft).HasRow(true);
            result.AddRowAction().HasText(this.T("Edit")).HasUrl(x => Url.Action("Edit", new { id = x.Id })).HasButtonStyle(ButtonStyle.Default).HasButtonSize(ButtonSize.ExtraSmall);
            result.AddRowAction(true).HasText(this.T("Delete")).HasName("Delete").HasValue(x => Convert.ToString(x.Id)).HasButtonStyle(ButtonStyle.Danger).HasButtonSize(ButtonSize.ExtraSmall).HasConfirmMessage(this.T(Constants.Messages.ConfirmDeleteRecord));

            result.AddAction(new ControlFormHtmlAction(BuildPageType)).HasParentClass(Constants.ContainerCssClassCol3);

            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");
            return result;
        }
        
        private ControlGridAjaxData<SliderInfo> GetSliders(ControlGridFormRequest options)
        {
            int totals;
            var service = WorkContext.Resolve<ISlidersService>();
            var pageType = 0;
            if (Utilities.IsNotNull(Request.Form[Extensions.Constants.PageType]))
            {
                pageType = Convert.ToInt32(Request.Form[Extensions.Constants.PageType]);
            }

            if (pageType != 0)
            {
                var items = service.GetRecords(options, out totals, x => x.Type == pageType);
                var result = new ControlGridAjaxData<SliderInfo>(items, totals);
                return result;
            }
            else
            {
                var items = service.GetRecords(options, out totals);
                var result = new ControlGridAjaxData<SliderInfo>(items, totals);
                return result;
            }
        }
        
        [Url("admin/sliders/edit/{id}")]
        public ActionResult Edit(int id)
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Quản lý banner"), Url = "#" });
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Thông tin banner"), Url = Url.Action("Index") });
            var model = new SlidersModel();
            if (id > 0)
            {
                var service = WorkContext.Resolve<ISlidersService>();
                var item = service.GetById(id);
                if (item != null)
                {
                    model.Type = item.Type;
                    var list = service.GetRecords(x => x.Type == item.Type);
                    var listModel = list.Select(image => new UploadImageModel { ImageUrl = image.ImageUrl, Caption = image.Caption, SortOrder = image.SortOrder }).ToList();
                    model.UploadPhotos = listModel;
                }
            }

            var result = new ControlFormResult<SlidersModel>(model);
            result.Title = this.T("Thông tin banner");
            result.FormMethod = FormMethod.Post;
            result.UpdateActionName = "Update";
            result.ShowCancelButton = false;
            result.ShowBoxHeader = false;
            result.FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml;
            result.FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml;

            //result.RegisterExternalDataSource(x => x.CategoryId, BindCategories());
            result.RegisterExternalDataSource(x => x.Type, y => BindSliderType());

            result.AddAction().HasText(this.T("Thêm mới")).HasUrl(this.Url.Action("Edit", RouteData.Values.Merge(new { id = 0 }))).HasButtonStyle(ButtonStyle.Success);
            result.AddAction().HasText(this.T("Back")).HasUrl(this.Url.Action("Index")).HasButtonStyle(ButtonStyle.Danger);

            return result;
        }
        
        [HttpPost()]
        [FormButton("Save")]
        [ValidateInput(false)]
        [Url("admin/sliders/update")]
        public ActionResult Update(SlidersModel model)
        {
            if (!ModelState.IsValid)
            {
                return new AjaxResult().Alert(T(Constants.Messages.InvalidModel));
            }

            var service = WorkContext.Resolve<ISlidersService>();
            var listDelete = service.GetRecords(x => x.Type == model.Type);
            service.DeleteMany(listDelete);

            foreach (var image in model.UploadPhotos)
            {
                var row = new SliderInfo
                {
                    Id = 0,
                    LanguageCode = "",
                    CategoryId = 0,
                    Type = model.Type,
                    ImageUrl = image.ImageUrl,
                    Caption = image.Caption,
                    SortOrder = image.SortOrder,
                    Url = image.Url,
                    IsDeleted = false
                };
                service.Save(row);
            }

            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").Alert(T("Đã cập nhật thành công.")).Redirect(Url.Action("Index"));
        }
        
        [ActionName("Update")]
        [FormButton("Delete")]
        public ActionResult Delete(int id)
        {
            var service = WorkContext.Resolve<ISlidersService>();
            var model = service.GetById(id);
            service.Delete(model);

            return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE").Alert(T("Đã xóa thành công."));
        }
    }
}
