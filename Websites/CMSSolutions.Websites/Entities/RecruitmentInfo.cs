namespace CMSSolutions.Websites.Entities
{
    using System.ComponentModel;
    using CMSSolutions.Data;
    using CMSSolutions.Data.Entity;
    using System.Runtime.Serialization;

    [DataContract()]
    public class RecruitmentInfo : BaseEntity<int>
    {
        
        [DataMember()]
        [DisplayName("LanguageCode")]
        public string LanguageCode { get; set; }

        [DataMember()]
        [DisplayName("RefId")]
        public int RefId { get; set; }
        
        [DataMember()]
        [DisplayName("CategoryId")]
        public int CategoryId { get; set; }
        
        [DataMember()]
        [DisplayName("Title")]
        public string Title { get; set; }
        
        [DataMember()]
        [DisplayName("Position")]
        public string Position { get; set; }
        
        [DataMember()]
        [DisplayName("TimeWork")]
        public string TimeWork { get; set; }
        
        [DataMember()]
        [DisplayName("Alias")]
        public string Alias { get; set; }
        
        [DataMember()]
        [DisplayName("Summary")]
        public string Summary { get; set; }
        
        [DataMember()]
        [DisplayName("Contents")]
        public string Contents { get; set; }
        
        [DataMember()]
        [DisplayName("FinishDate")]
        public System.DateTime FinishDate { get; set; }
        
        [DataMember()]
        [DisplayName("CreateDate")]
        public System.DateTime CreateDate { get; set; }
        
        [DataMember()]
        [DisplayName("CreateByUser")]
        public int CreateByUser { get; set; }
        
        [DataMember()]
        [DisplayName("Company")]
        public string Company { get; set; }
        
        [DataMember()]
        [DisplayName("ContactName")]
        public string ContactName { get; set; }
        
        [DataMember()]
        [DisplayName("ContactAddress")]
        public string ContactAddress { get; set; }
        
        [DataMember()]
        [DisplayName("ContactEmail")]
        public string ContactEmail { get; set; }
        
        [DataMember()]
        [DisplayName("ContactMobile")]
        public string ContactMobile { get; set; }
        
        [DataMember()]
        [DisplayName("IsDeleted")]
        public bool IsDeleted { get; set; }
    }
    
    public class RecruitmentMapping : EntityTypeConfiguration<RecruitmentInfo>, IEntityTypeConfiguration
    {
        public RecruitmentMapping()
        {
            this.ToTable("Modules_Recruitment");
            this.HasKey(m => m.Id);
            this.Property(m => m.LanguageCode).HasMaxLength(50);
            this.Property(m => m.CategoryId).IsRequired();
            this.Property(m => m.Title).IsRequired().HasMaxLength(250);
            this.Property(m => m.Position).HasMaxLength(250).IsRequired();
            this.Property(m => m.Alias).IsRequired().HasMaxLength(250);
            this.Property(m => m.Summary).IsRequired().HasMaxLength(400);
            this.Property(m => m.Contents).IsRequired();
            this.Property(m => m.FinishDate).IsRequired();
            this.Property(m => m.CreateDate).IsRequired();
            this.Property(m => m.CreateByUser).IsRequired();
            this.Property(m => m.ContactName).HasMaxLength(250);
            this.Property(m => m.ContactAddress).HasMaxLength(250);
            this.Property(m => m.ContactEmail).HasMaxLength(50);
            this.Property(m => m.ContactMobile).HasMaxLength(50);
            this.Property(m => m.TimeWork).IsRequired().HasMaxLength(200);
        }
    }
}
