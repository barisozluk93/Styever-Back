namespace UserManagement.Entity
{
    public class ShopierPayment
    {
        public long Id { get; set; }
        public Guid Reference { get; set; }
        public long? UserId { get; set; }
        public long PlanId { get; set; }
        public long MemoryId { get; set; }
        public string PurchaseType { get; set; } = "Package";
        public string ProductId { get; set; } = string.Empty;
        public string ProductUrl { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
        public string? GiftPayload { get; set; }
        public string Status { get; set; } = "Pending";
        public string? ShopierOrderId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
