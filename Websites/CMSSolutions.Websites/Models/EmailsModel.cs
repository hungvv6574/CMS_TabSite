namespace CMSSolutions.Websites.Models
{
    using System;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;
    
    
    public class EmailsModel
    {
        public EmailsModel()
        {
            IsBlocked = false;
        }

        [ControlHidden()]
        public int Id { get; set; }
        
        [ControlText(Type = ControlText.TextBox, LabelText = "Họ và tên", Required = true, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
        public string FullName { get; set; }

        [ControlText(Type = ControlText.TextBox, Required = false, LabelText = "Số điện thoại", MaxLength = 50, ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
        public string PhoneNumber { get; set; }

        [ControlText(Type = ControlText.Email, Required = true, LabelText = "Địa chỉ email", MaxLength = 50, ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
        public string Email { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", PrependText = "Đang khóa", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
        public bool IsBlocked { get; set; }
        
        [ControlText(Type=ControlText.MultiText, Rows = 3, Required=false, MaxLength=2000, ContainerCssClass=Constants.ContainerCssClassCol12, ContainerRowIndex=1)]
        public string Notes { get; set; }
        
        public static implicit operator EmailsModel(EmailInfo entity)
        {
            return new EmailsModel
            {
                Id = entity.Id,
                FullName = entity.FullName,
                PhoneNumber = entity.PhoneNumber,
                Email = entity.Email,
                Notes = entity.Notes,
                IsBlocked = entity.IsBlocked
            };
        }
    }
}
