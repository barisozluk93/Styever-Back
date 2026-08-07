using UserManagement.Entity;

namespace UserManagement.Model
{
    public class ShopierCheckoutResponse
    {
        public Guid Reference { get; set; }
        public string RedirectUrl { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class ShopierPaymentStatusResponse
    {
        public Guid Reference { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ShopierOrderId { get; set; }
    }

    public class ShopierOsbResult
    {
        public bool IsAuthenticated { get; set; }
        public bool IsTest { get; set; }
        public bool IsProcessed { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? Reference { get; set; }
        public string? ShopierOrderId { get; set; }
    }

    public class ShopierOptions
    {
        public string BaseUrl { get; set; } = "https://api.shopier.com/v1";
        public string AccessToken { get; set; } = string.Empty;
        public int SearchWindowHours { get; set; } = 24;
        public string OsbUsername { get; set; } = string.Empty;
        public string OsbPassword { get; set; } = string.Empty;
        public int OsbPendingWindowDays { get; set; } = 7;
        public Dictionary<long, ShopierProductOptions> Products { get; set; } = new();
    }

    public class ShopierProductOptions
    {
        public string ProductId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
