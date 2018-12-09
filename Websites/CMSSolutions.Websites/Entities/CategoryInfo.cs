using System.ComponentModel.DataAnnotations.Schema;

namespace CMSSolutions.Websites.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel;
    using CMSSolutions.Data;
    using CMSSolutions.Data.Entity;
    using System.Runtime.Serialization;
    
    
    [DataContract()]
    public class CategoryInfo : BaseEntity<int>
    {
        
        [DataMember()]
        [DisplayName("LanguageCode")]
        public string LanguageCode { get; set; }

        [DataMember()]
        [DisplayName("RefId")]
        public int RefId { get; set; }
        
        [DataMember()]
        [DisplayName("ParentId")]
        public int ParentId { get; set; }

        [NotMapped]
        [DisplayName(Constants.NotMapped)]
        public string ParentName { get; set; }

        [DataMember()]
        [DisplayName("ShortName")]
        public string ShortName { get; set; }
        
        [DataMember()]
        [DisplayName("Name")]
        public string Name { get; set; }
        
        [DataMember()]
        [DisplayName("Alias")]
        public string Alias { get; set; }
        
        [DataMember()]
        [DisplayName("IsHome")]
        public bool IsHome { get; set; }
        
        [DataMember()]
        [DisplayName("HasChilden")]
        public bool HasChilden { get; set; }

        [NotMapped]
        [DisplayName(Constants.NotMapped)]
        public string ChildenName { get; set; }

        [DataMember()]
        [DisplayName("CreateDate")]
        public System.DateTime CreateDate { get; set; }
        
        [DataMember()]
        [DisplayName("Notes")]
        public string Notes { get; set; }
        
        [DataMember()]
        [DisplayName("Description")]
        public string Description { get; set; }
        
        [DataMember()]
        [DisplayName("Tags")]
        public string Tags { get; set; }
        
        [DataMember()]
        [DisplayName("Url")]
        public string Url { get; set; }
        
        [DataMember()]
        [DisplayName("IsActived")]
        public bool IsActived { get; set; }

        [DataMember()]
        [DisplayName("IsDisplayMenu")]
        public bool IsDisplayMenu { get; set; }

        [DataMember()]
        [DisplayName("IsDisplayFooter")]
        public bool IsDisplayFooter { get; set; }

        [DataMember()]
        [DisplayName("OrderBy")]
        public int OrderBy { get; set; }

        [DataMember()]
        [DisplayName("MenuOrderBy")]
        public int MenuOrderBy { get; set; }
        
        [DataMember()]
        [DisplayName("IsDeleted")]
        public bool IsDeleted { get; set; }
    }
    
    public class CategoriesMapping : EntityTypeConfiguration<CategoryInfo>, IEntityTypeConfiguration
    {
        
        public CategoriesMapping()
        {
            this.ToTable("Modules_Categories");
            this.HasKey(m => m.Id);
            this.Property(m => m.LanguageCode).HasMaxLength(50);
            this.Property(m => m.ParentId).IsRequired();
            this.Property(m => m.ShortName).IsRequired().HasMaxLength(250);
            this.Property(m => m.Name).IsRequired().HasMaxLength(400);
            this.Property(m => m.Alias).IsRequired().HasMaxLength(400);
            this.Property(m => m.IsHome).IsRequired();
            this.Property(m => m.HasChilden).IsRequired();
            this.Property(m => m.CreateDate).IsRequired();
            this.Property(m => m.Description).IsRequired().HasMaxLength(2000);
            this.Property(m => m.Tags).IsRequired().HasMaxLength(2000);
            this.Property(m => m.Url).IsRequired();
            this.Property(m => m.IsActived).IsRequired();
            this.Property(m => m.IsDisplayMenu).IsRequired();
            this.Property(m => m.IsDisplayFooter).IsRequired();
            this.Property(m => m.OrderBy).IsRequired();
            this.Property(m => m.MenuOrderBy).IsRequired();
            this.Property(m => m.IsDeleted).IsRequired();
        }
    }
}
