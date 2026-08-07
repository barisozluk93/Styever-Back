using UserManagement.Entity;
using UserManagement.Model;

namespace UserManagement.Interfaces
{
    public interface IShopierPaymentService
    {
        Task<Result<ShopierCheckoutResponse>> StartPay(long userId);
        Task<Result<ShopierCheckoutResponse>> StartPackage(long userId, long planId, long memoryId);
        Task<Result<ShopierCheckoutResponse>> StartGift(UserVoucher voucher);
        Task<Result<ShopierCheckoutResponse>> GetPending(long userId, string purchaseType, long planId, long memoryId);
        Task<Result<ShopierPaymentStatusResponse>> Confirm(Guid reference);
        Task<Result<ShopierPaymentStatusResponse>> GetStatus(Guid reference);
        Task<ShopierOsbResult> HandleOsbAsync(
            IReadOnlyDictionary<string, string> form,
            string? authorizationHeader,
            CancellationToken cancellationToken = default);
    }
}
