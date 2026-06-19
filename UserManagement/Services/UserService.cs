using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserManagement.DbContexts;
using UserManagement.Entity;
using UserManagement.Interfaces;
using UserManagement.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UserManagement.Services
{
    public class UserService : IUserService
    {
        const int keySize = 64;
        const int iterations = 350000;
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        private readonly UserManagementContext _dbContext;

        private readonly IConfiguration configuration;

        private readonly MailSettings _mailSettings;

        public UserService(UserManagementContext dbContext, IConfiguration configuration, MailSettings mailSettings)
        {
            _dbContext = dbContext;
            this.configuration = configuration;
            _mailSettings = mailSettings;
        }

        public async Task<Result<PagingResult<PagedList<User>>>> Paginate(PagingParameter pagingParameter)
        {
            var result = new Result<PagingResult<PagedList<User>>>();

            string lowerFilterText = string.IsNullOrEmpty(pagingParameter.FilterText) ? null : pagingParameter.FilterText.ToLower();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var queryable = _dbContext.Users
                        .Where(x => (string.IsNullOrEmpty(lowerFilterText) || (x.Name.ToLower().Contains(lowerFilterText)) || x.Surname.ToLower().Contains(lowerFilterText)))
                        .Select(s => new User()
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Surname = s.Surname,
                        Username = s.Username,
                        IsDeleted = s.IsDeleted,
                        Email = s.Email,
                        Phone = s.Phone,
                        IsSystemData = s.IsSystemData,
                        Permissions = _dbContext.UserPermissions.Include(p => p.Permission).Where(x => !x.IsDeleted && x.UserId == s.Id).Select(p => p.Permission).ToList(),
                        Roles = _dbContext.UserRoles.Where(x => !x.IsDeleted && x.UserId == s.Id).Select(p => p.RoleId).ToList(),
                    });

                    var pagination = PagedList<User>.ToPagedList(queryable, pagingParameter.PageNumber, pagingParameter.PageSize);

                    result.SetData(new PagingResult<PagedList<User>>()
                    {
                        Items = pagination,
                        TotalCount = pagination.TotalCount,
                    });

                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<bool>> BuyPackage(long userId, long planId, long memoryId)
        {
            var result = new Result<bool>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var res = true;

                    if (memoryId > 0)
                    {
                        await SetBelongingIssuesToTrueUserMemory(userId, memoryId);
                    }

                    if(planId == 4)
                    {
                        await SetBelongingIssuesToFalseUserMemory(userId);
                    }

                    if (res)
                    {
                        var user = await _dbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
                        user.IsActive = true;
                        user.CreatedDate = DateTime.UtcNow;
                        user.ExpirationDate = DateTime.UtcNow.AddYears(1);

                        var userRole = await _dbContext.UserRoles.Where(x => x.UserId == userId).FirstOrDefaultAsync();
                        userRole.RoleId = planId;

                        UserPayment payment = new UserPayment();
                        payment.UserId = userId;
                        payment.Price = planId == 2 ? 359.00 : planId == 3 ? 559.00 : 959.00;
                        payment.PlanId = planId;
                        payment.PaymentDate = DateTime.UtcNow;
                        payment.IsDeleted = false;

                        _dbContext.Add(payment);
                        await _dbContext.SaveChangesAsync();

                        transaction.Commit();

                        result.SetData(true);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("İşlem sırasında bir hata oluştu.");
                    }
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(exception.Message);
                }
            }

            return result;
        }

        private async Task SendGiftMessage(UserVoucher userVoucher, User? user, string link)
        {
            var senderName = user != null
                ? $"{user.Name} {user.Surname}"
                : userVoucher.SenderEmail;

            var receiverName = "Değerli Dostumuz";
            var petName = "Dostunuz";

            var headerImagePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "email",
                "styever-gift-header.png"
            );

            var builder = new BodyBuilder();

            var headerImage = builder.LinkedResources.Add(headerImagePath);
            headerImage.ContentId = MimeUtils.GenerateMessageId();

            string html = $@"
<!DOCTYPE html>
<html lang='tr'>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>

<body style='margin:0;padding:0;background:#dcefe5;font-family:Arial,Helvetica,sans-serif;'>

<table width='100%' cellpadding='0' cellspacing='0' border='0'
       style='background:linear-gradient(180deg,#b8ded3 0%,#f5eee2 100%);padding:55px 0;'>

<tr>
<td align='center'>

<table width='600' cellpadding='0' cellspacing='0' border='0'
       style='width:600px;max-width:600px;background:#fffdf7;border-radius:14px;overflow:hidden;
              box-shadow:0 14px 35px rgba(0,0,0,0.25);border:1px solid rgba(0,0,0,0.18);'>

<tr>
<td>
<img src='cid:{headerImage.ContentId}'
     width='600'
     alt='Styever'
     style='width:600px;max-width:600px;height:auto;display:block;border:0;' />
</td>
</tr>

<tr>
<td style='padding:24px 34px 18px 34px;color:#111111;font-size:16px;line-height:1.35;'>

<p style='margin:0 0 18px 0;'>Merhaba <strong>{receiverName}</strong>,</p>

<p style='margin:0 0 18px 0;'>
Bazen bir hediye, binlerce kelimenin anlatamadığı o derin desteği ve
<strong>“yanındayım”</strong> mesajını en zarif haliyle hissettirir.
</p>

<p style='margin:0 0 18px 0;'>
<strong>{senderName}</strong>, kaybettiğiniz can dostunuz
<strong>{petName}</strong>’nın sevgisini her an kalbinizde hissetmeniz ve
hatırasını onurlandırmanız için size anlamlı bir
<strong>Styever anı sayfası</strong> hediye etti.
</p>

<p style='margin:0 0 18px 0;'>
<strong>Styever: Sevgiyi ve Anıları Yaşatan Dijital Bir Yuva</strong><br>
Styever; kaybettiğimiz dostlarımızın fotoğraflarını, videolarını ve en güzel
anılarını bir araya getirebileceğiniz size özel bir alandır. Burası, dostunuzun
hikayesini dilediğiniz her an ziyaret edebileceğiniz, sevdiklerinizle ve
dostlarınızla taziye mesajları paylaşarak hatırasını hep taze tutabileceğiniz
huzurlu bir köşedir. Onların hayatımızda bıraktığı izleri güvenle saklamanız
için tasarlanmış, sevgi dolu bir altyapıdır.
</p>

<p style='margin:0 0 20px 0;'>
<strong>Hemen Başlayın</strong><br>
Sizin için hazırlanan bu özel hediyeyi kabul etmek ve dostunuzun anı sayfasını
oluşturmaya başlamak için aşağıdaki bağlantıya tıklamanız yeterli:
</p>

<table width='100%' cellpadding='0' cellspacing='0' border='0'>
<tr>
<td align='center' style='padding:0 0 24px 0;'>
<a href='{link}'
   style='background:#1f4b3a;color:#ffffff;text-decoration:none;
          padding:13px 28px;border-radius:6px;font-size:16px;font-weight:bold;
          display:inline-block;'>
Anı Sayfasını Oluştur
</a>
</td>
</tr>
</table>

<p style='margin:0 0 18px 0;'>
<strong>Kupon Kodunuz:</strong> {userVoucher.Voucher}
</p>

<p style='margin:0 0 18px 0;'>
Her zaman yanınızdayız.
</p>

<p style='margin:0 0 18px 0;'>
Sevgi ve saygıyla,<br>
<strong>Styever Ekibi</strong>
</p>

<hr style='border:none;border-top:1px solid #d8d2c8;margin:20px 0;'>

<p style='text-align:center;font-size:12px;color:#555;margin:0;'>
*Bu e-posta {senderName} tarafından size ulaştırılmıştır.
</p>

</td>
</tr>

</table>

</td>
</tr>
</table>

</body>
</html>";

            builder.HtmlBody = html;

            builder.TextBody = $@"
Merhaba {receiverName},

Bazen bir hediye, binlerce kelimenin anlatamadığı o derin desteği ve ""yanındayım"" mesajını en zarif haliyle hissettirir.

{senderName}, kaybettiğiniz can dostunuz {petName}'nın sevgisini her an kalbinizde hissetmeniz ve hatırasını onurlandırmanız için size anlamlı bir Styever anı sayfası hediye etti.

Styever: Sevgiyi ve Anıları Yaşatan Dijital Bir Yuva

Styever; kaybettiğimiz dostlarımızın fotoğraflarını, videolarını ve en güzel anılarını bir araya getirebileceğiniz size özel bir alandır. Burası, dostunuzun hikayesini dilediğiniz her an ziyaret edebileceğiniz, sevdiklerinizle ve dostlarınızla taziye mesajları paylaşarak hatırasını hep taze tutabileceğiniz huzurlu bir köşedir. Onların hayatımızda bıraktığı izleri güvenle saklamanız için tasarlanmış, sevgi dolu bir altyapıdır.

Hemen Başlayın

Sizin için hazırlanan bu özel hediyeyi kabul etmek ve dostunuzun anı sayfasını oluşturmaya başlamak için aşağıdaki bağlantıya tıklamanız yeterli:

{link}

Kupon Kodunuz: {userVoucher.Voucher}

Her zaman yanınızdayız.

Sevgi ve saygıyla,
Styever Ekibi";

            var emailMessage = new MimeMessage();
            emailMessage.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            emailMessage.From.Add(MailboxAddress.Parse(_mailSettings.Mail));
            emailMessage.To.Add(MailboxAddress.Parse(userVoucher.ReceiverEmail));
            emailMessage.Subject = $"{senderName} size anlamlı bir Styever anı sayfası hediye etti";
            emailMessage.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.SslOnConnect);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(emailMessage);
            smtp.Disconnect(true);
        }
        public async Task<Result<UserVoucher>> VoucherControl(string voucher)
        {
            var result = new Result<UserVoucher>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var userVoucher = await _dbContext.UserVouchers.Where(x => !x.IsDeleted && x.Voucher == Guid.Parse(voucher)).FirstOrDefaultAsync();
                    result.SetData(userVoucher);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");

                }
                catch (Exception ex)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<UserVoucher>> BuyGiftPackage(UserVoucher userVoucher)
        {
            var result = new Result<UserVoucher>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    userVoucher.Date = DateTime.UtcNow;
                    userVoucher.Voucher = Guid.NewGuid();
                    userVoucher.IsDeleted = false;

                    _dbContext.Add(userVoucher);
                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();

                    result.SetData(userVoucher);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");

                    //await SendGiftMessage(userVoucher, userVoucher.UserId.HasValue ? _dbContext.Users.Where(x => x.Id == userVoucher.UserId).FirstOrDefault() : null, "http://localhost:4200/#/auth/registration");
                    await SendGiftMessage(userVoucher, userVoucher.UserId.HasValue ? _dbContext.Users.Where(x => x.Id == userVoucher.UserId).FirstOrDefault() : null, "https://styever.com/#/auth/registration");
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(exception.Message);
                }
            }

            return result;
        }

        public async Task<Result<bool>> Pay(long userId)
        {
            var result = new Result<bool>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var res = await ActivateUserMemories(userId);

                    if (res)
                    {
                        var user = await _dbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
                        user.Roles = await _dbContext.UserRoles.Where(x => x.UserId == userId).Select(s => s.RoleId).ToListAsync();

                        if (user.IsTrial)
                        {
                            user.IsTrial = false;
                            user.IsActive = true;
                        }

                        UserPayment payment = new UserPayment();
                        payment.UserId = userId;
                        payment.Price = user.Roles.First() == 2 ? 359.00 : user.Roles.First() == 3 ? 559.00 : 959.00;
                        payment.PlanId = user.Roles.First();
                        payment.PaymentDate = DateTime.UtcNow;
                        payment.IsDeleted = false;

                        _dbContext.Add(payment);
                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();

                        result.SetData(true);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("İşlem sırasında hata oluştu.");
                    }
                        
                }
                catch (Exception exception)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(exception.Message);
                }
            }

            return result;
        }

        public async Task<Result<List<User>>> GetUsers()
        {
            var result = new Result<List<User>>();
            
            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var data = await _dbContext.Users.Where(x => !x.IsDeleted).ToListAsync();

                    result.SetData(data);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<User>> Save(User user)
        {
            var result = new Result<User>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {

                    if(!_dbContext.Users.Where(x => x.Username == user.Username).Any())
                    {
                        var hashedPassword = HashPasword(user.Password, out var salt);

                        user.Password = hashedPassword;
                        user.Salt = salt;
                        user.CreatedDate = DateTime.UtcNow;
                        user.TrialExpirationDate = DateTime.UtcNow.AddDays(7);
                        user.IsTrial = true;

                        _dbContext.Users.Add(user);
                        await _dbContext.SaveChangesAsync();

                        foreach (var role in user.Roles)
                        {
                            UserRole ur = new UserRole();
                            ur.RoleId = role;
                            ur.UserId = user.Id;
                            ur.IsDeleted = false;

                            _dbContext.Add(ur);
                            await _dbContext.SaveChangesAsync();

                            var rolePerms = await _dbContext.RolePermissions.Include(x => x.Permission).Where(x => x.RoleId == role).Select(s => s.Permission).ToListAsync();
                            user.Permissions.AddRange(rolePerms);
                        }

                        foreach (var permission in user.Permissions)
                        {
                            UserPermission up = new UserPermission();
                            up.PermissionId = permission.Id;
                            up.UserId = user.Id;
                            up.IsDeleted = false;

                            _dbContext.Add(up);
                            await _dbContext.SaveChangesAsync();
                        }

                        UserAddress ua = new UserAddress();
                        ua.Address = user.UserAddress.Address;
                        ua.AddressHeader = user.UserAddress.AddressHeader;
                        ua.UserId = user.Id;
                        ua.IsDeleted = false;
                        ua.City = user.UserAddress.City;
                        ua.Country = user.UserAddress.Country;
                        ua.District = user.UserAddress.District;
                        ua.IsPrimary = true;

                        _dbContext.Add(ua);
                        await _dbContext.SaveChangesAsync();

                        transaction.Commit();

                        result.SetData(user);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Aynı kullanıcı ismine sahip başka bir kullanıcı bulunmaktadır.");
                    }
                    
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<User>> Update(User user)
        {
            var result = new Result<User>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var oldUser = await _dbContext.Users.Where(x => x.Id == user.Id).FirstOrDefaultAsync();
                    if(oldUser != null) 
                    { 
                        oldUser.Surname = user.Surname;
                        oldUser.Name = user.Name;
                        oldUser.Email = user.Email;
                        oldUser.Phone = user.Phone;
                        oldUser.Username = user.Username;
                        oldUser.FileId = user.FileId;

                        var roles = await _dbContext.UserRoles.Where(x => x.UserId == user.Id).ToListAsync();
                        _dbContext.UserRoles.RemoveRange(roles);

                        var permissions = await _dbContext.UserPermissions.Where(x => x.UserId == user.Id).ToListAsync();
                        _dbContext.UserPermissions.RemoveRange(permissions);

                        await _dbContext.SaveChangesAsync();

                        foreach (var role in user.Roles)
                        {
                            UserRole ur = new UserRole();
                            ur.RoleId = role;
                            ur.UserId = user.Id;
                            ur.IsDeleted = false;

                            _dbContext.Add(ur);
                            await _dbContext.SaveChangesAsync();

                            var rolePerms = await _dbContext.RolePermissions.Include(x => x.Permission).Where(x => x.RoleId == role).Select(s => s.Permission).ToListAsync();
                            user.Permissions.AddRange(rolePerms);
                        }

                        foreach (var permission in user.Permissions)
                        {
                            UserPermission up = new UserPermission();
                            up.PermissionId = permission.Id;
                            up.UserId = user.Id;
                            up.IsDeleted = false;

                            _dbContext.Add(up);
                            await _dbContext.SaveChangesAsync();
                        }

                        transaction.Commit();

                        result.SetData(user);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Böyle bir kayıt bulunmamaktadır.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<User>> Delete(long id)
        {
            var result = new Result<User>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var oldUser = await _dbContext.Users.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
                    if (oldUser != null)
                    {
                        oldUser.IsDeleted = true;

                        var roles = await _dbContext.UserRoles.Where(x => x.UserId == oldUser.Id).ToListAsync();
                        _dbContext.UserRoles.RemoveRange(roles);

                        var permissions = await _dbContext.UserPermissions.Where(x => x.UserId == oldUser.Id).ToListAsync();
                        _dbContext.UserPermissions.RemoveRange(permissions);

                        var addresses = await _dbContext.UserAddresses.Where(x => x.UserId == oldUser.Id).ToListAsync();
                        _dbContext.UserAddresses.RemoveRange(addresses);

                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();

                        result.SetData(oldUser);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Böyle bir kayıt bulunmamaktadır.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<User>> GetById(long id, string token)
        {
            var result = new Result<User>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var user = await _dbContext.Users.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
                    if (user != null)
                    {
                        user.Password = null;
                        user.Salt = null;
                        user.Roles = await _dbContext.UserRoles.Where(x => x.UserId == id && !x.IsDeleted).Select(s => s.RoleId).ToListAsync();

                        if (user.FileId.HasValue)
                        {
                            user.File = await GetFile(user.FileId.Value, token);
                        }

                        result.SetData(user);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Böyle bir kayıt bulunmamaktadır.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<UserAddress>> GetUserAddressById(long id)
        {
            var result = new Result<UserAddress>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var userAddress = await _dbContext.UserAddresses.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
                    if (userAddress != null)
                    {
                        result.SetData(userAddress);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Böyle bir kayıt bulunmamaktadır.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }


        public async Task<Result<UserAddress>> UserAddressSave(UserAddress userAddress)
        {
            var result = new Result<UserAddress>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    if (!_dbContext.UserAddresses.Where(x => (x.AddressHeader == userAddress.AddressHeader) && !x.IsDeleted).Any())
                    {

                        _dbContext.Add(userAddress);
                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();

                        result.SetData(userAddress);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Aynı isim veya kodla tanımlı bir yetki bulunmaktadır.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<UserAddress>> UserAddressUpdate(UserAddress userAddress)
        {
            var result = new Result<UserAddress>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var oldAddress = await _dbContext.UserAddresses.Where(x => x.Id == userAddress.Id && !x.IsDeleted).FirstOrDefaultAsync();

                    if (oldAddress != null)
                    {
                        if (!_dbContext.UserAddresses.Where(x => x.Id != oldAddress.Id && (x.AddressHeader == userAddress.AddressHeader) && !x.IsDeleted).Any())
                        {
                            oldAddress.Country = userAddress.Country;
                            oldAddress.City = userAddress.City;
                            oldAddress.District = userAddress.District;
                            oldAddress.Address = userAddress.Address;
                            oldAddress.AddressHeader = userAddress.AddressHeader;
                            oldAddress.IsPrimary = userAddress.IsPrimary;

                            await _dbContext.SaveChangesAsync();
                            transaction.Commit();

                            result.SetData(userAddress);
                            result.SetMessage("İşlem başarı ile gerçekleşti.");
                        }
                        else
                        {
                            result.SetIsSuccess(false);
                            result.SetMessage("Aynı başlıkla tanımlı bir adres bulunmaktadır.");
                        }
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Böyle bir kayıt bulunmamaktadır.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<List<UserAddress>>> GetUserAddresses(long userId)
        {
            var result = new Result<List<UserAddress>>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var data = await _dbContext.UserAddresses.Where(x => x.UserId == userId && !x.IsDeleted).ToListAsync();

                    result.SetData(data);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<UserAddress>> UserAddressDelete(long id)
        {
            var result = new Result<UserAddress>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var oldAddress = await _dbContext.UserAddresses.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
                    if (oldAddress != null)
                    {
                        oldAddress.IsDeleted = true;

                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();

                        result.SetData(oldAddress);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Böyle bir kayıt bulunmamaktadır.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<List<string>>> GetUserPermissions(string token)
        {
            var result = new Result<List<string>>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var principal = await GetPrincipalFromToken(token);
                    var userId = principal.Claims.Where(x => x.Type == "id").Select(s => s.Value).FirstOrDefault();

                    var roleIds = await _dbContext.UserRoles.Include(x => x.Role).Where(x => !x.Role.IsDeleted && x.UserId == Convert.ToInt64(userId)).Select(s => s.RoleId).ToListAsync();

                    var permissions = await _dbContext.RolePermissions.Include(x => x.Permission).Where(x => !x.Permission.IsDeleted && roleIds.Contains(x.RoleId))
                                                            .Select(s => s.Permission.Code).ToListAsync();

                    result.SetData(permissions);
                    result.SetMessage("Islem basari ile gerceklesti.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<User>> UserAvatarUpdate(long id, long fileId)
        {
            var result = new Result<User>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var oldUser = await _dbContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
                    if (oldUser != null)
                    {
                        oldUser.FileId = fileId;
                     
                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();

                        result.SetData(oldUser);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Böyle bir kayıt bulunmamaktadır.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }


        public async Task<Result<UserAddress>> GetPrimaryUserAddressById(long userId)
        {
            var result = new Result<UserAddress>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var userAddress = await _dbContext.UserAddresses.Where(x => x.UserId == userId && !x.IsDeleted && x.IsPrimary).FirstOrDefaultAsync();
                    if (userAddress != null)
                    {
                        result.SetData(userAddress);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Böyle bir kayıt bulunmamaktadır.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }


        private Task<ClaimsPrincipal?> GetPrincipalFromToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:Secret"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return Task.FromResult(principal);

        }

        private string HashPasword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(keySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);
            return Convert.ToHexString(hash);
        }

        private async Task<Model.File> GetFile(long id, string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(configuration["AppSettings:ApiUrl"] + "/api/File/" + id);

            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseStr))
                {
                    try
                    {
                        Result<Model.File> result = JsonConvert.DeserializeObject<Result<Model.File>>(responseStr);

                        if (result != null)
                        {
                            return result.GetData();
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }

                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            return null;
        }

        private async Task<bool> ActivateUserMemories(long id)
        {
            HttpClient client = new HttpClient();

            var response = await client.GetAsync(configuration["AppSettings:ApiUrl"] + "/api/Memory/ActivateUserMemories/" + id);

            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseStr))
                {
                    try
                    {
                        Result<bool> result = JsonConvert.DeserializeObject<Result<bool>>(responseStr);

                        if (result != null)
                        {
                            return result.GetData();
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return false;
        }

        private async Task<bool> SetBelongingIssuesToTrueUserMemory(long id, long memoryId)
        {
            HttpClient client = new HttpClient();

            var response = await client.GetAsync(configuration["AppSettings:ApiUrl"] + "/api/Memory/SetBelongingIssuesToTrueUserMemory/" + id + "/" + memoryId);

            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseStr))
                {
                    try
                    {
                        Result<bool> result = JsonConvert.DeserializeObject<Result<bool>>(responseStr);

                        if (result != null)
                        {
                            return result.GetData();
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return false;
        }


        private async Task<bool> SetBelongingIssuesToFalseUserMemory(long id)
        {
            HttpClient client = new HttpClient();

            var response = await client.GetAsync(configuration["AppSettings:ApiUrl"] + "/api/Memory/SetBelongingIssuesToFalseUserMemory/" + id);

            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseStr))
                {
                    try
                    {
                        Result<bool> result = JsonConvert.DeserializeObject<Result<bool>>(responseStr);

                        if (result != null)
                        {
                            return result.GetData();
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return false;
        }

    }
}
