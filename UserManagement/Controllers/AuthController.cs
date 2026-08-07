using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Authorization;
using UserManagement.Entity;
using UserManagement.Interfaces;
using UserManagement.Model;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly IAuthService authService;
        readonly IAgreementService agreementService;

        public AuthController(IAuthService authService, IAgreementService agreementService)
        {
            this.authService = authService;
            this.agreementService = agreementService;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var result = await authService.Login(request);

            return new OkObjectResult(result);
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var result = await authService.Register(user);
            if (result.GetIsSuccess() == true && result.GetData() != null && user.AgreementAcceptances?.Count > 0)
            {
                user.AgreementAcceptances.ForEach(x => x.UserId = result.GetData()!.Id);
                await agreementService.Accept(user.AgreementAcceptances, HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers.UserAgent.FirstOrDefault());
            }
            return new OkObjectResult(result);
        }

        [HttpPost("RegisterWithVoucher")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterWithVoucher([FromBody] User user)
        {
            var result = await authService.RegisterWithVoucher(user);
            if (result.GetIsSuccess() == true && result.GetData() != null && user.AgreementAcceptances?.Count > 0)
            {
                user.AgreementAcceptances.ForEach(x => x.UserId = result.GetData()!.Id);
                await agreementService.Accept(user.AgreementAcceptances, HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers.UserAgent.FirstOrDefault());
            }
            return new OkObjectResult(result);
        }

        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await authService.RefreshToken(request);

            return new OkObjectResult(result);
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await authService.ForgotPassword(request);

            return new OkObjectResult(result);
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await authService.ResetPassword(request);

            return new OkObjectResult(result);
        }

        [HttpPost("ChangePassword")]
        [Authorize]
        [HasPermission("ProfileScene.ChangePw.Permission")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await authService.ChangePassword(request);

            return new OkObjectResult(result);
        }
    }
}
