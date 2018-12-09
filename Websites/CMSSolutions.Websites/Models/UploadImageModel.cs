using CMSSolutions.Web.UI.ControlForms;
using JetBrains.Annotations;

namespace CMSSolutions.Websites.Models
{
    [UsedImplicitly]
    public class UploadImageModel
    {
        [ControlNumeric(Required = true, LabelText = "Vị trí", ColumnWidth = 80)]
        public int SortOrder { get; set; }

        [ControlText(MaxLength = 250, LabelText = "Mô tả", ColumnWidth = 150)]
        public string Caption { get; set; }

        [ControlText(Type = ControlText.TextBox, Required = false, MaxLength = 500, ColumnWidth = 200)]
        public string Url { get; set; }

        [ControlFileUpload(EnableFineUploader = true, Required = true, LabelText = "Đường dẫn ảnh")]
        public string ImageUrl { get; set; }
    }
}