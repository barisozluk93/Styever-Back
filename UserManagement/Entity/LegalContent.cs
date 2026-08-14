namespace UserManagement.Entity
{
    public class LegalContent
    {
        public long Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ContentEn { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool IsDeleted { get; set; }
    }
}
