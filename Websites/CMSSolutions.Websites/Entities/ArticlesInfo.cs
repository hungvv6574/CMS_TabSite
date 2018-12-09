using System.ComponentModel.DataAnnotations.Schema;

namespace CMSSolutions.Websites.Entities
{
    using System.ComponentModel;
    using CMSSolutions.Data;
    using CMSSolutions.Data.Entity;
    using System.Runtime.Serialization;
    
    
    [DataContract()]
    public class ArticlesInfo : BaseEntity<int>
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

        [NotMapped]
        [DisplayName("CategoryAlias")]
        public string CategoryAlias { get; set; }

        [NotMapped]
        [DisplayName("CategoryName")]
        public string CategoryName { get; set; }
        
        [DataMember()]
        [DisplayName("Title")]
        public string Title { get; set; }
        
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
        [DisplayName("CreateDate")]
        public System.DateTime CreateDate { get; set; }
        
        [DataMember()]
        [DisplayName("CreateByUser")]
        public int CreateByUser { get; set; }
        
        [DataMember()]
        [DisplayName("IsPublished")]
        public bool IsPublished { get; set; }
        
        [DataMember()]
        [DisplayName("PublishedDate")]
        public System.Nullable<System.DateTime> PublishedDate { get; set; }
        
        [DataMember()]
        [DisplayName("Year")]
        public int Year { get; set; }
        
        [DataMember()]
        [DisplayName("StartDate")]
        public System.Nullable<System.DateTime> StartDate { get; set; }
        
        [DataMember()]
        [DisplayName("EndDate")]
        public System.Nullable<System.DateTime> EndDate { get; set; }
        
        [DataMember()]
        [DisplayName("VideoUrl")]
        public string VideoUrl { get; set; }
        
        [DataMember()]
        [DisplayName("Image")]
        public string Image { get; set; }
        
        [DataMember()]
        [DisplayName("ViewCount")]
        public int ViewCount { get; set; }
        
        [DataMember()]
        [DisplayName("Description")]
        public string Description { get; set; }
        
        [DataMember()]
        [DisplayName("Tags")]
        public string Tags { get; set; }
        
        [DataMember()]
        [DisplayName("IsDeleted")]
        public bool IsDeleted { get; set; }
    }
    
    public class ArticlesMapping : EntityTypeConfiguration<ArticlesInfo>, IEntityTypeConfiguration
    {
        
        public ArticlesMapping()
        {
            this.ToTable("Modules_Articles");
            this.HasKey(m => m.Id);
            this.Property(m => m.LanguageCode).HasMaxLength(50);
            this.Property(m => m.CategoryId).IsRequired();
            this.Property(m => m.Title).IsRequired().HasMaxLength(200);
            this.Property(m => m.Alias).IsRequired().HasMaxLength(200);
            this.Property(m => m.Summary).IsRequired().HasMaxLength(400);
            this.Property(m => m.CreateDate).IsRequired();
            this.Property(m => m.CreateByUser).IsRequired();
            this.Property(m => m.IsPublished).IsRequired();
            this.Property(m => m.PublishedDate);
            this.Property(m => m.Year).IsRequired();
            this.Property(m => m.VideoUrl).HasMaxLength(500);
            this.Property(m => m.Image).IsRequired().HasMaxLength(300);
            this.Property(m => m.ViewCount);
            this.Property(m => m.Description).HasMaxLength(500);
            this.Property(m => m.Tags).HasMaxLength(500);
            this.Property(m => m.IsDeleted).IsRequired();
        }
    }
}
