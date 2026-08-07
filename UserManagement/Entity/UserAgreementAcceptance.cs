using System.ComponentModel.DataAnnotations;

namespace UserManagement.Entity
{
    public class UserAgreementAcceptance
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        [MaxLength(80)] public string AgreementType { get; set; } = string.Empty;
        [MaxLength(250)] public string Title { get; set; } = string.Empty;
        [MaxLength(40)] public string Version { get; set; } = "1.0";
        [MaxLength(10)] public string Language { get; set; } = "tr";
        [MaxLength(40)] public string Context { get; set; } = string.Empty;
        [MaxLength(500)] public string? DocumentUrl { get; set; }
        public string? ContentSnapshot { get; set; }
        [MaxLength(100)] public string? RelatedReference { get; set; }
        [MaxLength(64)] public string? IpAddress { get; set; }
        [MaxLength(1000)] public string? UserAgent { get; set; }
        public DateTime AcceptedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
