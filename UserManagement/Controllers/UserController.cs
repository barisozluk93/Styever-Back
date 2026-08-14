using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Authorization;
using UserManagement.Entity;
using UserManagement.Interfaces;
using UserManagement.Model;
using UserManagement.Utils;

namespace UserManagement.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IShopierPaymentService _shopierPaymentService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IUserService userService,
            IShopierPaymentService shopierPaymentService,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _shopierPaymentService = shopierPaymentService;
            _logger = logger;
        }

        [HttpGet("Paginate")]
        [Authorize]
        [HasPermission("UserScene.Paging.Permission")]

        public async Task<IActionResult> Paginate([FromQuery] PagingParameter pagingParameter)
        {
            var result = await _userService.Paginate(pagingParameter);
            return new OkObjectResult(result);
        }

        [HttpGet("DashboardStats")]
        [Authorize]
        [HasPermission("UserScene.Paging.Permission")]
        public async Task<IActionResult> DashboardStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            return new OkObjectResult(await _userService.GetDashboardStats(startDate, endDate));
        }

        [HttpGet("All")]
        [Authorize]
        [HasPermission("UserScene.List.Permission")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetUsers();
            return new OkObjectResult(result);
        }

        [HttpPost("Save")]
        [Authorize]
        [HasPermission("UserScene.Save.Permission")]
        public async Task<IActionResult> Save([FromBody] User user)
        {
            var result = await _userService.Save(user);
            return new OkObjectResult(result);
        }

        [HttpPost("Update")]
        [Authorize]
        [HasPermission("UserScene.Edit.Permission")]
        public async Task<IActionResult> Update([FromBody] User user)
        {
            var result = await _userService.Update(user);
            return new OkObjectResult(result);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize]
        [HasPermission("UserScene.Delete.Permission")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _userService.Delete(id);
            return new OkObjectResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _userService.GetById(id, token);
            return new OkObjectResult(result);
        }

        [HttpGet("Pay/{id}")]
        [Authorize]
        [HasPermission("PaymentScene.MembershipPayment.Permission")]

        public async Task<IActionResult> Pay(long id)
        {
            var result = await _shopierPaymentService.StartPay(id);
            return new OkObjectResult(result);
        }

        [HttpGet("BuyPackage/{id}/{planId}/{memoryId}")]
        [Authorize]
        [HasPermission("PaymentScene.BuyMembership.Permission")]
        public async Task<IActionResult> BuyPackage(long id, long planId, long memoryId)
        {
            var result = await _shopierPaymentService.StartPackage(id, planId, memoryId);
            return new OkObjectResult(result);
        }

        [HttpPost("BuyGiftPackage")]

        public async Task<IActionResult> BuyGiftPackage([FromBody] UserVoucher userVoucher)
        {
            var result = await _shopierPaymentService.StartGift(userVoucher);
            return new OkObjectResult(result);
        }


        [HttpGet("PendingShopierPayment/{userId:long}/{purchaseType}/{planId:long}/{memoryId:long}")]
        public async Task<IActionResult> PendingShopierPayment(long userId, string purchaseType, long planId, long memoryId)
        {
            var result = await _shopierPaymentService.GetPending(userId, purchaseType, planId, memoryId);
            return new OkObjectResult(result);
        }

        [HttpPost("ConfirmShopierPayment/{reference:guid}")]
        public async Task<IActionResult> ConfirmShopierPayment(Guid reference)
        {
            var result = await _shopierPaymentService.Confirm(reference);
            return new OkObjectResult(result);
        }

        [HttpGet("ShopierPaymentStatus/{reference:guid}")]
        public async Task<IActionResult> ShopierPaymentStatus(Guid reference)
        {
            var result = await _shopierPaymentService.GetStatus(reference);
            return new OkObjectResult(result);
        }


        [AllowAnonymous]
        [HttpPost("ShopierOsb")]
        public async Task<IActionResult> ShopierOsb(CancellationToken cancellationToken)
        {
            ShopierFileLogger.Info(
                $"ShopierOsb ENDPOINT HIT. Method={Request.Method}, ContentType={Request.ContentType}, ContentLength={Request.ContentLength}, RemoteIp={HttpContext.Connection.RemoteIpAddress}");

            if (!string.IsNullOrWhiteSpace(ShopierFileLogger.LastSuccessfulPath))
                Response.Headers["X-Shopier-Log-Path"] = ShopierFileLogger.LastSuccessfulPath;

            if (!Request.HasFormContentType)
            {
                ShopierFileLogger.Warning($"ShopierOsb form content-type degil: {Request.ContentType}");

                Request.EnableBuffering();
                using var reader = new StreamReader(
                    Request.Body,
                    System.Text.Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    leaveOpen: true);

                var rawBody = await reader.ReadToEndAsync(cancellationToken);
                Request.Body.Position = 0;
                ShopierFileLogger.Info($"ShopierOsb RAW BODY: {rawBody}");

                return BadRequest("Form verisi bekleniyor.");
            }

            var requestForm = await Request.ReadFormAsync(cancellationToken);
            var form = requestForm.ToDictionary(
                x => x.Key,
                x => x.Value.ToString(),
                StringComparer.OrdinalIgnoreCase);

            var safeForm = form.ToDictionary(
                x => x.Key,
                x => IsSensitiveShopierField(x.Key) ? "***MASKED***" : x.Value,
                StringComparer.OrdinalIgnoreCase);

            _logger.LogInformation(
                "Shopier OSB isteği alındı. ContentType: {ContentType}, RemoteIp: {RemoteIp}, Form: {@Form}",
                Request.ContentType,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                safeForm);

            ShopierFileLogger.Info(
                $"ShopierOsb endpointine istek geldi. ContentType={Request.ContentType}, RemoteIp={HttpContext.Connection.RemoteIpAddress}");
            ShopierFileLogger.WriteForm(form);

            ShopierOsbResult result;
            try
            {
                result = await _shopierPaymentService.HandleOsbAsync(
                    form,
                    Request.Headers.Authorization.FirstOrDefault(),
                    cancellationToken);
            }
            catch (Exception ex)
            {
                ShopierFileLogger.Error("ShopierOsb endpointinde HandleOsbAsync exception olustu.", ex);
                _logger.LogError(ex, "ShopierOsb endpointinde HandleOsbAsync exception olustu.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Shopier bildirimi islenirken hata olustu.");
            }

            _logger.LogInformation(
                "Shopier OSB sonucu. Authenticated: {Authenticated}, Test: {IsTest}, Processed: {Processed}, Reference: {Reference}, OrderId: {OrderId}, Message: {Message}",
                result.IsAuthenticated,
                result.IsTest,
                result.IsProcessed,
                result.Reference,
                result.ShopierOrderId,
                result.Message);

            ShopierFileLogger.Info(
                $"Shopier OSB sonucu: Authenticated={result.IsAuthenticated}, Test={result.IsTest}, Processed={result.IsProcessed}, Reference={result.Reference}, OrderId={result.ShopierOrderId}, Message={result.Message}");

            if (!result.IsAuthenticated)
            {
                _logger.LogWarning("Shopier OSB isteği kimlik doğrulamasından geçemedi.");
                ShopierFileLogger.Warning("Shopier OSB kimlik dogrulamasi BASARISIZ.");
                return Unauthorized(result.Message);
            }

            if (result.IsTest)
            {
                ShopierFileLogger.Info("Shopier OSB test bildirimi; success donuluyor.");
                return Content("success", "text/plain", System.Text.Encoding.UTF8);
            }

            if (!result.IsProcessed)
            {
                _logger.LogWarning(
                    "Shopier OSB işlenemedi; success dönülmedi. OrderId: {OrderId}, Reference: {Reference}, Message: {Message}",
                    result.ShopierOrderId,
                    result.Reference,
                    result.Message);

                ShopierFileLogger.Warning($"Shopier OSB islenemedi; HTTP 500 donuluyor. Message={result.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
            }

            ShopierFileLogger.Info("Shopier OSB basariyla islendi; Shopier'e success donuluyor.");
            return Content("success", "text/plain", System.Text.Encoding.UTF8);
        }

        [AllowAnonymous]
        [HttpGet("ShopierOsbDiagnostics")]
        public IActionResult ShopierOsbDiagnostics([FromServices] Microsoft.Extensions.Options.IOptions<ShopierOptions> shopierOptions)
        {
            var options = shopierOptions.Value;
            return Ok(new
            {
                utcNow = DateTime.UtcNow,
                osbUsernameConfigured = !string.IsNullOrWhiteSpace(options.OsbUsername),
                osbKeyConfigured = !string.IsNullOrWhiteSpace(options.OsbPassword),
                accessTokenConfigured = !string.IsNullOrWhiteSpace(options.AccessToken),
                logger = ShopierFileLogger.Probe()
            });
        }

        private static bool IsSensitiveShopierField(string key)
        {
            var normalized = key.Replace("_", string.Empty).ToLowerInvariant();
            return normalized.Contains("password") ||
                   normalized.Contains("pass") ||
                   normalized.Contains("username") ||
                   normalized.Contains("osbuser") ||
                   normalized.Contains("signature") ||
                   normalized == "hash" ||
                   normalized == "res" ||
                   normalized.Contains("token") ||
                   normalized.Contains("authorization");
        }

        [HttpGet("VoucherControl/{voucher}")]

        public async Task<IActionResult> VoucherControl(string voucher)
        {
            var result = await _userService.VoucherControl(voucher);
            return new OkObjectResult(result);
        }

        [HttpPost("UserProfileEdit")]
        [Authorize]
        [HasPermission("ProfileScene.Edit.Permission")]
        public async Task<IActionResult> UserProfileEdit([FromBody] User user)
        {
            var result = await _userService.Update(user);
            return new OkObjectResult(result);
        }

        [HttpGet("UserAvatarUpdate/{id}/{fileId}")]
        [Authorize]
        [HasPermission("ProfileScene.Edit.Permission")]
        public async Task<IActionResult> UserAvatarUpdate(long id, long fileId)
        {
            var result = await _userService.UserAvatarUpdate(id, fileId);
            return new OkObjectResult(result);
        }

        [HttpGet("GetUserPermissions")]
        [Authorize]
        public async Task<IActionResult> GetUserPermissions()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _userService.GetUserPermissions(token);
            return new OkObjectResult(result);
        }

        [HttpGet("UserAddressList/{userId}")]
        [Authorize]
        [HasPermission("ProfileScene.ListAddress.Permission")]
        public async Task<IActionResult> GetUserAddresses(long userId)
        {
            var result = await _userService.GetUserAddresses(userId);
            return new OkObjectResult(result);
        }

        [HttpPost("UserAddressSave")]
        [Authorize]
        [HasPermission("ProfileScene.SaveAddress.Permission")]
        public async Task<IActionResult> UserAddressSave([FromBody] UserAddress userAddress)
        {
            var result = await _userService.UserAddressSave(userAddress);
            return new OkObjectResult(result);
        }

        [HttpPost("UserAddressUpdate")]
        [Authorize]
        [HasPermission("ProfileScene.EditAddress.Permission")]
        public async Task<IActionResult> UserAddressUpdate([FromBody] UserAddress userAddress)
        {
            var result = await _userService.UserAddressUpdate(userAddress);
            return new OkObjectResult(result);
        }

        [HttpDelete("UserAddressDelete/{id}")]
        [Authorize]
        [HasPermission("ProfileScene.DeletAddress.Permission")]
        public async Task<IActionResult> UserAddressDelete(long id)
        {
            var result = await _userService.UserAddressDelete(id);
            return new OkObjectResult(result);
        }

        [HttpGet("UserAddressById/{id}")]
        [Authorize]
        [HasPermission("ProfileScene.GetAddressById.Permission")]
        public async Task<IActionResult> GetUserAddressById(long id)
        {
            var result = await _userService.GetUserAddressById(id);
            return new OkObjectResult(result);
        }

        [HttpGet("GetPrimaryUserAddressById/{userId}")]
        
        public async Task<IActionResult> GetPrimaryUserAddressById(long userId)
        {
            var result = await _userService.GetPrimaryUserAddressById(userId);
            return new OkObjectResult(result);
        }
    }
}
