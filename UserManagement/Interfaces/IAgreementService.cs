using UserManagement.Entity;
using UserManagement.Model;
namespace UserManagement.Interfaces
{
    public interface IAgreementService
    {
        Task<Result<List<UserAgreementAcceptance>>> Accept(List<AgreementAcceptanceRequest> requests, string? ipAddress, string? userAgent);
        Task<Result<List<UserAgreementAcceptance>>> GetByUser(long userId);
    }
}
