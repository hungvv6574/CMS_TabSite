namespace CMSSolutions.Websites.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel;
    using CMSSolutions.Data;
    using CMSSolutions.Data.Entity;
    using System.Runtime.Serialization;
    
    
    [DataContract()]
    public class EmailInfo : BaseEntity<int>
    {
        
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
        [DisplayName("Notes")]
        public string Notes { get; set; }
        
        [DataMember()]
        [DisplayName("IsBlocked")]
        public bool IsBlocked { get; set; }
    }
    
    public class EmailsMapping : EntityTypeConfiguration<EmailInfo>, IEntityTypeConfiguration
    {
        
        public EmailsMapping()
        {
            this.ToTable("Modules_Emails");
            this.HasKey(m => m.Id);
            this.Property(m => m.FullName).HasMaxLength(250);
            this.Property(m => m.PhoneNumber).HasMaxLength(50);
            this.Property(m => m.Email).IsRequired().HasMaxLength(50);
            this.Property(m => m.Notes).HasMaxLength(2000);
            this.Property(m => m.IsBlocked).IsRequired();
        }
    }
}
