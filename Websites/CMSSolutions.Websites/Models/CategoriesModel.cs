namespace CMSSolutions.Websites.Models
{
    using System;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;
    
    
    public class CategoriesModel
    {
        public CategoriesModel()
        {
            IsActived = true;
            IsDisplayFooter = false;
            IsDisplayMenu = true;
        }

        [ControlHidden]
        public int Id { get; set; }

        [ControlChoice(ControlChoice.DropDownList, LabelText = "Chuyên mục gốc", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0, OnSelectedIndexChanged = "getUrl(this.value);")]
        public int ParentId { get; set; }

        [ControlNumeric(LabelText = "Vị trí(Thứ tự mục cha, footer)", Required = true, MaxLength = 3, ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
        public int OrderBy { get; set; }

        [ControlNumeric(LabelText = "Vị trí trên menu", Required = true, MaxLength = 3, ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
        public int MenuOrderBy { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", PrependText = "Có chuyên mục con", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 1)]
        public bool HasChilden { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", PrependText = "Làm trang chủ", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 1)]
        public bool IsHome { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", PrependText = "Được hiển thị", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 1)]
        public bool IsActived { get; set; }

        [ControlText(Type = ControlText.TextBox, LabelText = "Tên rút gọn", PlaceHolder = "Tối đa 250 ký tự", Required = true, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 2)]
        public string ShortName { get; set; }

        [ControlText(Type = ControlText.TextBox, LabelText = "Tên đầy đủ", PlaceHolder = "Tối đa 400 ký tự", Required = true, MaxLength = 400, ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 2)]
        public string Name { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", PrependText = "Hiển thị menu", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 2)]
        public bool IsDisplayMenu { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", PrependText = "Hiển thị footer", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 2)]
        public bool IsDisplayFooter { get; set; }

        [ControlText(Type = ControlText.TextBox, LabelText = "Tiêu đề không dấu", MaxLength = 200, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 3)]
        public string Alias { get; set; }

        [ControlText(Type = ControlText.TextBox, LabelText = "Đường dẫn", PlaceHolder = "Tối đa 250 ký tự", MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 3)]
        public string Url { get; set; }

        [ControlText(LabelText = "Ghi chú", Required = false, Type = ControlText.RichText, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 5)]
        public string Notes { get; set; }

        [ControlText(LabelText = "Mô tả SEO", PlaceHolder = "Tối đa 2000 ký tự", Type = ControlText.MultiText, Required = true, Rows = 2, MaxLength = 2000, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 6)]
        public string Description { get; set; }

        [ControlText(LabelText = "Tags SEO", PlaceHolder = "Tối đa 2000 ký tự", Type = ControlText.MultiText, Required = true, Rows = 2, MaxLength = 2000, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 6)]
        public string Tags { get; set; }
        
        public static implicit operator CategoriesModel(CategoryInfo entity)
        {
            return new CategoriesModel
            {
                Id = entity.Id,
                ParentId = entity.ParentId,
                ShortName = entity.ShortName,
                Name = entity.Name,
                IsHome = entity.IsHome,
                HasChilden = entity.HasChilden,
                Notes = entity.Notes,
                Description = entity.Description,
                Tags = entity.Tags,
                Url = entity.Url,
                IsActived = entity.IsActived,
                OrderBy = entity.OrderBy,
                MenuOrderBy = entity.MenuOrderBy,
                IsDisplayMenu = entity.IsDisplayMenu,
                IsDisplayFooter = entity.IsDisplayFooter,
                Alias = entity.Alias
            };
        }
    }
}
