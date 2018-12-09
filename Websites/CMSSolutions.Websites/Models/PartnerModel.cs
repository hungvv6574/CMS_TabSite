namespace CMSSolutions.Websites.Models
{
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;

    public class PartnerModel
    {
        public PartnerModel()
        {
            SortOrder = 0;
        }

        [ControlHidden()]
        public int Id { get; set; }

        [ControlText(Type = ControlText.TextBox, LabelText = "Tên viết tắt", PlaceHolder = "Nhập tối đa 250 ký tự.", Required = true, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
        public string ShortName { get; set; }

        [ControlText(Type = ControlText.TextBox, LabelText = "Tên đầy đủ", PlaceHolder = "Nhập tối đa 250 ký tự.", Required = true, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 0)]
        public string FullName { get; set; }

        [ControlNumeric(Required = false, LabelText = "Thứ tự", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 1)]
        public int SortOrder { get; set; }

        [ControlFileUpload(EnableFineUploader = true, Required = true, LabelText = "Logo", ContainerCssClass = Constants.ContainerCssClassCol9, ContainerRowIndex = 1, ShowThumbnail = true)]
        public string Logo { get; set; }

        [ControlText(Type = ControlText.TextBox, LabelText = "Số điện thoại", Required = false, MaxLength = 50, ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 2)]
        public string PhoneNumber { get; set; }

        [ControlText(Type = ControlText.Email, LabelText = "Email", Required = false, MaxLength = 50, ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 2)]
        public string Email { get; set; }

        [ControlText(Type = ControlText.Url, LabelText = "Website", Required = false, MaxLength = 500, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 2)]
        public string Website { get; set; }

        [ControlText(Type = ControlText.TextBox, LabelText = "Địa chỉ", Required = false, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 3)]
        public string Address { get; set; }

        [ControlText(Type = ControlText.MultiText, Rows = 3, LabelText = "Giới thiệu", Required = false, MaxLength = 500, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 3)]
        public string Description { get; set; }

        public static implicit operator PartnerModel(PartnerInfo entity)
        {
            return new PartnerModel
            {
                Id = entity.Id,
                Logo = entity.Logo,
                ShortName = entity.ShortName,
                FullName = entity.FullName,
                PhoneNumber = entity.PhoneNumber,
                Website = entity.Website,
                Address = entity.Address,
                Description = entity.Description,
                SortOrder = entity.SortOrder
            };
        }
    }
}
