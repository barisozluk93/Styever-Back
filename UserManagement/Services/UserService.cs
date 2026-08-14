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
                    var queryable = _dbContext.Users.AsQueryable();

                    if (!string.IsNullOrEmpty(lowerFilterText))
                        queryable = queryable.Where(x => x.Name.ToLower().Contains(lowerFilterText) || x.Surname.ToLower().Contains(lowerFilterText) || x.Username.ToLower().Contains(lowerFilterText) || x.Email.ToLower().Contains(lowerFilterText));
                    if (!string.IsNullOrWhiteSpace(pagingParameter.Name)) queryable = queryable.Where(x => x.Name.ToLower().Contains(pagingParameter.Name.ToLower()));
                    if (!string.IsNullOrWhiteSpace(pagingParameter.NameSurname))
                    {
                        var nameSurname = pagingParameter.NameSurname.ToLower();
                        queryable = queryable.Where(x => (x.Name + " " + x.Surname).ToLower().Contains(nameSurname));
                    }
                    if (!string.IsNullOrWhiteSpace(pagingParameter.Surname)) queryable = queryable.Where(x => x.Surname.ToLower().Contains(pagingParameter.Surname.ToLower()));
                    if (!string.IsNullOrWhiteSpace(pagingParameter.Username)) queryable = queryable.Where(x => x.Username.ToLower().Contains(pagingParameter.Username.ToLower()));
                    if (!string.IsNullOrWhiteSpace(pagingParameter.Email)) queryable = queryable.Where(x => x.Email.ToLower().Contains(pagingParameter.Email.ToLower()));
                    if (!string.IsNullOrWhiteSpace(pagingParameter.Phone)) queryable = queryable.Where(x => x.Phone.ToLower().Contains(pagingParameter.Phone.ToLower()));
                    if (pagingParameter.IsDeleted.HasValue) queryable = queryable.Where(x => x.IsDeleted == pagingParameter.IsDeleted.Value);
                    if (pagingParameter.IsActive.HasValue) queryable = queryable.Where(x => x.IsActive == pagingParameter.IsActive.Value);
                    if (pagingParameter.CreatedDateFrom.HasValue) queryable = queryable.Where(x => x.CreatedDate >= pagingParameter.CreatedDateFrom.Value);
                    if (pagingParameter.CreatedDateTo.HasValue) queryable = queryable.Where(x => x.CreatedDate < pagingParameter.CreatedDateTo.Value.Date.AddDays(1));
                    if (pagingParameter.RoleId.HasValue) queryable = queryable.Where(x => _dbContext.UserRoles.Any(r => !r.IsDeleted && r.UserId == x.Id && r.RoleId == pagingParameter.RoleId.Value));

                    queryable = queryable.Select(s => new User()
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Surname = s.Surname,
                        Username = s.Username,
                        IsDeleted = s.IsDeleted,
                        Email = s.Email,
                        Phone = s.Phone,
                        IsSystemData = s.IsSystemData,
                        IsActive = s.IsActive,
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
                        var selectedPlan = await _dbContext.Plans.AsNoTracking().FirstOrDefaultAsync(x => x.Id == planId && !x.IsDeleted);
                        if (selectedPlan == null) throw new InvalidOperationException("Seçilen paket bulunamadı.");
                        payment.Price = selectedPlan.Price;
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
                : (!string.IsNullOrWhiteSpace(userVoucher.SenderFullName)
                    ? userVoucher.SenderFullName
                    : userVoucher.SenderEmail);

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
        public async Task SendMailAsync(
            string to,
            string subject,
            string htmlBody,
            string textBody,
            IEnumerable<MailAttachment>? attachments = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Alıcı e-posta adresi zorunludur.", nameof(to));

            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = textBody
            };

            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    if (attachment.Content == null || attachment.Content.Length == 0)
                        continue;

                    var mimeType = string.IsNullOrWhiteSpace(attachment.ContentType)
                        ? "application/octet-stream"
                        : attachment.ContentType;

                    var parts = mimeType.Split('/', 2);
                    var contentType = parts.Length == 2
                        ? new ContentType(parts[0], parts[1])
                        : new ContentType("application", "octet-stream");

                    builder.Attachments.Add(
                        attachment.FileName,
                        attachment.Content,
                        contentType);
                }
            }

            var emailMessage = new MimeMessage();
            emailMessage.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            emailMessage.From.Add(MailboxAddress.Parse(_mailSettings.Mail));
            emailMessage.To.Add(MailboxAddress.Parse(to));
            emailMessage.Subject = subject;
            emailMessage.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _mailSettings.Host,
                _mailSettings.Port,
                SecureSocketOptions.SslOnConnect,
                cancellationToken);

            await smtp.AuthenticateAsync(
                _mailSettings.Mail,
                _mailSettings.Password,
                cancellationToken);

            await smtp.SendAsync(emailMessage, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);
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
                    var selectedPlan = await _dbContext.Plans
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == userVoucher.PlanId && !x.IsDeleted);

                    if (selectedPlan == null)
                        throw new InvalidOperationException("Seçilen paket bulunamadı.");

                    // Fiyat istemciden güvenilmez; her zaman Plan tablosundaki güncel fiyat kullanılır.
                    userVoucher.Price = selectedPlan.Price;
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
                        var selectedPlan = await _dbContext.Plans.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Roles.First() && !x.IsDeleted);
                        if (selectedPlan == null) throw new InvalidOperationException("Kullanıcının paketi bulunamadı.");
                        payment.Price = selectedPlan.Price;
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

        public async Task<Result<DashboardUserStats>> GetDashboardStats(DateTime? startDate, DateTime? endDate)
        {
            var result = new Result<DashboardUserStats>();
            try
            {
                TimeZoneInfo istanbulTimeZone;
                try
                {
                    istanbulTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");
                }
                catch (TimeZoneNotFoundException)
                {
                    istanbulTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
                }

                DateTime ToUtcBoundary(DateTime value)
                {
                    var localDate = DateTime.SpecifyKind(value.Date, DateTimeKind.Unspecified);
                    return TimeZoneInfo.ConvertTimeToUtc(localDate, istanbulTimeZone);
                }

                DateTime ToIstanbulTime(DateTime value)
                {
                    var utc = value.Kind switch
                    {
                        DateTimeKind.Utc => value,
                        DateTimeKind.Local => value.ToUniversalTime(),
                        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
                    };
                    return DateTime.SpecifyKind(
                        TimeZoneInfo.ConvertTimeFromUtc(utc, istanbulTimeZone),
                        DateTimeKind.Unspecified);
                }

                var endInput = endDate ?? TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, istanbulTimeZone);
                var startInput = startDate ?? endInput.AddDays(-6);

                // Dashboard tarihleri Türkiye takvim günü olarak değerlendirilir.
                // Örn. 13.08 00:00 +03, UTC'de 12.08 21:00 olsa bile 13 Ağustos gelirine yazılır.
                var startUtc = ToUtcBoundary(startInput);
                var endExclusiveUtc = ToUtcBoundary(endInput.Date.AddDays(1));
                var startCalendarDate = startInput.Date;
                var endCalendarDate = endInput.Date;

                // Üst özet ve paket kartları global metriklerdir; tarih filtresinden etkilenmez.
                // Dönem filtresi yalnızca trend, gelir analizi ve son hareketler için kullanılır.
                var allUsers = _dbContext.Users.AsNoTracking()
                    .Where(x => !x.IsDeleted && x.Id > 4);

                var users = allUsers
                    .Where(x => x.CreatedDate >= startUtc && x.CreatedDate < endExclusiveUtc);

                var allRoleQuery =
                    from userRole in _dbContext.UserRoles.AsNoTracking()
                    join user in allUsers on userRole.UserId equals user.Id
                    where !userRole.IsDeleted
                    select userRole;

                var allPayments = _dbContext.UserPayments.AsNoTracking()
                    .Where(x => !x.IsDeleted);
                // UserVoucher.IsDeleted voucher kullanıldığında true yapılıyor. Bu bir satış iptali değildir;
                // sadece voucher'ın tekrar kullanılmasını engeller. Dashboard gelir/hediye metrikleri
                // satın alınmış gift kayıtlarını kullanım durumundan bağımsız olarak korumalıdır.
                var allGifts = _dbContext.UserVouchers.AsNoTracking();

                var payments = allPayments
                    .Where(x => x.PaymentDate >= startUtc && x.PaymentDate < endExclusiveUtc);
                var gifts = allGifts
                    .Where(x => x.Date >= startUtc && x.Date < endExclusiveUtc);

                var totalUsers = await allUsers.CountAsync();

                // Kayıt kaynağı dağılımı globaldir. RegisterWithVoucher akışında
                // UserVoucher.IsDeleted=true olması voucher'ın kullanıldığını ve bu voucher ile
                // sisteme bir kayıt yapıldığını gösterir. ReceiverEmail ile kullanıcı mailinin
                // aynı olması gerekmez; bu nedenle doğrudan kullanılmış voucher sayısını baz alıyoruz.
                var usedVoucherCount = await _dbContext.UserVouchers
                    .AsNoTracking()
                    .LongCountAsync(x => x.IsDeleted);

                // Veri tutarsızlığı durumunda dağılımın toplam kullanıcı sayısını aşmaması için sınırla.
                var giftVoucherUsers = Math.Min(totalUsers, usedVoucherCount);
                var regularUsers = Math.Max(0, totalUsers - giftVoucherUsers);

                var stats = new DashboardUserStats
                {
                    TotalUsers = totalUsers,
                    ActiveMembers = await allUsers.CountAsync(x => x.IsActive),
                    OriginUsers = await allRoleQuery.CountAsync(x => x.RoleId == 2),
                    HeartUsers = await allRoleQuery.CountAsync(x => x.RoleId == 3),
                    FamilyUsers = await allRoleQuery.CountAsync(x => x.RoleId == 4),
                    // Bu alanlar üstteki global özet kartlarında kullanılır.
                    // Dönemsel gelir kırılımı frontend'de filtrelenmiş Trend toplamlarından hesaplanır.
                    MembershipRevenue = await allPayments.SumAsync(x => (double?)x.Price) ?? 0,
                    GiftRevenue = await allGifts.SumAsync(x => (double?)x.Price) ?? 0,
                    TotalGifts = await allGifts.CountAsync(),
                    GiftVoucherUsers = giftVoucherUsers,
                    RegularUsers = regularUsers,
                    ExpiredTrialUsers = await allUsers.LongCountAsync(x => x.IsTrial && !x.IsActive && x.TrialExpirationDate <= DateTime.UtcNow),
                    ExpiredPackageUsers = await allUsers.LongCountAsync(x => x.ExpirationDate <= DateTime.UtcNow && !x.IsTrial)
                };

                // PostgreSQL timestamptz UTC instant saklar. Günlük gruplama DB'deki UTC .Date ile
                // değil, Türkiye saatine çevrildikten sonra yapılır; aksi halde 00:00 +03 kayıtları
                // bir önceki güne düşer.
                var userDates = await users
                    .Where(x => x.CreatedDate >= startUtc && x.CreatedDate < endExclusiveUtc)
                    .Select(x => x.CreatedDate)
                    .ToListAsync();

                var paymentRows = await payments
                    .Select(x => new { x.PaymentDate, x.Price })
                    .ToListAsync();

                var giftRows = await gifts
                    .Select(x => new { x.Date, x.Price })
                    .ToListAsync();

                var userDaily = userDates
                    .GroupBy(x => ToIstanbulTime(x).Date)
                    .ToDictionary(g => g.Key, g => (long)g.Count());

                var paymentDaily = paymentRows
                    .GroupBy(x => ToIstanbulTime(x.PaymentDate).Date)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Price));

                var giftDaily = giftRows
                    .GroupBy(x => ToIstanbulTime(x.Date).Date)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Price));

                for (var day = startCalendarDate; day <= endCalendarDate; day = day.AddDays(1))
                {
                    stats.Trend.Add(new DashboardTrendPoint
                    {
                        Date = DateTime.SpecifyKind(day, DateTimeKind.Unspecified),
                        NewUsers = userDaily.TryGetValue(day, out var userCount) ? userCount : 0,
                        MembershipRevenue = paymentDaily.TryGetValue(day, out var membershipRevenue) ? membershipRevenue : 0,
                        GiftRevenue = giftDaily.TryGetValue(day, out var giftRevenue) ? giftRevenue : 0
                    });
                }

                var recentUsers = (await users
                    .Where(x => x.CreatedDate >= startUtc && x.CreatedDate < endExclusiveUtc)
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(8)
                    .Select(x => new
                    {
                        x.Name,
                        x.Surname,
                        x.CreatedDate
                    })
                    .ToListAsync())
                    .Select(x => new DashboardRecentActivity
                    {
                        Type = "user",
                        Name = (x.Name + " " + x.Surname).Trim(),
                        ActorName = (x.Name + " " + x.Surname).Trim(),
                        Date = ToIstanbulTime(x.CreatedDate)
                    })
                    .ToList();

                var recentPayments = (await payments
                    .Include(x => x.User)
                    .OrderByDescending(x => x.PaymentDate)
                    .Take(8)
                    .ToListAsync())
                    .Select(x => new DashboardRecentActivity
                    {
                        Type = "payment",
                        Name = x.User == null ? null : (x.User.Name + " " + x.User.Surname).Trim(),
                        ActorName = x.User == null ? null : (x.User.Name + " " + x.User.Surname).Trim(),
                        Amount = x.Price,
                        Date = ToIstanbulTime(x.PaymentDate)
                    })
                    .ToList();

                var recentGiftRows = await gifts
                    .Include(x => x.User)
                    .OrderByDescending(x => x.Date)
                    .Take(8)
                    .ToListAsync();

                var senderEmails = recentGiftRows
                    .Where(x => x.User == null && !string.IsNullOrWhiteSpace(x.SenderEmail))
                    .Select(x => x.SenderEmail!)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var senderUsers = senderEmails.Count == 0
                    ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    : (await _dbContext.Users.AsNoTracking()
                        .Where(x => senderEmails.Contains(x.Email))
                        .Select(x => new { x.Email, x.Name, x.Surname })
                        .ToListAsync())
                        .GroupBy(x => x.Email, StringComparer.OrdinalIgnoreCase)
                        .ToDictionary(
                            g => g.Key,
                            g => (g.First().Name + " " + g.First().Surname).Trim(),
                            StringComparer.OrdinalIgnoreCase);

                var recentGifts = recentGiftRows
                    .Select(x =>
                    {
                        string? actorName = null;
                        if (x.User != null)
                        {
                            actorName = (x.User.Name + " " + x.User.Surname).Trim();
                        }
                        else if (!string.IsNullOrWhiteSpace(x.SenderEmail) && senderUsers.TryGetValue(x.SenderEmail, out var senderName))
                        {
                            actorName = senderName;
                        }

                        return new DashboardRecentActivity
                        {
                            Type = "gift",
                            // Alıcı maili Recent Activities'te gösterilmez.
                            Name = null,
                            ActorName = actorName,
                            Amount = x.Price,
                            Date = ToIstanbulTime(x.Date)
                        };
                    })
                    .ToList();

                stats.RecentActivities = recentUsers
                    .Concat(recentPayments)
                    .Concat(recentGifts)
                    .OrderByDescending(x => x.Date)
                    .Take(8)
                    .ToList();

                result.SetData(stats);
                result.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex)
            {
                result.SetIsSuccess(false);
                result.SetMessage(ex.Message);
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
                        oldUser.IsActive = false;

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
