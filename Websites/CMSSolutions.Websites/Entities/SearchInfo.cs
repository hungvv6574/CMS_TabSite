namespace CMSSolutions.Websites.Entities
{
    using System.ComponentModel;
    using CMSSolutions.Data;
    using CMSSolutions.Data.Entity;
    using System.Runtime.Serialization;

    public enum SearchField
    {
        Id, LanguageCode, CategoryId, Type, SearchId, Title, Alias, Sumary, Tags, Keyword, Url
    }

    [DataContract()]
    public class SearchInfo : BaseEntity<long>
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
        [DisplayName("Url")]
        public string Url { get; set; }
        
        [DataMember()]
        [DisplayName("Type")]
        public int Type { get; set; }
        
        [DataMember()]
        [DisplayName("SearchId")]
        public string SearchId { get; set; }
        
        [DataMember()]
        [DisplayName("Title")]
        public string Title { get; set; }
        
        [DataMember()]
        [DisplayName("Alias")]
        public string Alias { get; set; }
        
        [DataMember()]
        [DisplayName("Sumary")]
        public string Sumary { get; set; }

        [DataMember()]
        [DisplayName("VideoUrl")]
        public string VideoUrl { get; set; }

        [DataMember()]
        [DisplayName("Images")]
        public string Images { get; set; }
        
        [DataMember()]
        [DisplayName("Tags")]
        public string Tags { get; set; }
        
        [DataMember()]
        [DisplayName("CreateDate")]
        public System.DateTime CreateDate { get; set; }
    }
    
    public class SearchMapping : EntityTypeConfiguration<SearchInfo>, IEntityTypeConfiguration
    {
        
        public SearchMapping()
        {
            this.ToTable("Modules_Search");
            this.HasKey(m => m.Id);
            this.Property(m => m.LanguageCode).IsRequired().HasMaxLength(50);
            this.Property(m => m.CategoryId).IsRequired();
            this.Property(m => m.Type).IsRequired();
            this.Property(m => m.SearchId).IsRequired().HasMaxLength(50);
            this.Property(m => m.Title).IsRequired().HasMaxLength(250);
            this.Property(m => m.Alias).IsRequired().HasMaxLength(250);
            this.Property(m => m.Sumary).HasMaxLength(500);
            this.Property(m => m.VideoUrl).HasMaxLength(500);
            this.Property(m => m.Images).HasMaxLength(500);
            this.Property(m => m.Tags).HasMaxLength(400);
            this.Property(m => m.CreateDate).IsRequired();
            this.Property(m => m.Url).HasMaxLength(500).IsRequired();
        }
    }
}
