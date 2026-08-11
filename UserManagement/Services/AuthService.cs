using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using UserManagement.DbContexts;
using UserManagement.Entity;
using UserManagement.Interfaces;
using UserManagement.Model;

namespace UserManagement.Services
{
    public class AuthService : IAuthService
    {
        const int keySize = 64;
        const int iterations = 350000;
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        private readonly UserManagementContext _dbContext;
        private readonly ITokenService tokenService;
        private readonly MailSettings _mailSettings;

        public AuthService(UserManagementContext dbContext, ITokenService tokenService, MailSettings mailSettings)
        {
            _dbContext = dbContext;
            this.tokenService = tokenService;
            _mailSettings = mailSettings;
        }

        public async Task<Result<UserLoginResponse>> Login(UserLoginRequest request)
        {
            var result = new Result<UserLoginResponse>();

                try
                {
                    var user = await _dbContext.Users.Where(x => (request.Username == x.Username || request.Username == x.Email) && !x.IsDeleted)
                        .Select(s => new User()
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Surname = s.Surname,
                            Username = s.Username,
                            Password = s.Password,
                            Salt = s.Salt,
                            Email = s.Email,
                            Phone = s.Phone,
                            ExpirationDate = s.ExpirationDate,
                            CreatedDate = s.CreatedDate,
                            TrialExpirationDate = s.TrialExpirationDate,
                            IsTrial = s.IsTrial,
                            Permissions = _dbContext.UserPermissions.Include(p => p.Permission).Where(x => !x.IsDeleted && x.UserId == s.Id).Select(p => p.Permission).ToList(),
                            Roles = _dbContext.UserRoles.Where(x => !x.IsDeleted && x.UserId == s.Id).Select(p => p.RoleId).ToList(),
                            IsActive = s.IsActive,
                        }).FirstOrDefaultAsync();

                    if(user != null)
                    {
                        var isSuccess = VerifyPassword(request.Password, user.Password, user.Salt);
                        if (isSuccess)
                        {
                            var generatedTokenInformation = await tokenService.GenerateToken(new GenerateTokenRequest { User = user });
                            await UpdateRefreshToken(user, generatedTokenInformation.RefreshToken, generatedTokenInformation.RefreshTokenExpireDate);

                            UserLoginResponse response = new UserLoginResponse();
                            response.AuthenticateResult = true;
                            response.AccessToken = generatedTokenInformation.Token;
                            response.RefreshToken = generatedTokenInformation.RefreshToken;

                            if (user.IsTrial && !user.IsActive)
                            {
                                response.IsPaymentRequired = true;
                            }
                            else
                            {
                                response.IsPaymentRequired = false;

                            }

                        result.SetData(response);
                            result.SetMessage("İşlem başarı ile gerçekleşti.");
                        }
                        else
                        {
                            result.SetIsSuccess(false);
                            result.SetMessage("Şifre hatalı.");
                        }
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Kullanıcı adı/e-posta hatalı.");
                    }

                }
                catch (Exception ex)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }

            return result;
        }

        public async Task<Result<UserLoginResponse>> RefreshToken(RefreshTokenRequest request)
        {
            var result = new Result<UserLoginResponse>();

            var principal = await tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
            {
                result.SetIsSuccess(false);
                result.SetMessage("Invalid access token or refresh token");
            }
            else
            {
                try
                {
                    var userId = principal.Claims.Where(x => x.Type == "id").Select(s => s.Value).FirstOrDefault();

                    var applicationUser = await _dbContext.ApplicationUsers.Include(i => i.User).Where(x => x.UserId == Convert.ToInt64(userId))
                    .FirstOrDefaultAsync();

                    if (applicationUser == null || applicationUser.RefreshToken != request.RefreshToken || applicationUser.RefreshTokenExpireDate <= DateTime.UtcNow)
                    {

                        result.SetIsSuccess(false);
                        result.SetMessage("Invalid access token or refresh token");
                    }
                    else
                    {
                        var user = await _dbContext.Users.Where(x => x.Id == applicationUser.User.Id && !x.IsDeleted)
                       .Select(s => new User()
                       {
                           Id = s.Id,
                           Name = s.Name,
                           Surname = s.Surname,
                           Username = s.Username,
                           Password = s.Password,
                           Salt = s.Salt,
                           Email = s.Email,
                           Phone = s.Phone,
                           ExpirationDate = s.ExpirationDate,
                           CreatedDate = s.CreatedDate,
                           TrialExpirationDate = s.TrialExpirationDate,
                           IsTrial = s.IsTrial,
                           Permissions = _dbContext.UserPermissions.Include(p => p.Permission).Where(x => !x.IsDeleted && x.UserId == s.Id).Select(p => p.Permission).ToList(),
                           Roles = _dbContext.UserRoles.Where(x => !x.IsDeleted && x.UserId == s.Id).Select(p => p.RoleId).ToList(),
                           IsActive = s.IsActive,
                       }).FirstOrDefaultAsync();

                        var generatedTokenInformation = await tokenService.GenerateToken(new GenerateTokenRequest { User = user });
                        await UpdateRefreshToken(applicationUser.User, generatedTokenInformation.RefreshToken, generatedTokenInformation.RefreshTokenExpireDate);

                        UserLoginResponse response = new UserLoginResponse();
                        response.AuthenticateResult = true;
                        response.AccessToken = generatedTokenInformation.Token;
                        //response.AccessTokenExpireDate = generatedTokenInformation.TokenExpireDate;
                        response.RefreshToken = generatedTokenInformation.RefreshToken;
                        //response.RefreshTokenExpireDate = generatedTokenInformation.RefreshTokenExpireDate;

                        result.SetData(response);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                }
                catch (Exception ex)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }


        private async Task UpdateRefreshToken(User user, string refreshToken, DateTime refreshTokenExpireDate)
        {
            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var applicationUser = await _dbContext.ApplicationUsers.Where(x => x.UserId == user.Id).FirstOrDefaultAsync();

                    if (applicationUser == null)
                    {
                        applicationUser = new ApplicationUser();
                        applicationUser.UserId = user.Id;
                        applicationUser.RefreshToken = refreshToken;
                        applicationUser.RefreshTokenExpireDate = refreshTokenExpireDate;
                        _dbContext.Add(applicationUser);
                    }
                    else
                    {
                        applicationUser.RefreshToken = refreshToken;
                        applicationUser.RefreshTokenExpireDate = refreshTokenExpireDate;
                    }

                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }

        public async Task<Result<string>> ForgotPassword(ForgotPasswordRequest request)
        {
            var result = new Result<string>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var user = await _dbContext.Users.Where(x => x.Email == request.Email).FirstOrDefaultAsync();

                    if(user != null)
                    {
                        //await SendResetPasswordLink(user.Email, "http://localhost:4200/#/auth/reset-password/" + user.Id);
                        await SendResetPasswordLink(user.Email, "https://styever.com/#/auth/reset-password/" + user.Id);

                        result.SetData(request.Email);
                        result.SetIsSuccess(true);
                        result.SetMessage("Sıfırlama linki mail adresine gönderildi.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Kullanıcı adı/e-posta hatalı.");
                    }
                }
                catch (Exception ex)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<bool>> ResetPassword(ResetPasswordRequest request)
        {
            var result = new Result<bool>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var user = await _dbContext.Users.Where(x => x.Id == request.Id).FirstOrDefaultAsync();

                    if (user != null)
                    {
                        var hashedPassword = HashPasword(request.Password, out var salt);

                        user.Password = hashedPassword;
                        user.Salt = salt;

                        await _dbContext.SaveChangesAsync();

                        transaction.Commit();
                        result.SetData(true);
                        result.SetIsSuccess(true);
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Böyle bir kullanıcı bulunamadı.");
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

        public async Task<Result<bool>> ChangePassword(ChangePasswordRequest request)
        {
            var result = new Result<bool>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var user = await _dbContext.Users.Where(x => x.Id == request.Id).FirstOrDefaultAsync();

                    if (user != null)
                    {
                        var isSuccess = VerifyPassword(request.CurrentPassword, user.Password, user.Salt);
                        if (isSuccess)
                        {

                            var hashedPassword = HashPasword(request.Password, out var salt);

                            user.Password = hashedPassword;
                            user.Salt = salt;

                            await _dbContext.SaveChangesAsync();

                            transaction.Commit();
                            result.SetData(true);
                            result.SetIsSuccess(true);
                        }
                        else
                        {
                            result.SetIsSuccess(false);
                            result.SetMessage("Current password is incorrect.");
                        }
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Böyle bir kullanıcı bulunamadı.");
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

        public async Task<Result<User>> RegisterWithVoucher(User user)
        {
            var result = new Result<User>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var userVoucher = await _dbContext.UserVouchers.Where(x => !x.IsDeleted && x.Voucher == user.Voucher).FirstOrDefaultAsync();
                    if (userVoucher != null)
                    {
                        if (!_dbContext.Users.Where(x => x.Username == user.Username && !x.IsDeleted).Any())
                        {
                            var hashedPassword = HashPasword(user.Password, out var salt);

                            user.Password = hashedPassword;
                            user.Salt = salt;
                            user.CreatedDate = DateTime.UtcNow;
                            user.ExpirationDate = user.CreatedDate.AddYears(1);
                            user.TrialExpirationDate = user.CreatedDate.AddDays(7);
                            user.IsTrial = false;
                            user.IsDeleted = false;
                            user.IsActive = true;

                            _dbContext.Users.Add(user);
                            await _dbContext.SaveChangesAsync();

                            UserRole ur = new UserRole();
                            ur.RoleId = user.Roles.First();
                            ur.UserId = user.Id;
                            ur.IsDeleted = false;

                            _dbContext.Add(ur);
                            await _dbContext.SaveChangesAsync();

                            var rolePermissions = await _dbContext.RolePermissions.Where(x => x.RoleId == user.Roles.First() && !x.IsDeleted).Select(s => s.PermissionId).ToListAsync();
                            foreach (var permission in rolePermissions)
                            {
                                UserPermission up = new UserPermission();
                                up.PermissionId = permission;
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

                            userVoucher.IsDeleted = true;
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
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Böyle bir kupon kodu bulunmamaktadır.");
                    }

                    return result;

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
        public async Task<Result<User>> Register(User user)
        {
            var result = new Result<User>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    if (!_dbContext.Users.Where(x => x.Username == user.Username && !x.IsDeleted).Any())
                    {
                        var hashedPassword = HashPasword(user.Password, out var salt);

                        user.Password = hashedPassword;
                        user.Salt = salt;
                        user.CreatedDate = DateTime.UtcNow;
                        user.ExpirationDate = user.CreatedDate.AddYears(1);
                        user.TrialExpirationDate = user.CreatedDate.AddDays(7);
                        user.IsTrial = true;
                        user.IsDeleted = false;
                        user.IsActive = true;

                        _dbContext.Users.Add(user);
                        await _dbContext.SaveChangesAsync();

                        UserRole ur = new UserRole();
                        ur.RoleId = user.Roles.First();
                        ur.UserId = user.Id;
                        ur.IsDeleted = false;
                        
                        _dbContext.Add(ur);
                        await _dbContext.SaveChangesAsync();

                        var rolePermissions = await _dbContext.RolePermissions.Where(x => x.RoleId == user.Roles.First() && !x.IsDeleted).Select(s => s.PermissionId).ToListAsync();
                        foreach (var permission in rolePermissions)
                        {
                            UserPermission up = new UserPermission();
                            up.PermissionId = permission;
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

        private async Task SendResetPasswordLink(string email, string link)
        {
            var emailMessage = new MimeMessage();
            emailMessage.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            emailMessage.From.Add(MailboxAddress.Parse(_mailSettings.Mail));
            emailMessage.To.Add(MailboxAddress.Parse(email));
            emailMessage.Subject = "Şifre Sıfırlama Talebi";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = "Merhaba,\nHesabınız için bir şifre sıfırlama talebi aldık.\n Yeni şifrenizi belirlemek için lütfen aşağıdaki bağlantıya tıklayınız.\n" + link + "\nEğer bu talebi siz yapmadıysanız, bu e-postayı dikkate almayabilirsiniz.\n\nSevgiler,\nStyever Ekibi"};

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.SslOnConnect);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(emailMessage);
            smtp.Disconnect(true);
        }

        private bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
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

    }
}
