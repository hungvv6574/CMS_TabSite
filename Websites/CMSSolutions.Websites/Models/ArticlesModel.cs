using System;

namespace CMSSolutions.Websites.Models
{
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;

    public class ArticlesModel
    {
        public ArticlesModel()
        {
            IsPublished = true;
            Year = DateTime.Now.Year;
        }

        [ControlHidden]
        public int Id { get; set; }

        [ControlHidden]
        public int ViewCount { get; set; }

        [ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Chuyên mục hiển thị", ContainerCssClass = Constants.ContainerCssClassCol2, ContainerRowIndex = 0)]
        public int CategoryId { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", PrependText = "Đăng tin", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
        public bool IsPublished { get; set; }

        [ControlText(Type = ControlText.TextBox, PlaceHolder = "Vui lòng nhập tiêu đề bài viết. Tối đa 200 ký tự.", LabelText = "Tiêu đề", Required = true, MaxLength = 200, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 1)]
        public string Title { get; set; }

        [ControlText(Type = ControlText.TextBox, LabelText = "Tiêu đề không dấu", MaxLength = 200, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 1)]
        public string Alias { get; set; }

        [ControlNumeric(LabelText = "Năm", ContainerCssClass = Constants.ContainerCssClassCol2, ContainerRowIndex = 3)]
        public int Year { get; set; }

        [ControlFileUpload(EnableFineUploader = true, LabelText = "Ảnh đại diện", ContainerCssClass = Constants.ContainerCssClassCol10, ContainerRowIndex = 3, ShowThumbnail = true)]
        public string Image { get; set; }

        [ControlText(LabelText = "Tóm tắt nội dung", Required = true, PlaceHolder = "Nhập tối đa 400 ký tự.", Type = ControlText.MultiText, Rows = 2, MaxLength = 400, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 4)]
        public string Summary { get; set; }

        [ControlText(Type = ControlText.TextBox, Required = false, MaxLength = 500, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 5)]
        public string VideoUrl { get; set; }

        [ControlText(LabelText = "Nội dung bài viết", Required = false, Type = ControlText.RichText, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 6)]
        public string Contents { get; set; }

        [ControlText(LabelText = "Từ khóa SEO", Required = true, PlaceHolder = "Vui lòng nhập từ khóa SEO cho bài viết. Tối đa 400 ký tự.", Type = ControlText.MultiText, Rows = 2, MaxLength = 400, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 7)]
        public string Description { get; set; }

        [ControlText(LabelText = "Tags SEO", Required = true, PlaceHolder = "Nhập từ khóa SEO cách nhau bởi dấu, Tối đa 500 ký tự.", Type = ControlText.MultiText, Rows = 2, MaxLength = 500, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 7)]
        public string Tags { get; set; }

        public static implicit operator ArticlesModel(ArticlesInfo entity)
        {
            return new ArticlesModel
            {
                Id = entity.Id,
                CategoryId = entity.CategoryId,
                Title = entity.Title,
                Alias = entity.Alias,
                Summary = entity.Summary,
                Contents = entity.Contents,
                IsPublished = entity.IsPublished,
                Year = entity.Year,
                VideoUrl = entity.VideoUrl,
                Image = entity.Image,
                ViewCount = entity.ViewCount,
                Description = entity.Description,
                Tags = entity.Tags
            };
        }
    }
}
