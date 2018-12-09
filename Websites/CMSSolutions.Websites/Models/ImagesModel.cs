using System.Collections.Generic;

namespace CMSSolutions.Websites.Models
{
    using CMSSolutions.Web.UI.ControlForms;

    public class ImagesModel
    {
        [ControlHidden()]
        public long Id { get; set; }
        
        [ControlHidden()]
        public int ArticlesId { get; set; }

        [ControlChoice(ControlChoice.DropDownList, LabelText = "Chuyên mục hiển thị", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
        public int CategoryId { get; set; }

        [ControlChoice(ControlChoice.DropDownList, AllowMultiple = true, EnableChosen = true, LabelText = "Đường dẫn tới mục", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
        public int[] ListCategory { get; set; }

        [ControlGrid(1, 20, 5, CssClass = "table table-striped table-bordered", LabelText = "Upload ảnh", ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 1)]
        public List<UploadImageModel> UploadPhotos { get; set; }
    }
}
