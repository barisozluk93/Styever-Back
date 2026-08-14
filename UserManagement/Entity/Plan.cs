namespace UserManagement.Entity
{
    public class Plan
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Currency { get; set; } = "₺";
        public string Period { get; set; } = "Yıl";
        public string PeriodEn { get; set; } = "Year";
        public string Properties { get; set; } = string.Empty;
        public string PropertiesEn { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool IsPopular { get; set; }
        public bool IsDeleted { get; set; }
    }
}
