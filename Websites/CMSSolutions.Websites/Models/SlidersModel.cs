using System.Collections.Generic;

namespace CMSSolutions.Websites.Models
{
    using CMSSolutions.Web.UI.ControlForms;

    public class SlidersModel
    {
        [ControlHidden()]
        public int Id { get; set; }

        //[ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Chuyên mục hiển thị", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
        //public int CategoryId { get; set; }

        [ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Slider cho trang", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
        public int Type { get; set; }

        [ControlGrid(1, 20, 5, CssClass = "table table-striped table-bordered", LabelText = "Upload ảnh", ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 1)]
        public List<UploadImageModel> UploadPhotos { get; set; }
    }
}
