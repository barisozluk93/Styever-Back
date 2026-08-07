using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Interfaces;
using UserManagement.Model;
namespace UserManagement.Controllers
{
    [Route("api/[controller]")] [ApiController]
    public class AgreementController : ControllerBase
    {
        private readonly IAgreementService _service;
        public AgreementController(IAgreementService service){_service=service;}
        [HttpPost("Accept")] [Authorize]
        public async Task<IActionResult> Accept([FromBody] List<AgreementAcceptanceRequest> requests)
        {
            var ip=HttpContext.Connection.RemoteIpAddress?.ToString();
            var ua=Request.Headers.UserAgent.FirstOrDefault();
            return Ok(await _service.Accept(requests,ip,ua));
        }
        [HttpGet("User/{userId:long}")] [Authorize]
        public async Task<IActionResult> UserAgreements(long userId)=>Ok(await _service.GetByUser(userId));
    }
}
