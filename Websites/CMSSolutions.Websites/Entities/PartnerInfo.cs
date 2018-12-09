using System.ComponentModel.DataAnnotations.Schema;
using CMSSolutions.Websites.Extensions;

namespace CMSSolutions.Websites.Entities
{
    using System.ComponentModel;
    using CMSSolutions.Data;
    using CMSSolutions.Data.Entity;
    using System.Runtime.Serialization;
    
    
    [DataContract()]
    public class PartnerInfo : BaseEntity<int>
    {
        
        [DataMember()]
        [DisplayName("LanguageCode")]
        public string LanguageCode { get; set; }

        [DataMember()]
        [DisplayName("RefId")]
        public int RefId { get; set; }
        
        [DataMember()]
        [DisplayName("Logo")]
        public string Logo { get; set; }

        [NotMapped]
        [DisplayName(Constants.NotMapped)]
        public string LogoResize
        {
            get
            {
                if (!string.IsNullOrEmpty(Logo))
                {
                    //return ResizePhoto.Resize(Logo, Extensions.Constants.WidthPartner, Extensions.Constants.HeightPartner);
                    return Logo;
                }

                return string.Empty;
            }
        }

        [DataMember()]
        [DisplayName("ShortName")]
        public string ShortName { get; set; }
        
        [DataMember()]
        [DisplayName("FullName")]
        public string FullName { get; set; }
        
        [DataMember()]
        [DisplayName("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [DataMember()]
        [DisplayName("Email")]
        public string Email { get; set; }
        
        [DataMember()]
        [DisplayName("Website")]
        public string Website { get; set; }
        
        [DataMember()]
        [DisplayName("CreateDate")]
        public System.Nullable<System.DateTime> CreateDate { get; set; }
        
        [DataMember()]
        [DisplayName("Address")]
        public string Address { get; set; }
        
        [DataMember()]
        [DisplayName("Description")]
        public string Description { get; set; }

        [DataMember()]
        [DisplayName("SortOrder")]
        public int SortOrder { get; set; }
        
        [DataMember()]
        [DisplayName("IsDeleted")]
        public bool IsDeleted { get; set; }
    }
    
    public class PartnerMapping : EntityTypeConfiguration<PartnerInfo>, IEntityTypeConfiguration
    {
        
        public PartnerMapping()
        {
            this.ToTable("Modules_Partner");
            this.HasKey(m => m.Id);
            this.Property(m => m.LanguageCode).HasMaxLength(50);
            this.Property(m => m.Logo).IsRequired().HasMaxLength(500);
            this.Property(m => m.ShortName).IsRequired().HasMaxLength(250);
            this.Property(m => m.FullName).IsRequired().HasMaxLength(250);
            this.Property(m => m.PhoneNumber).HasMaxLength(50);
            this.Property(m => m.Email).HasMaxLength(50);
            this.Property(m => m.Website).HasMaxLength(500);
            this.Property(m => m.CreateDate);
            this.Property(m => m.Address).HasMaxLength(250);
            this.Property(m => m.Description).HasMaxLength(500);
            this.Property(m => m.IsDeleted);
        }
    }
}
