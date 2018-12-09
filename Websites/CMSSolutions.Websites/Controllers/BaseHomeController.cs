using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Widgets.Services;
using CMSSolutions.Localization.Services;
using CMSSolutions.Net.Mail;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Websites.Entities;
using CMSSolutions.Websites.Extensions;
using CMSSolutions.Websites.Models;
using CMSSolutions.Websites.Services;

namespace CMSSolutions.Websites.Controllers
{
    public class BaseHomeController : BaseController
    {
        #region Paged
        public int PageIndex { get; set; }

        public int PageSize { get; set; }
        #endregion

        public BaseHomeController(IWorkContextAccessor workContextAccessor)
            : base(workContextAccessor)
        {

        }

        #region Send Emails
        [HttpPost, ValidateInput(false)]
        [Url("home/contact.html")]
        public ActionResult ContactInformations()
        {
            var email = Request.Form["txtEmailAddress"];
            var fullName = Request.Form["txtFullName"];
            var phoneNumber = Request.Form["txtPhoneNumber"];
            var messages = Request.Form["txtMessages"];

            var result = new DataViewerModel();
            var htmlBuilder = new StringBuilder();
            htmlBuilder.Append("<div style=\"float:left;width:100%;\">");
            htmlBuilder.AppendFormat("<div style=\"float:left;\">{0} </div>", T("Họ và Tên:"));
            htmlBuilder.AppendFormat("<div style=\"float:left;margin-left:5px;\">{0}</div>", fullName);
            htmlBuilder.Append("</div><br>");

            htmlBuilder.Append("<div style=\"float:left;width:100%;\">");
            htmlBuilder.AppendFormat("<div style=\"float:left;\">{0} </div>", T("Số điện thoại:"));
            htmlBuilder.AppendFormat("<div style=\"float:left;margin-left:5px;\">{0}</div>", phoneNumber);
            htmlBuilder.Append("</div><br>");

            htmlBuilder.Append("<div style=\"float:left;width:100%;\">");
            htmlBuilder.AppendFormat("<div style=\"float:left;\">{0} </div>", T("Địa chỉ email:"));
            htmlBuilder.AppendFormat("<div style=\"float:left;margin-left:5px;\">{0}</div>", email);
            htmlBuilder.Append("</div><br>");

            htmlBuilder.Append("<div style=\"float:left;width:100%;\">");
            htmlBuilder.AppendFormat("<div style=\"float:left;\">{0} </div>", T("Yêu cầu của bạn:"));
            htmlBuilder.AppendFormat("<div style=\"float:left;margin-left:5px;\">{0}</div>", messages);
            htmlBuilder.Append("</div><br>");

            string html = System.IO.File.ReadAllText(Server.MapPath("~/Media/Default/EmailTemplates/ContactInfo.html"));
            html = html.Replace("[MAILBODY]", htmlBuilder.ToString());
            try
            {
                SendEmail(T("Thông tin liên hệ"), html, email, T("Receive Email Contact"));
                result.Status = true;
                result.Data = T("Send Email Success");
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Data = ex.Message;
            }

            return Json(result);
        }
        public void SendEmail(string subject, string body, string toEmailReceive, string ccEmail)
        {
            var service = WorkContext.Resolve<IEmailSender>();
            var mailMessage = new MailMessage
            {
                Subject = subject,
                SubjectEncoding = Encoding.UTF8,
                Body = body,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true
            };

            mailMessage.Sender = new MailAddress(toEmailReceive);
            mailMessage.To.Add(toEmailReceive);
            if (!string.IsNullOrEmpty(ccEmail))
            {
                mailMessage.CC.Add(ccEmail);
            }
            //mailMessage.Bcc.Add("sitetab2016@gmail.com");

            service.Send(mailMessage);
        }
        #endregion

        #region Build Html
        public void BuildPage(int categoryId, bool isShowPartner = false)
        {
            var widget = WorkContext.Resolve<IWidgetService>();
            var viewRenderer = new ViewRenderer { Context = ControllerContext };
            var categoryService = WorkContext.Resolve<ICategoriesService>();
            categoryService.LanguageCode = WorkContext.CurrentCulture;
            var partnerService = WorkContext.Resolve<IPartnerService>();

            #region PageHeaderTop
            var modelPageHeaderTop = new DataViewerModel();
            var languageService = WorkContext.Resolve<ILanguageService>();
            modelPageHeaderTop.ListLanguages = languageService.GetRecords().Where(x => !x.IsBlocked).ToList();
            var viewPageHeaderTop = viewRenderer.RenderPartialView(Extensions.Constants.ViewPageHeaderTop, modelPageHeaderTop);
            WorkContext.Layout.PageHeaderTop.Add(new MvcHtmlString(viewPageHeaderTop));
            #endregion

            #region PageNavigationMenu
            var modelPageNavigationMenu = new DataViewerModel();
            modelPageNavigationMenu.CategoryId = categoryId;
            modelPageNavigationMenu.ListCategoriesParent = categoryService.GetRecords(x => x.IsActived && !x.IsDeleted && x.IsDisplayMenu && x.ParentId == 0 && x.LanguageCode == WorkContext.CurrentCulture).OrderBy(x => x.OrderBy).ToList();
            modelPageNavigationMenu.ListCategories = categoryService.GetRecords(x => x.IsActived && !x.IsDeleted && x.IsDisplayMenu && x.ParentId > 0 && x.LanguageCode == WorkContext.CurrentCulture);
            var viewPageNavigationMenu = viewRenderer.RenderPartialView(Extensions.Constants.ViewPageNavigationMenu, modelPageNavigationMenu);
            WorkContext.Layout.PageNavigationMenu.Add(new MvcHtmlString(viewPageNavigationMenu));
            #endregion

            #region SectionClient
            var modelSectionClient = new DataViewerModel();
            modelSectionClient.IsShowPartner = isShowPartner;
            var catePartner = categoryService.GetRecords(x => x.RefId == (int)CategoriesType.Partner && x.LanguageCode == WorkContext.CurrentCulture).FirstOrDefault();
            if (catePartner != null)
            {
                modelSectionClient.CategoryInfo = catePartner;
            }
            modelSectionClient.ListPartners = partnerService.GetRecords(x => !x.IsDeleted && x.LanguageCode == WorkContext.CurrentCulture).OrderBy(x => x.SortOrder).ToList();
            var viewSectionClient = viewRenderer.RenderPartialView(Extensions.Constants.ViewSectionClient, modelSectionClient);
            WorkContext.Layout.SectionClient.Add(new MvcHtmlString(viewSectionClient));
            #endregion

            #region PageFooter
            var modelPageFooter = new DataViewerModel();
            modelPageFooter.ListCategories = categoryService.GetRecords(x => x.IsActived && !x.IsDeleted && x.LanguageCode == WorkContext.CurrentCulture && x.IsDisplayFooter).OrderBy(x => x.OrderBy).ToList();
            var viewPageFooter = viewRenderer.RenderPartialView(Extensions.Constants.ViewPageFooter, modelPageFooter);
            WorkContext.Layout.PageFooter.Add(new MvcHtmlString(viewPageFooter));
            #endregion
        }

        protected void BuildBreadcrumb(DataViewerModel model, int categoryId = 0, int year = 0)
        {
            model.Breadcrumb = new StringBuilder();
            var categoryService = WorkContext.Resolve<ICategoriesService>();
            categoryService.LanguageCode = WorkContext.CurrentCulture;
            var categoryHome = categoryService.GetHomePage();
            model.Breadcrumb.Append("<ol class=\"breadcrumb\">");
            model.Breadcrumb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", categoryHome.Url, categoryHome.ShortName);
            if (categoryId > 0)
            {
                var cate = categoryService.GetByIdCache(categoryId);
                if (cate.ParentId > 0)
                {
                    var parent = categoryService.GetByIdCache(cate.ParentId);
                    if (parent != null)
                    {
                        var children = categoryService.GetByIdCache(parent.ParentId);
                        if (children != null)
                        {
                            model.Breadcrumb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", children.Url, children.ShortName);
                        }
                        model.Breadcrumb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", parent.Url, parent.ShortName);
                    }
                }

                if (year > 0)
                {
                    model.Breadcrumb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", Url.Action("Index", "HomeMediaNews", new {alias = cate.Alias}), cate.ShortName);
                    var url = Url.Action("MediaCategory", "HomeMediaNews", new { alias = cate.Alias, year = year });
                    model.Breadcrumb.AppendFormat("<li class=\"active\"><a href=\"{0}\">{1}</a></li>", url, year);
                }
                else
                {
                    model.Breadcrumb.AppendFormat("<li class=\"active\"><a href=\"{0}\">{1}</a></li>", cate.Url, cate.ShortName);
                }
            }
            else
            {
                if (categoryId == -1)
                {
                    model.Breadcrumb.AppendFormat("<li class=\"active\"><a href=\"#\">{0}</a></li>", T("Search"));
                }
                else
                {
                    if (model.CategoryInfo.ParentId > 0)
                    {
                        var parent = categoryService.GetByIdCache(model.CategoryInfo.ParentId);
                        if (parent != null)
                        {
                            var children = categoryService.GetByIdCache(parent.ParentId);
                            if (children != null)
                            {
                                model.Breadcrumb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", children.Url, children.ShortName);
                            }
                            model.Breadcrumb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", parent.Url, parent.ShortName);
                        }
                    }
                    model.Breadcrumb.AppendFormat("<li class=\"active\"><a href=\"{0}\">{1}</a></li>", model.CategoryInfo.Url, model.CategoryInfo.ShortName);
                }
            }
            model.Breadcrumb.Append("</ol>");
        }
        #endregion

        #region Search
        [HttpPost, ValidateInput(false)]
        [Url("search-data/keyword.html")]
        public ActionResult SearchData()
        {
            var keyword = Request.Form["keyword"];
            var service = WorkContext.Resolve<ISearchService>();
            service.LanguageCode = WorkContext.CurrentCulture;
            var condition = new List<SearchCondition>
            {
                new SearchCondition(new[]
                {
                    SearchField.Title.ToString(),
                    SearchField.Keyword.ToString(),
                    SearchField.Sumary.ToString()
                }, keyword)
            };

            var total = 0;
            var data = service.Search(condition, 1, 10, ref total);
            return Json(data);
        }
        #endregion

        #region Receive Email
        [HttpPost, ValidateInput(false)]
        [Url("email/receive-email.html")]
        public ActionResult ReceiveEmail()
        {
            var emailAddress = Request.Form["txtEmailAddress"];
            var model = new DataViewerModel();
            if (string.IsNullOrEmpty(emailAddress))
            {
                model.Status = false;
                model.Data = T("Địa chỉ email không để trống.");
                return Json(model);
            }

            var service = WorkContext.Resolve<IEmailsService>();
            var status = service.CheckEmailExist(emailAddress);
            if (!status)
            {
                var email = new EmailInfo
                {
                    Email = emailAddress,
                    FullName = string.Empty,
                    IsBlocked = false,
                    Notes = "Yêu cầu nhận gửi thông tin qua email."
                };
                service.Save(email);
            }

            model.Status = true;
            model.Data = T("Đã lưu lại địa chỉ email của bạn thành công.");

            return Json(model);
        }
        #endregion
    }
}
