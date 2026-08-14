namespace UserManagement.Model
{
    public class PagingParameter
    {
        public string? FilterText { get; set; }
        public long? Id { get; set; }
        public string? Category { get; set; }
        public string? Title { get; set; }
        public string? TitleEn { get; set; }
        public string? Slug { get; set; }
        public int? SortOrder { get; set; }
        public string? Name { get; set; }
        public string? NameSurname { get; set; }
        public string? Code { get; set; }
        public string? Surname { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
        public long? RoleId { get; set; }
        public DateTime? CreatedDateFrom { get; set; }
        public DateTime? CreatedDateTo { get; set; }
        const int maxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize { get => _pageSize; set => _pageSize = value > maxPageSize ? maxPageSize : value; }
    }
}
