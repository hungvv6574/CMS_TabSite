using System;
using CMSSolutions.Configuration;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Websites.Controllers
{
    public class CustomSettings : ISettings
    {
        public CustomSettings()
        {
            CategoryAboutUs = 2;
            CategoryBusinesses = 3;
            ListCategoriesBusinesses = "5,7,15,19";
            CategoryContact = 31;
            CategoryRootMedia = 26;
        }

        public void OnEditing(ControlFormResult<ISettings> controlForm, WorkContext workContext)
        {
            
        }

        [ControlNumeric(Required = true, LabelText = "Chuyên mục AboutUs trên trang chủ", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
        public int CategoryAboutUs { get; set; }

        [ControlNumeric(Required = true, LabelText = "Chuyên mục Businesses trên trang chủ", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
        public int CategoryBusinesses { get; set; }

        [ControlText(Type = ControlText.TextBox, LabelText = "Các chuyên mục hiển thị trong Businesses", Required = true, MaxLength = 200, ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
        public string ListCategoriesBusinesses { get; set; }

        [ControlNumeric(Required = true, LabelText = "Các chuyên mục gốc Media", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
        public int CategoryRootMedia { get; set; }

        [ControlNumeric(Required = true, LabelText = "Chuyên mục AboutUs trên trang chủ", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
        public int CategoryContact { get; set; }

        public string Name { get { return "Cài đặt chung"; } }

        public bool Hidden { get { return false; } }
    }
}