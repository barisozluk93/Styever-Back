namespace UserManagement.Model
{
    public class AgreementAcceptanceRequest
    {
        public long UserId { get; set; }
        public string AgreementType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Version { get; set; } = "1.0";
        public string Language { get; set; } = "tr";
        public string Context { get; set; } = string.Empty;
        public string? DocumentUrl { get; set; }
        public string? ContentSnapshot { get; set; }
        public string? RelatedReference { get; set; }
    }
}
