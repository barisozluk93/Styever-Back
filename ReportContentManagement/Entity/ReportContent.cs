namespace ReportContentManagement.Entity
{
    public class ReportContent
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ComplaintType { get; set; }
        public string ReportedUrl { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
    }
}
