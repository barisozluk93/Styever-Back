using UserManagement.Entity;
using UserManagement.Model;

namespace UserManagement.Interfaces
{
    public interface IUserService
    {
        Task<Result<PagingResult<PagedList<User>>>> Paginate(PagingParameter pagingParameter);
        Task<Result<List<User>>> GetUsers();
        Task<Result<User>> Save(User user);
        Task<Result<User>> Update(User user);
        Task<Result<User>> Delete(long id);
        Task<Result<User>> GetById(long id, string token);
        Task<Result<User>> UserAvatarUpdate(long id, long fileId);
        Task<Result<List<String>>> GetUserPermissions(string token);
        Task<Result<List<UserAddress>>> GetUserAddresses(long userId);
        Task<Result<UserAddress>> UserAddressSave(UserAddress user);
        Task<Result<UserAddress>> UserAddressUpdate(UserAddress user);
        Task<Result<UserAddress>> UserAddressDelete(long id);
        Task<Result<UserAddress>> GetUserAddressById(long id);
        Task<Result<UserAddress>> GetPrimaryUserAddressById(long userId);
        Task<Result<bool>> Pay(long userId);
        Task<Result<bool>> BuyPackage(long userId, long planId, long memoryId);
        Task<Result<UserVoucher>> BuyGiftPackage(UserVoucher userVoucher);
        Task<Result<UserVoucher>> VoucherControl(string voucher);
        Task SendMailAsync(
            string to,
            string subject,
            string htmlBody,
            string textBody,
            IEnumerable<MailAttachment>? attachments = null,
            CancellationToken cancellationToken = default);
    }
}
