using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CMSSolutions.Extensions;
using CMSSolutions.Localization.Domain;
using CMSSolutions.Localization.Services;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Security.Services;
using CMSSolutions.Websites.Extensions;
using CMSSolutions.Websites.Services;

namespace CMSSolutions.Websites.Controllers
{
    public class BaseAdminController : BaseController
    {
        public BaseAdminController(IWorkContextAccessor workContextAccessor) 
            : base(workContextAccessor)
        {

        }

        [Url("admin/reset-cache")]
        public ActionResult ResetCache()
        {
            var serviceLanguage = WorkContext.Resolve<ILanguageService>();
            var items = serviceLanguage.GetRecords(x => x.Theme == Constants.ThemeDefault);
            if (items != null && items.Count > 0)
            {
                var categoryService = WorkContext.Resolve<ICategoriesService>();
                var searchService = WorkContext.Resolve<ISearchService>();
                foreach (var language in items)
                {
                    try
                    {
                        #region Category
                        categoryService.LanguageCode = language.CultureCode;
                        categoryService.ResetCache();
                        #endregion

                        #region Search
                        searchService.LanguageCode = language.CultureCode;
                        searchService.ResetCache();
                        #endregion
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            return Redirect(Url.Action("Index", "Admin"));
        }

        public virtual string BuildLanguages(bool hasEvent)
        {
            var service = WorkContext.Resolve<ILanguageService>();
            var items = service.GetActiveLanguages();
            var list = new List<Language>();
            list.AddRange(items.Where(x => x.Theme == Constants.ThemeDefault));
            var sb = new StringBuilder();

            if (hasEvent)
            {
                var url = Url.Action("GetCategoriesByLanguage");
                sb.AppendFormat(T("Ngôn ngữ") + " <select id=\"" + Extensions.Constants.LanguageCode + "\" name=\"" + Extensions.Constants.LanguageCode + "\" autocomplete=\"off\" class=\"uniform form-control col-md-3\" onchange=\"" +
                " var data = $('#" + TableName + "_Container').find('select').serialize();" +
                    @"$.ajax({{
	                url: '" + url + @"',
	                data: data,
	                type: 'POST',
	                dataType: 'json',
	                success: function (result) {{
                        $('#" + Extensions.Constants.CategoryId + @"').empty();
                        if (result != null){{
		                    $.each(result, function(idx, item) {{                  
                                $('#" + Extensions.Constants.CategoryId + @"').append($('<option>', {{
                                    value: item.Value,
                                    text: item.Text
                                }}));
                            }});   
                        }}
	                }}
                }});
                $('#" + TableName + "').jqGrid().trigger('reloadGrid');" +
                "\">");
            }
            else
            {
                sb.AppendFormat(T("Ngôn ngữ") + " <select id=\"" + Extensions.Constants.LanguageCode + "\" name=\"" + Extensions.Constants.LanguageCode + "\" autocomplete=\"off\" class=\"uniform form-control col-md-3\" onchange=\"$('#" + TableName + "').jqGrid().trigger('reloadGrid');\">");
            }

            foreach (var language in list)
            {
                if (language.CultureCode == WorkContext.CurrentCulture)
                {
                    sb.AppendFormat("<option selected value=\"{1}\">{0}</option>", language.Name, language.CultureCode);
                    continue;
                }

                sb.AppendFormat("<option value=\"{1}\">{0}</option>", language.Name, language.CultureCode);
            }

            sb.Append("</select>");

            return sb.ToString();
        }

        public IEnumerable<SelectListItem> BindLanguages()
        {
            var service = WorkContext.Resolve<ILanguageService>();
            var items = service.GetActiveLanguages();
            var result = new List<SelectListItem>();
            foreach (var language in items)
            {
                var item = new SelectListItem();
                item.Text = language.Name;
                item.Value = language.CultureCode;
                item.Selected = false;
                result.Add(item);
            }

            return result;
        }

        public IEnumerable<SelectListItem> BindCategories()
        {
            var service = WorkContext.Resolve<ICategoriesService>();
            var items = service.GetTree();
            var result = new List<SelectListItem>();
            foreach (var categoryInfo in items)
            {
                var item = new SelectListItem
                {
                    Value = categoryInfo.Id.ToString(),
                    Text = categoryInfo.ChildenName,
                    Selected = false
                };
                result.Add(item);
            }

            result.Insert(0, new SelectListItem { Text = T("--- Không chọn ---"), Value = "0" });

            return result;

        }
        [Url("admin/category/get-categories-by-language")]
        public ActionResult GetCategoriesByLanguage()
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
            foreach (var categoryInfo in items)
            {
                var item = new SelectListItem();
                item.Value = categoryInfo.Id.ToString();
                item.Text = categoryInfo.ChildenName;
                item.Selected = false;
                result.Add(item);
            }

            result.Insert(0, new SelectListItem { Text = T("--- Không chọn ---"), Value = "0" });

            return Json(result);
        }

        public string BuildCategories(int defaultValue = 0)
        {
            var service = WorkContext.Resolve<ICategoriesService>();
            var sb = new StringBuilder();

            sb.AppendFormat(T("Chuyên mục") + " <select id=\"" + Extensions.Constants.CategoryId + "\" name=\"" + Extensions.Constants.CategoryId + "\" autocomplete=\"off\" class=\"uniform form-control col-md-3\" onchange=\"$('#" + TableName + "').jqGrid().trigger('reloadGrid');\">");

            var list = service.GetTree();
            sb.AppendFormat("<option value=\"{1}\">{0}</option>", T("--- Không chọn ---"), 0);
            foreach (var cate in list)
            {
                if (defaultValue != 0 && cate.Id == defaultValue)
                {
                    sb.AppendFormat("<option selected value=\"{1}\">{0}</option>", cate.ChildenName, cate.Id);
                    continue;
                }

                sb.AppendFormat("<option value=\"{1}\">{0}</option>", cate.ChildenName, cate.Id);
            }

            sb.Append("</select>");

            return sb.ToString();
        }

        public virtual string BuildArticles()
        {
            var languageCode = WorkContext.CurrentCulture;
            if (Utilities.IsNotNull(Request.Form[Extensions.Constants.LanguageCode]))
            {
                languageCode = Request.Form[Extensions.Constants.LanguageCode];
            }

            var categoryId = 0;
            if (Utilities.IsNotNull(Request.Form[Extensions.Constants.CategoryId]))
            {
                categoryId = Convert.ToInt32(Request.Form[Extensions.Constants.CategoryId]);
            }

            var service = WorkContext.Resolve<IArticlesService>();
            service.LanguageCode = languageCode;
            service.CategoryId = categoryId;
            var sb = new StringBuilder();

            sb.AppendFormat(T("Tin bài") + " <select id=\"" + Extensions.Constants.ArticlesId + "\" name=\"" + Extensions.Constants.ArticlesId + "\" autocomplete=\"off\" class=\"uniform form-control col-md-3\" onchange=\"$('#" + TableName + "').jqGrid().trigger('reloadGrid');\">");
            sb.AppendFormat("<option selected value=\"{1}\">{0}</option>", T("--- Không chọn ---"), 0);
            var list = service.GetRecords(x => x.LanguageCode == languageCode && x.CategoryId == categoryId);
            foreach (var item in list)
            {
                sb.AppendFormat("<option value=\"{1}\">{0}</option>", item.Title, item.Id);
            }

            sb.Append("</select>");

            return sb.ToString();
        }

        [Url("admin/articles/get-articles-by-language")]
        public ActionResult GetArticlesByLanguage()
        {
            var languageCode = WorkContext.CurrentCulture;
            if (Utilities.IsNotNull(Request.Form[Extensions.Constants.LanguageCode]))
            {
                languageCode = Request.Form[Extensions.Constants.LanguageCode];
            }

            var categoryId = 0;
            if (Utilities.IsNotNull(Request.Form[Extensions.Constants.CategoryId]))
            {
                categoryId = Convert.ToInt32(Request.Form[Extensions.Constants.CategoryId]);
            }

            var service = WorkContext.Resolve<IArticlesService>();
            service.LanguageCode = languageCode;
            service.CategoryId = categoryId;
            var items = service.GetRecords(x => x.LanguageCode == languageCode && x.CategoryId == categoryId && x.IsDeleted == false);
            var result = new List<SelectListItem>();
            foreach (var articlesInfo in items)
            {
                var item = new SelectListItem();
                item.Value = articlesInfo.Id.ToString();
                item.Text = articlesInfo.Title;
                item.Selected = false;
                result.Add(item);
            }

            result.Insert(0, new SelectListItem { Text = T("--- Không chọn ---"), Value = "0" });

            return Json(result);
        }

        public virtual IEnumerable<SelectListItem> BindSliderType()
        {
            var items = EnumExtensions.GetListItems<PageSlider>();

            return items.Select(type => new SelectListItem { Text = type.Text, Value = type.Value, Selected = false }).ToList();
        }

        public virtual string BuildStatus()
        {
            var list = EnumExtensions.GetListItems<Status>();
            var sb = new StringBuilder();
            sb.AppendFormat(T("Trạng thái") + " <select id=\"" + Extensions.Constants.StatusId + "\" name=\"" + Extensions.Constants.StatusId + "\" autocomplete=\"off\" class=\"uniform form-control col-md-3\" onchange=\"$('#" + TableName + "').jqGrid().trigger('reloadGrid');\">");
            foreach (var status in list)
            {
                sb.AppendFormat("<option value=\"{1}\">{0}</option>", status.Text, status.Value);
            }

            sb.Append("</select>");
            return sb.ToString();
        }

        public virtual string BuildPageType()
        {
            var list = EnumExtensions.GetListItems<PageSlider>();
            var sb = new StringBuilder();
            sb.AppendFormat(T("Trang hiển thị") + " <select id=\"" + Extensions.Constants.PageType + "\" name=\"" + Extensions.Constants.PageType + "\" autocomplete=\"off\" class=\"uniform form-control col-md-3\" onchange=\"$('#" + TableName + "').jqGrid().trigger('reloadGrid');\">");
            foreach (var status in list)
            {
                sb.AppendFormat("<option value=\"{1}\">{0}</option>", status.Text, status.Value);
            }

            sb.Append("</select>");
            return sb.ToString();
        }

        public virtual string BuildUsers()
        {
            var service = WorkContext.Resolve<IMembershipService>();
            var list = service.GetRecords(x => !x.IsLockedOut);
            var sb = new StringBuilder();
            sb.AppendFormat(T("Người quản trị") + " <select id=\"" + Extensions.Constants.UserId + "\" name=\"" + Extensions.Constants.UserId + "\" autocomplete=\"off\" class=\"uniform form-control col-md-3\" onchange=\"$('#" + TableName + "').jqGrid().trigger('reloadGrid');\">");
            foreach (var user in list)
            {
                if (user.Id == WorkContext.CurrentUser.Id)
                {
                    sb.AppendFormat("<option selected value=\"{1}\">{0}</option>", user.FullName, user.Id);
                    continue;
                }

                sb.AppendFormat("<option value=\"{1}\">{0}</option>", user.FullName, user.Id);
            }

            sb.Append("</select>");

            return sb.ToString();
        }

        public virtual string BuildFromDate(bool showDefaultDate = true)
        {
            var sb = new StringBuilder();
            var date = "";
            if (showDefaultDate)
            {
                date = DateTime.Now.AddDays(-7).ToString(Extensions.Constants.DateTimeFomat);
            }

            sb.AppendFormat(T("Từ ngày") + " <input id=\"" + Extensions.Constants.FromDate + "\" name=\"" + Extensions.Constants.FromDate + "\" value=\"" + date + "\" class=\"form-control datepicker\"></input>");
            sb.Append("<script>$(document).ready(function () { " +
                      "$('.datepicker').datepicker({ " +
                      "dateFormat: 'dd/mm/yy', " +
                      "changeMonth: true, " +
                      "changeYear: true, " +
                      "onSelect: function (dateText) { " +
                      "$('#" + TableName + "').jqGrid().trigger('reloadGrid'); " +
                      "}}); });</script>");

            return sb.ToString();
        }

        public virtual string BuildToDate(bool showDefaultDate = true)
        {
            var sb = new StringBuilder();
            var date = "";
            if (showDefaultDate)
            {
                date = DateTime.Now.ToString(Extensions.Constants.DateTimeFomat);
            }

            sb.AppendFormat(T("Đến ngày") + " <input id=\"" + Extensions.Constants.ToDate + "\" name=\"" + Extensions.Constants.ToDate + "\" value=\"" + date + "\" class=\"form-control datepicker\"></input>");
            sb.Append("<script>$(document).ready(function () { " +
                      "$('.datepicker').datepicker({ " +
                      "dateFormat: 'dd/mm/yy', " +
                      "changeMonth: true, " +
                      "changeYear: true, " +
                      "onSelect: function (dateText) { " +
                      "$('#" + TableName + "').jqGrid().trigger('reloadGrid'); " +
                      "}}); });</script>");

            return sb.ToString();
        }

        public virtual string BuildSearchText()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(T("Từ khóa") + " <input value=\"\" placeholder=\"" + T("Nhập từ khóa cần tìm.") + "\" id=\"" + Extensions.Constants.SearchText + "\" name=\"" + Extensions.Constants.SearchText + "\" class=\"form-control\" onkeypress = \"return InputEnterEvent(event, '" + TableName + "');\" onblur=\"$('#" + TableName + "').jqGrid().trigger('reloadGrid');\"></input>");

            return sb.ToString();
        }
    }
}
