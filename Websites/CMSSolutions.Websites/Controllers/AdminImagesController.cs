using System.Linq;
using CMSSolutions.Web.Routing;
using CMSSolutions.Websites.Extensions;
using CMSSolutions.Websites.Permissions;

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
    public class AdminImagesController : BaseAdminController
    {
        public AdminImagesController(IWorkContextAccessor workContextAccessor) : 
                base(workContextAccessor)
        {
            this.TableName = "tblImages";
        }
        
        [Url("admin/images")]
        public ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Quản lý ảnh bài viết"), Url = "#" });
            var result = new ControlGridFormResult<ImageInfo>();
            var siteSettings = WorkContext.Resolve<SiteSettings>();
            result.Title = this.T("Quản lý ảnh bài viết");
            result.CssClass = "table table-bordered table-striped";
            result.UpdateActionName = "Update";
            result.IsAjaxSupported = true;
            result.DefaultPageSize = siteSettings.DefaultPageSize;
            result.EnablePaginate = true;
            result.FetchAjaxSource = this.GetImages;
            result.GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml;
            result.GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml;
            result.ClientId = TableName;
            result.ActionsColumnWidth = 120;

            result.AddCustomVar(Extensions.Constants.CategoryId, "$('#" + Extensions.Constants.CategoryId + "').val();", true);

            result.AddColumn(x => x.SortOrder, T("Thứ tự")).HasWidth(80).AlignCenter();
            result.AddColumn(x => x.FilePath, T("Ảnh"))
                .AlignCenter()
                .HasWidth(100)
                .RenderAsImage(y => y.FilePath, Extensions.Constants.CssThumbsSize);
            result.AddColumn(x => x.Caption, T("Mô tả"));

            result.AddRowAction().HasText(this.T("Edit")).HasUrl(x => Url.Action("Edit", new { id = x.Id, cateId = 0, articlesId = 0 })).HasButtonStyle(ButtonStyle.Default).HasButtonSize(ButtonSize.ExtraSmall);
            result.AddRowAction(true).HasText(this.T("Delete")).HasName("Delete").HasValue(x => Convert.ToString(x.Id)).HasButtonStyle(ButtonStyle.Danger).HasButtonSize(ButtonSize.ExtraSmall).HasConfirmMessage(this.T(Constants.Messages.ConfirmDeleteRecord));

            result.AddAction(new ControlFormHtmlAction(() => BuildCategories()).HasParentClass(Constants.ContainerCssClassCol3));

            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");
            return result;
        }
        
        private ControlGridAjaxData<ImageInfo> GetImages(ControlGridFormRequest options)
        {
            int totals;
            var service = WorkContext.Resolve<IImagesService>();

            var categoryId = 0;
            if (Utilities.IsNotNull(Request.Form[Extensions.Constants.CategoryId]))
            {
                categoryId = Convert.ToInt32(Request.Form[Extensions.Constants.CategoryId]);
            }

            var records = service.SearchPaged("", categoryId, -1, options.PageIndex, options.PageSize, out totals);
            return new ControlGridAjaxData<ImageInfo>(records, totals);
        }

        [Url("admin/images/edit/{id}/{cateId}/{articlesId}")]
        public ActionResult Edit(int id, int cateId, int articlesId)
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Quản lý ảnh bài viết"), Url = "#" });
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Thông tin ảnh bài viết"), Url = Url.Action("Index") });
            var model = new ImagesModel {CategoryId = cateId, ArticlesId = articlesId};
            var service = WorkContext.Resolve<IImagesService>();

            if (id > 0)
            {
                var item = service.GetById(id);
                if (item != null)
                {
                    model.CategoryId = item.CategoryId;
                    model.ArticlesId = item.ArticlesId;
                    model.ListCategory = Utilities.ParseListInt(item.ListCategory);
                    if (item.ArticlesId > 0)
                    {
                        var list = service.GetRecords(x => x.ArticlesId == item.ArticlesId && x.CategoryId == 0);
                        var listModel = list.Select(image => new UploadImageModel { ImageUrl = image.FilePath, Caption = image.Caption, SortOrder = image.SortOrder }).ToList();
                        model.UploadPhotos = listModel;
                    }
                    else
                    {
                        var list = service.GetRecords(x => x.CategoryId == item.CategoryId && x.ArticlesId == 0);
                        var listModel = list.Select(image => new UploadImageModel { ImageUrl = image.FilePath, Caption = image.Caption, SortOrder = image.SortOrder }).ToList();
                        model.UploadPhotos = listModel;
                    }
                }
            }

            if (cateId > 0)
            {
                var list = service.GetRecords(x => x.CategoryId == cateId);
                foreach (var imageInfo in list)
                {
                    if (!string.IsNullOrEmpty(imageInfo.ListCategory))
                    {
                        model.ListCategory = Utilities.ParseListInt(imageInfo.ListCategory);
                    } 
                }
                var listModel = list.Select(image => new UploadImageModel { ImageUrl = image.FilePath, Caption = image.Caption, SortOrder = image.SortOrder }).ToList();
                model.UploadPhotos = listModel;
            }

            if (articlesId > 0)
            {
                var list = service.GetRecords(x => x.ArticlesId == articlesId);
                var listModel = list.Select(image => new UploadImageModel { ImageUrl = image.FilePath, Caption = image.Caption, SortOrder = image.SortOrder }).ToList();
                model.UploadPhotos = listModel;
            }

            var result = new ControlFormResult<ImagesModel>(model);
            result.Title = this.T("Thông tin ảnh chuyên mục");
            result.FormMethod = FormMethod.Post;
            result.UpdateActionName = "Update";
            result.ShowCancelButton = false;
            result.ShowBoxHeader = false;
            result.FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml;
            result.FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml;

            if (articlesId > 0)
            {
                result.MakeReadOnlyProperty(x => x.CategoryId);
                result.MakeReadOnlyProperty(x => x.ListCategory);
            }

            result.RegisterExternalDataSource(x => x.CategoryId, BindCategories());
            result.RegisterExternalDataSource(x => x.ListCategory, BindCategories());

            result.AddAction().HasText(this.T("Back to Articles")).HasUrl(this.Url.Action("Index", "AdminArticles")).HasButtonStyle(ButtonStyle.Danger);
            result.AddAction().HasText(this.T("Back to Categories")).HasUrl(this.Url.Action("Index", "AdminCategories")).HasButtonStyle(ButtonStyle.Danger);
            //if (CheckPermission(ImagesPermissions.ManagerImages, T("Can't access the dashboard panel.")))
            //{
            //    result.AddAction().HasText(this.T("Back")).HasUrl(this.Url.Action("Index")).HasButtonStyle(ButtonStyle.Danger);
            //}

            return result;
        }

        [HttpPost()]
        [FormButton("Save")]
        [ValidateInput(false)]
        [Url("admin/images/update")]
        public ActionResult Update(ImagesModel model)
        {
            if (!ModelState.IsValid)
            {
                return new AjaxResult().Alert(T(Constants.Messages.InvalidModel));
            }

            var service = WorkContext.Resolve<IImagesService>();
            if (model.ArticlesId == 0)
            {
                var listDelete = service.GetRecords(x => x.CategoryId == model.CategoryId && x.ArticlesId == 0);
                service.DeleteMany(listDelete);
            }
            else
            {
                var listDelete = service.GetRecords(x => x.CategoryId == 0 && x.ArticlesId == model.ArticlesId);
                service.DeleteMany(listDelete);
            }

            foreach (var image in model.UploadPhotos)
            {
                var row = new ImageInfo
                {
                    Id = 0,
                    LanguageCode = "",
                    CategoryId = model.CategoryId,
                    ArticlesId = model.ArticlesId,
                    ListCategory = Utilities.ParseString(model.ListCategory),
                    FilePath = image.ImageUrl,
                    Caption = image.Caption,
                    SortOrder = image.SortOrder,
                    Url = image.Url
                };
                if (model.ArticlesId > 0)
                {
                    row.CategoryId = 0;
                }
                if (model.CategoryId > 0)
                {
                    row.ArticlesId = 0;
                }
                service.Save(row);
            }

            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").Alert(T("Đã cập nhật thành công."));
        }
        
        [ActionName("Update")]
        [FormButton("Delete")]
        public ActionResult Delete(int id)
        {
            var service = WorkContext.Resolve<IImagesService>();
            var model = service.GetById(id);
            service.Delete(model);

            return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE").Alert(T("Đã xóa thành công."));
        }
    }
}
