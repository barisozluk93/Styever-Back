using UserManagement.Entity;

namespace UserManagement.Interfaces
{
    public interface IPurchaseDocumentService
    {
        Task SendPurchaseDocumentsAsync(
            ShopierPayment payment,
            string? shopierOrderId,
            CancellationToken cancellationToken = default);
    }
}
