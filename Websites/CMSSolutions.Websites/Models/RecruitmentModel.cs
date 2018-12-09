using System;

namespace CMSSolutions.Websites.Models
{
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;

    public class RecruitmentModel
    {
        public RecruitmentModel()
        {
           FinishDate = DateTime.Now.AddMonths(1).ToString(Extensions.Constants.DateTimeFomat);
        }

        [ControlHidden]
        public int Id { get; set; }

        [ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Chuyên mục hiển thị", ContainerCssClass = Constants.ContainerCssClassCol2, ContainerRowIndex = 0)]
        public int CategoryId { get; set; }

        [ControlDatePicker(LabelText = "Hạn nộp hồ sơ", Required = true, DateFormat = Extensions.Constants.DateTimeFomat, ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
        public string FinishDate { get; set; }

        [ControlText(Type = ControlText.TextBox, LabelText = "Tiêu đề", Required = true, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 1)]
        public string Title { get; set; }

        [ControlText(Type = ControlText.TextBox, LabelText = "Tiêu đề không dấu", MaxLength = 200, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 1)]
        public string Alias { get; set; }

        [ControlText(Type = ControlText.TextBox, LabelText = "Nơi làm việc", Required = true, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 2)]
        public string Position { get; set; }
        
        [ControlText(Type = ControlText.TextBox, LabelText = "Thời gian làm việc", Required = true, MaxLength = 200, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 2)]
        public string TimeWork { get; set; }

        [ControlText(LabelText = "Tóm tắt nội dung", Required = true, PlaceHolder = "Nhập tối đa 400 ký tự.", Type = ControlText.MultiText, Rows = 2, MaxLength = 400, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 3)]
        public string Summary { get; set; }

        [ControlText(LabelText = "Nội dung bài viết", Required = false, Type = ControlText.RichText, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 4)]
        public string Contents { get; set; }

        public static implicit operator RecruitmentModel(RecruitmentInfo entity)
        {
            return new RecruitmentModel
            {
                Id = entity.Id,
                CategoryId = entity.CategoryId,
                Title = entity.Title,
                Alias = entity.Alias,
                Position = entity.Position,
                Summary = entity.Summary,
                Contents = entity.Contents,
                FinishDate = entity.FinishDate.ToString(Extensions.Constants.DateTimeFomat),
                TimeWork = entity.TimeWork
            };
        }
    }
}
