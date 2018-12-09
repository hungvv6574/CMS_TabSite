namespace CMSSolutions.Websites.Entities
{
    using System.ComponentModel;
    using CMSSolutions.Data;
    using CMSSolutions.Data.Entity;
    using System.Runtime.Serialization;
    
    
    [DataContract()]
    public class ImageInfo : BaseEntity<long>
    {
        [DataMember()]
        [DisplayName("ArticlesId")]
        public int ArticlesId { get; set; }
        
        [DataMember()]
        [DisplayName("LanguageCode")]
        public string LanguageCode { get; set; }
        
        [DataMember()]
        [DisplayName("CategoryId")]
        public int CategoryId { get; set; }
        
        [DataMember()]
        [DisplayName("SortOrder")]
        public int SortOrder { get; set; }
        
        [DataMember()]
        [DisplayName("Url")]
        public string Url { get; set; }
        
        [DataMember()]
        [DisplayName("ListCategory")]
        public string ListCategory { get; set; }
        
        [DataMember()]
        [DisplayName("Caption")]
        public string Caption { get; set; }
        
        [DataMember()]
        [DisplayName("FilePath")]
        public string FilePath { get; set; }
    }
    
    public class ImagesMapping : EntityTypeConfiguration<ImageInfo>, IEntityTypeConfiguration
    {
        
        public ImagesMapping()
        {
            this.ToTable("Modules_Images");
            this.HasKey(m => m.Id);
            this.Property(m => m.LanguageCode).IsRequired().HasMaxLength(50);
            this.Property(m => m.SortOrder).IsRequired();
            this.Property(m => m.Url).HasMaxLength(500);
            this.Property(m => m.ListCategory).HasMaxLength(250);
            this.Property(m => m.Caption).HasMaxLength(250);
            this.Property(m => m.FilePath).IsRequired().HasMaxLength(500);
        }
    }
}
