using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;
using UserManagement.DbContexts;
using UserManagement.Entity;
using UserManagement.Interfaces;
using UserManagement.Model;
using UserManagement.Utils;

namespace UserManagement.Services
{
    public class ShopierPaymentService : IShopierPaymentService
    {
        private readonly UserManagementContext _db;
        private readonly IUserService _userService;
        private readonly ShopierOptions _options;
        private readonly ILogger<ShopierPaymentService> _logger;
        private readonly IPurchaseDocumentService _purchaseDocumentService;

        public ShopierPaymentService(
            UserManagementContext db,
            IUserService userService,
            IOptions<ShopierOptions> options,
            ILogger<ShopierPaymentService> logger,
            IPurchaseDocumentService purchaseDocumentService)
        {
            _db = db;
            _userService = userService;
            _options = options.Value;
            _logger = logger;
            _purchaseDocumentService = purchaseDocumentService;
        }

        public async Task<Result<ShopierCheckoutResponse>> StartPay(long userId)
        {
            var roleId = await _db.UserRoles
                .AsNoTracking()
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .Select(x => x.RoleId)
                .FirstOrDefaultAsync();

            return await StartPackageInternal(userId, roleId, 0, "Pay");
        }

        public Task<Result<ShopierCheckoutResponse>> StartPackage(
            long userId,
            long planId,
            long memoryId) =>
            StartPackageInternal(userId, planId, memoryId, "Package");

        private async Task<Result<ShopierCheckoutResponse>> StartPackageInternal(
            long userId,
            long planId,
            long memoryId,
            string type)
        {
            var result = new Result<ShopierCheckoutResponse>();

            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId && !x.IsDeleted);

            if (user == null)
                return Fail(result, "Kullanıcı bulunamadı.");

            if (!_options.Products.TryGetValue(planId, out var product) ||
                string.IsNullOrWhiteSpace(product.ProductId) ||
                string.IsNullOrWhiteSpace(product.Url))
            {
                return Fail(result, "Bu paket için Shopier ürün eşleştirmesi bulunamadı.");
            }

            var existing = await _db.ShopierPayments
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId &&
                    x.PurchaseType == type &&
                    x.PlanId == planId &&
                    x.MemoryId == memoryId &&
                    x.Status == "Pending" &&
                    !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();

            if (existing != null)
                return Success(result, existing, "Bekleyen ödeme bulundu.");

            var payment = new ShopierPayment
            {
                Reference = Guid.NewGuid(),
                UserId = userId,
                PlanId = planId,
                MemoryId = memoryId,
                PurchaseType = type,
                ProductId = product.ProductId,
                ProductUrl = product.Url,
                BuyerEmail = NormalizeEmail(user.Email),
                Status = "Pending",
                CreatedDate = DateTime.UtcNow
            };

            _db.ShopierPayments.Add(payment);
            await _db.SaveChangesAsync();

            return Success(result, payment, "Ödeme oluşturuldu.");
        }

        public async Task<Result<ShopierCheckoutResponse>> StartGift(UserVoucher voucher)
        {
            var result = new Result<ShopierCheckoutResponse>();

            if (!_options.Products.TryGetValue(voucher.PlanId, out var product) ||
                string.IsNullOrWhiteSpace(product.ProductId) ||
                string.IsNullOrWhiteSpace(product.Url))
            {
                return Fail(result, "Hediye paketi için Shopier ürün eşleştirmesi bulunamadı.");
            }

            // Guest istemciler eski sürümlerde UserId=0 gönderebilir.
            // 0 ve negatif değerleri guest olarak normalize ediyoruz.
            long? giftUserId = voucher.UserId.HasValue && voucher.UserId.Value > 0
                ? voucher.UserId.Value
                : null;

            string? buyerEmail;

            if (giftUserId.HasValue)
            {
                buyerEmail = await _db.Users
                    .AsNoTracking()
                    .Where(x => x.Id == giftUserId.Value && !x.IsDeleted)
                    .Select(x => x.Email)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrWhiteSpace(buyerEmail))
                    return Fail(result, "Ödeme yapan kullanıcı bulunamadı veya e-posta adresi tanımlı değil.");
            }
            else
            {
                buyerEmail = voucher.SenderEmail;
            }

            if (string.IsNullOrWhiteSpace(buyerEmail))
                return Fail(result, "Ödeme yapan kişinin e-posta adresi zorunludur.");

            voucher.UserId = giftUserId;

            // Login kullanıcı için bekleyen ödeme tekrar kullanılabilir. Guest'te kullanıcı
            // anahtarı olmadığı için yeni reference üretilir.
            if (giftUserId.HasValue)
            {
                var existing = await _db.ShopierPayments
                    .AsNoTracking()
                    .Where(x =>
                        x.UserId == giftUserId.Value &&
                        x.PurchaseType == "Gift" &&
                        x.PlanId == voucher.PlanId &&
                        x.Status == "Pending" &&
                        !x.IsDeleted)
                    .OrderByDescending(x => x.CreatedDate)
                    .FirstOrDefaultAsync();

                if (existing != null)
                    return Success(result, existing, "Bekleyen ödeme bulundu.");
            }

            var payment = new ShopierPayment
            {
                Reference = Guid.NewGuid(),
                UserId = giftUserId,
                PlanId = voucher.PlanId,
                PurchaseType = "Gift",
                ProductId = product.ProductId,
                ProductUrl = product.Url,
                BuyerEmail = NormalizeEmail(buyerEmail),
                GiftPayload = JsonConvert.SerializeObject(voucher),
                Status = "Pending",
                CreatedDate = DateTime.UtcNow
            };

            _db.ShopierPayments.Add(payment);
            await _db.SaveChangesAsync();

            return Success(result, payment, "Ödeme oluşturuldu.");
        }

        public async Task<Result<ShopierCheckoutResponse>> GetPending(
            long userId,
            string purchaseType,
            long planId,
            long memoryId)
        {
            var result = new Result<ShopierCheckoutResponse>();

            var payment = await _db.ShopierPayments
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId &&
                    x.PurchaseType == purchaseType &&
                    x.PlanId == planId &&
                    x.MemoryId == memoryId &&
                    x.Status == "Pending" &&
                    !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefaultAsync();

            return payment == null
                ? Fail(result, "Bekleyen ödeme bulunamadı.")
                : Success(result, payment, "Bekleyen ödeme bulundu.");
        }

        /*
         * Eski Orders API doğrulaması kaldırıldı. Bu metot endpoint uyumluluğu
         * için yalnızca veritabanındaki mevcut durumu döndürür.
         */
        public Task<Result<ShopierPaymentStatusResponse>> Confirm(Guid reference) =>
            GetStatus(reference);

        public async Task<Result<ShopierPaymentStatusResponse>> GetStatus(Guid reference)
        {
            var result = new Result<ShopierPaymentStatusResponse>();

            var payment = await _db.ShopierPayments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Reference == reference && !x.IsDeleted);

            return payment == null
                ? FailStatus(result, "Ödeme kaydı bulunamadı.")
                : StatusSuccess(result, payment);
        }

        public async Task<ShopierOsbResult> HandleOsbAsync(
            IReadOnlyDictionary<string, string> form,
            string? authorizationHeader,
            CancellationToken cancellationToken = default)
        {
            var response = new ShopierOsbResult();
            ShopierFileLogger.Info($"HandleOsbAsync basladi. FormFieldCount={form.Count}");

            var res = FirstValue(form, "res");
            var receivedHash = FirstValue(form, "hash");

            if (string.IsNullOrWhiteSpace(res) || string.IsNullOrWhiteSpace(receivedHash))
            {
                response.Message = "Shopier OSB res/hash parametreleri bulunamadı.";
                ShopierFileLogger.Warning(response.Message);
                return response;
            }

            if (!ValidateShopierHash(res, receivedHash))
            {
                response.Message = "Shopier OSB hash doğrulaması başarısız.";
                ShopierFileLogger.Warning(response.Message);
                return response;
            }

            response.IsAuthenticated = true;

            IReadOnlyDictionary<string, string> payload;
            try
            {
                payload = DecodeShopierPayload(res);
            }
            catch (Exception ex)
            {
                response.Message = "Shopier OSB res içeriği çözümlenemedi.";
                ShopierFileLogger.Error(response.Message, ex);
                return response;
            }

            ShopierFileLogger.Info($"Shopier OSB payload decode edildi. FieldCount={payload.Count}");
            ShopierFileLogger.WriteForm(payload);

            if (IsTestNotification(payload))
            {
                response.IsTest = true;
                response.IsProcessed = true;
                response.Message = "Shopier OSB test bildirimi başarıyla alındı.";
                ShopierFileLogger.Info(response.Message);
                return response;
            }

            var orderId = FirstValue(payload, "orderid", "order_id", "orderId");
            var buyerEmail = NormalizeEmail(FirstValue(payload, "email", "buyeremail", "buyer_email", "buyerEmail"));
            var productId = NormalizeValue(FirstValue(payload, "productid", "product_id", "productId"));

            _logger.LogInformation(
                "Shopier OSB payload çözümlendi. OrderId: {OrderId}, BuyerEmail: {BuyerEmail}, ProductId: {ProductId}",
                orderId,
                buyerEmail,
                productId);

            ShopierFileLogger.Info(
                $"Cozumlenen payload: OrderId={orderId}, BuyerEmail={buyerEmail}, ProductId={productId}");

            if (string.IsNullOrWhiteSpace(orderId))
            {
                response.Message = "Shopier OSB bildiriminde orderid bulunamadı.";
                ShopierFileLogger.Warning(response.Message);
                return response;
            }

            var alreadyProcessed = await _db.ShopierPayments
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.ShopierOrderId == orderId &&
                    !x.IsDeleted,
                    cancellationToken);

            if (alreadyProcessed != null)
            {
                response.IsProcessed = alreadyProcessed.Status == "Completed";
                response.Reference = alreadyProcessed.Reference;
                response.ShopierOrderId = orderId;
                response.Message = alreadyProcessed.Status == "Completed"
                    ? "Bu Shopier siparişi daha önce tamamlandı."
                    : $"Bu Shopier siparişi daha önce kaydedilmiş. Status={alreadyProcessed.Status}";

                ShopierFileLogger.Info(
                    $"ShopierOrderId daha once kaydedilmis. PaymentId={alreadyProcessed.Id}, Status={alreadyProcessed.Status}, Reference={alreadyProcessed.Reference}");
                return response;
            }

            ShopierPayment? payment = null;
            var oldestAllowed = DateTime.UtcNow.AddDays(-Math.Max(1, _options.OsbPendingWindowDays));

            if (!string.IsNullOrWhiteSpace(buyerEmail))
            {
                var candidates = await _db.ShopierPayments
                    .Where(x =>
                        x.Status == "Pending" &&
                        !x.IsDeleted &&
                        x.CreatedDate >= oldestAllowed &&
                        x.BuyerEmail.ToLower() == buyerEmail)
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(20)
                    .ToListAsync(cancellationToken);

                ShopierFileLogger.Info(
                    $"Pending aday aramasi: Email={buyerEmail}, ProductId={productId}, Count={candidates.Count}, WindowDays={Math.Max(1, _options.OsbPendingWindowDays)}");

                foreach (var candidate in candidates)
                {
                    ShopierFileLogger.Info(
                        $"Aday Payment: Id={candidate.Id}, Reference={candidate.Reference}, UserId={candidate.UserId}, ProductId={candidate.ProductId}, Type={candidate.PurchaseType}, CreatedDate={candidate.CreatedDate:O}");
                }

                if (!string.IsNullOrWhiteSpace(productId))
                {
                    payment = candidates
                        .Where(x => NormalizeValue(x.ProductId) == productId)
                        .OrderByDescending(x => x.CreatedDate)
                        .FirstOrDefault();
                }

                if (payment == null && candidates.Count == 1)
                    payment = candidates[0];
            }

            // Shopier bazı ürün tiplerinde e-posta formatını farklı gönderebilir.
            // E-posta ile bulunamadıysa ürün id'si tek bir Pending kayda işaret ediyorsa onu kullan.
            if (payment == null && !string.IsNullOrWhiteSpace(productId))
            {
                var productCandidates = await _db.ShopierPayments
                    .Where(x =>
                        x.Status == "Pending" &&
                        !x.IsDeleted &&
                        x.CreatedDate >= oldestAllowed &&
                        x.ProductId == productId)
                    .OrderByDescending(x => x.CreatedDate)
                    .Take(5)
                    .ToListAsync(cancellationToken);

                ShopierFileLogger.Info($"Product fallback: ProductId={productId}, Count={productCandidates.Count}");

                if (productCandidates.Count == 1)
                    payment = productCandidates[0];
            }

            if (payment == null)
            {
                response.ShopierOrderId = orderId;
                response.Message = "Shopier OSB için eşleşen Pending ödeme bulunamadı.";

                _logger.LogWarning(
                    "Shopier OSB eşleşmesi bulunamadı. OrderId: {OrderId}, Email: {Email}, ProductId: {ProductId}",
                    orderId,
                    buyerEmail,
                    productId);

                ShopierFileLogger.Warning(
                    $"ESLESME YOK. OrderId={orderId}, Email={buyerEmail}, ProductId={productId}");
                return response;
            }

            response.Reference = payment.Reference;
            response.ShopierOrderId = orderId;

            ShopierFileLogger.Info(
                $"ESLESEN PAYMENT bulundu. PaymentId={payment.Id}, Reference={payment.Reference}, UserId={payment.UserId}, Type={payment.PurchaseType}, PlanId={payment.PlanId}, MemoryId={payment.MemoryId}");

            var claimed = await _db.ShopierPayments
                .Where(x => x.Id == payment.Id && x.Status == "Pending")
                .ExecuteUpdateAsync(
                    setters => setters
                        .SetProperty(x => x.Status, "Processing")
                        .SetProperty(x => x.ShopierOrderId, orderId),
                    cancellationToken);

            ShopierFileLogger.Info($"Payment Processing claim sonucu: PaymentId={payment.Id}, UpdatedRowCount={claimed}");

            if (claimed == 0)
            {
                var current = await _db.ShopierPayments
                    .AsNoTracking()
                    .FirstAsync(x => x.Id == payment.Id, cancellationToken);

                response.IsProcessed = current.Status == "Completed";
                response.Message = current.Status == "Completed"
                    ? "Ödeme daha önce tamamlandı."
                    : "Ödeme başka bir işlem tarafından işleniyor.";

                return response;
            }

            try
            {
                // Guest Gift işleminde kullanıcı hesabı bulunmadığı için Agreement tablosuna
                // kayıt atmıyoruz. Buna rağmen ödeme ekranındaki checkbox ile kullanıcı
                // sözleşmeleri onaylamadan Shopier'e yönlendirilmemelidir.
                // Login kullanıcıların tüm ödeme tiplerinde DB agreement kaydı zorunludur.
                var isGuestGift = payment.PurchaseType == "Gift" && !payment.UserId.HasValue;

                if (!isGuestGift)
                {
                    var requiredAgreementCount = await _db.UserAgreementAcceptances
                        .AsNoTracking()
                        .CountAsync(x =>
                            x.RelatedReference == payment.Reference.ToString() &&
                            (x.AgreementType == "PreInformationForm" ||
                             x.AgreementType == "DistanceSalesAgreement") &&
                            !x.IsDeleted,
                            cancellationToken);

                    ShopierFileLogger.Info($"Sozlesme kontrolu: Reference={payment.Reference}, AgreementCount={requiredAgreementCount}");

                    if (requiredAgreementCount < 2)
                    {
                        await ResetPending(payment.Id, cancellationToken);
                        response.Message = "Satış sözleşmesi onayları bulunamadı.";
                        ShopierFileLogger.Warning(response.Message + $" Reference={payment.Reference}");
                        return response;
                    }
                }
                else
                {
                    ShopierFileLogger.Info($"Guest Gift: Agreement DB kontrolu atlandi. Reference={payment.Reference}");
                }

                if (payment.PurchaseType == "Gift")
                {
                    var voucher = JsonConvert.DeserializeObject<UserVoucher>(payment.GiftPayload ?? string.Empty);

                    if (voucher == null)
                    {
                        await ResetPending(payment.Id, cancellationToken);
                        response.Message = "Hediye bilgileri okunamadı.";
                        ShopierFileLogger.Warning(response.Message);
                        return response;
                    }

                    var giftResult = await _userService.BuyGiftPackage(voucher);

                    if (giftResult.GetIsSuccess() != true)
                    {
                        await ResetPending(payment.Id, cancellationToken);
                        response.Message = giftResult.GetMessage() ?? "Hediye paketi tanımlanamadı.";
                        ShopierFileLogger.Warning(response.Message);
                        return response;
                    }
                }
                else
                {
                    if (!payment.UserId.HasValue)
                    {
                        await ResetPending(payment.Id, cancellationToken);
                        response.Message = "Ödeme kaydında kullanıcı bulunamadı.";
                        ShopierFileLogger.Warning(response.Message);
                        return response;
                    }

                    Result<bool> operation;

                    if (payment.PurchaseType == "Pay")
                    {
                        operation = await _userService.Pay(payment.UserId.Value);
                    }
                    else
                    {
                        operation = await _userService.BuyPackage(
                            payment.UserId.Value,
                            payment.PlanId,
                            payment.MemoryId);
                    }

                    if (operation.GetIsSuccess() != true)
                    {
                        await ResetPending(payment.Id, cancellationToken);
                        response.Message = operation.GetMessage() ?? "Paket tanımlanamadı.";
                        ShopierFileLogger.Warning(response.Message);
                        return response;
                    }
                }

                var completedDate = DateTime.UtcNow;

                await _db.ShopierPayments
                    .Where(x => x.Id == payment.Id)
                    .ExecuteUpdateAsync(
                        setters => setters
                            .SetProperty(x => x.Status, "Completed")
                            .SetProperty(x => x.ShopierOrderId, orderId)
                            .SetProperty(x => x.CompletedDate, completedDate),
                        cancellationToken);

                // PDF sözleşmeler ödeme başarıyla kesinleştikten sonra ödeme yapan
                // kişinin e-posta adresine gönderilir. E-posta problemi ödeme
                // işlemini geri almamalı; hata loglanır ve OSB success döner.
                payment.Status = "Completed";
                payment.ShopierOrderId = orderId;
                payment.CompletedDate = completedDate;

                try
                {
                    await _purchaseDocumentService.SendPurchaseDocumentsAsync(
                        payment,
                        orderId,
                        cancellationToken);
                }
                catch (Exception mailException)
                {
                    _logger.LogError(
                        mailException,
                        "Ödeme tamamlandı ancak satış sözleşmeleri e-posta ile gönderilemedi. Reference: {Reference}, Email: {Email}",
                        payment.Reference,
                        payment.BuyerEmail);

                    ShopierFileLogger.Error(
                        $"CONTRACT EMAIL FAILED. Reference={payment.Reference}, Email={payment.BuyerEmail}",
                        mailException);
                }

                response.IsProcessed = true;
                response.Message = "Shopier ödemesi başarıyla tamamlandı.";

                _logger.LogInformation(
                    "Shopier OSB ödemesi tamamlandı. Reference: {Reference}, OrderId: {OrderId}, Type: {PurchaseType}",
                    payment.Reference,
                    orderId,
                    payment.PurchaseType);

                ShopierFileLogger.Info(
                    $"COMPLETED. PaymentId={payment.Id}, Reference={payment.Reference}, OrderId={orderId}, Type={payment.PurchaseType}");

                return response;
            }
            catch (Exception ex)
            {
                await ResetPending(payment.Id, cancellationToken);

                _logger.LogError(
                    ex,
                    "Shopier OSB bildirimi işlenirken hata oluştu. Reference: {Reference}, OrderId: {OrderId}",
                    payment.Reference,
                    orderId);

                ShopierFileLogger.Error(
                    $"HandleOsbAsync EXCEPTION. PaymentId={payment.Id}, Reference={payment.Reference}, OrderId={orderId}",
                    ex);

                throw;
            }
        }

        private bool ValidateShopierHash(string res, string receivedHash)
        {
            if (string.IsNullOrWhiteSpace(_options.OsbUsername) ||
                string.IsNullOrWhiteSpace(_options.OsbPassword))
            {
                _logger.LogError("Shopier OSB username/key sunucuda tanımlı değil.");
                ShopierFileLogger.Warning("Shopier OSB username/key appsettings içinde tanımlı değil.");
                return false;
            }

            try
            {
                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.OsbPassword));
                var source = res + _options.OsbUsername;
                var computedBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(source));
                var computedHash = Convert.ToHexString(computedBytes).ToLowerInvariant();
                var suppliedHash = receivedHash.Trim().ToLowerInvariant();

                var computed = Encoding.ASCII.GetBytes(computedHash);
                var supplied = Encoding.ASCII.GetBytes(suppliedHash);
                var valid = computed.Length == supplied.Length &&
                            CryptographicOperations.FixedTimeEquals(computed, supplied);

                ShopierFileLogger.Info(
                    $"Shopier OSB hash kontrolu: {(valid ? "BASARILI" : "BASARISIZ")}. " +
                    $"ResLength={res.Length}, ReceivedHashLength={suppliedHash.Length}, ConfigUsername={(!string.IsNullOrWhiteSpace(_options.OsbUsername))}, ConfigKey={(!string.IsNullOrWhiteSpace(_options.OsbPassword))}");
                return valid;
            }
            catch (Exception ex)
            {
                ShopierFileLogger.Error("Shopier OSB hash kontrolünde hata oluştu.", ex);
                return false;
            }
        }

        private static IReadOnlyDictionary<string, string> DecodeShopierPayload(string res)
        {
            // Bazı application/x-www-form-urlencoded katmanları '+' karakterini boşluğa
            // çevirebilir. Shopier standard Base64 kullandığı için decode öncesi bunu tolere et.
            var normalizedRes = res.Trim().Replace(' ', '+');
            var jsonBytes = Convert.FromBase64String(normalizedRes);
            var json = Encoding.UTF8.GetString(jsonBytes);
            var root = JObject.Parse(json);

            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var property in root.Properties())
                result[property.Name] = TokenToScalar(property.Value);

            return result;
        }

        private static string TokenToScalar(JToken token)
        {
            if (token.Type is JTokenType.Null or JTokenType.Undefined)
                return string.Empty;

            if (token is JValue value)
                return value.Value?.ToString() ?? string.Empty;

            // Shopier bazı hesaplarda productid gibi alanları tek elemanlı dizi
            // olarak döndürebiliyor. Eşleşmede doğrudan ilk gerçek değeri kullan.
            if (token is JArray array)
            {
                var first = array.FirstOrDefault(x => x.Type is not JTokenType.Null and not JTokenType.Undefined);
                return first == null ? string.Empty : TokenToScalar(first);
            }

            return token.ToString(Formatting.None);
        }

        private bool ValidateOsbCredentials(
            IReadOnlyDictionary<string, string> form,
            string? authorizationHeader)
        {
            if (string.IsNullOrWhiteSpace(_options.OsbUsername) ||
                string.IsNullOrWhiteSpace(_options.OsbPassword))
            {
                _logger.LogError(
                    "Shopier OSB kullanıcı adı veya şifresi sunucuda tanımlı değil.");
                return false;
            }

            string? username = null;
            string? password = null;

            if (!string.IsNullOrWhiteSpace(authorizationHeader) &&
                authorizationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var encoded = authorizationHeader[6..].Trim();
                    var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                    var separator = decoded.IndexOf(':');

                    if (separator >= 0)
                    {
                        username = decoded[..separator];
                        password = decoded[(separator + 1)..];
                    }
                }
                catch (FormatException)
                {
                    return false;
                }
            }

            username ??= FirstValue(form,
                "osb_username",
                "osb_user",
                "username",
                "user");

            password ??= FirstValue(form,
                "osb_password",
                "osb_pass",
                "password",
                "pass");

            return FixedTimeEquals(username, _options.OsbUsername) &&
                   FixedTimeEquals(password, _options.OsbPassword);
        }

        private static bool FixedTimeEquals(string? left, string? right)
        {
            if (left == null || right == null)
                return false;

            var leftBytes = Encoding.UTF8.GetBytes(left);
            var rightBytes = Encoding.UTF8.GetBytes(right);

            return leftBytes.Length == rightBytes.Length &&
                   CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
        }

        private static bool IsTestNotification(IReadOnlyDictionary<string, string> form)
        {
            var test = NormalizeValue(FirstValue(form,
                "test",
                "is_test",
                "isTest",
                "istest",
                "test_mode",
                "testMode",
                "testmode"));

            if (test is "1" or "true" or "yes" or "test")
                return true;

            var orderId = NormalizeValue(FirstValue(form,
                "order_id",
                "orderId",
                "orderid",
                "shopier_order_id",
                "shopierOrderId"));

            return orderId.Contains("test", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSuccessfulStatus(string status) =>
            status is "success" or "successful" or "paid" or "completed" or "complete";

        private static bool IsRejectedStatus(string status) =>
            status is "cancelled" or "canceled" or "cancel" or
                "refunded" or "refund" or "failed" or "rejected" or
                "void" or "deleted" or "iptal" or "iade";

        private Task ResetPending(long id, CancellationToken cancellationToken) =>
            _db.ShopierPayments
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(x => x.Status, "Pending"),
                    cancellationToken);

        private static string FirstValue(
            IReadOnlyDictionary<string, string> form,
            params string[] keys)
        {
            foreach (var key in keys)
            {
                var match = form.FirstOrDefault(x =>
                    string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrWhiteSpace(match.Value))
                    return match.Value.Trim();
            }

            return string.Empty;
        }

        private static string NormalizeEmail(string? value) =>
            (value ?? string.Empty).Trim().ToLowerInvariant();

        private static string NormalizeValue(string? value) =>
            (value ?? string.Empty).Trim().Trim('"').ToLowerInvariant();

        private static Result<ShopierCheckoutResponse> Success(
            Result<ShopierCheckoutResponse> result,
            ShopierPayment payment,
            string message)
        {
            result.SetData(new ShopierCheckoutResponse
            {
                Reference = payment.Reference,
                RedirectUrl = payment.ProductUrl,
                BuyerEmail = payment.BuyerEmail,
                Message = "Shopier ödeme sayfasında Styever hesabınızdaki e-posta adresini kullanın."
            });

            result.SetMessage(message);
            return result;
        }

        private static Result<ShopierCheckoutResponse> Fail(
            Result<ShopierCheckoutResponse> result,
            string message)
        {
            result.SetIsSuccess(false);
            result.SetMessage(message);
            return result;
        }

        private static Result<ShopierPaymentStatusResponse> StatusSuccess(
            Result<ShopierPaymentStatusResponse> result,
            ShopierPayment payment)
        {
            result.SetData(new ShopierPaymentStatusResponse
            {
                Reference = payment.Reference,
                Status = payment.Status,
                ShopierOrderId = payment.ShopierOrderId
            });

            result.SetMessage("İşlem başarılı.");
            return result;
        }

        private static Result<ShopierPaymentStatusResponse> FailStatus(
            Result<ShopierPaymentStatusResponse> result,
            string message)
        {
            result.SetIsSuccess(false);
            result.SetMessage(message);
            return result;
        }
    }
}
