using DashboardManagement.Authorization;
using DashboardManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DashboardManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;

        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        [HttpGet("Get")]
        [Authorize]
        [HasPermission("DashboardScene.View.Permission")]
        public async Task<IActionResult> Get(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            return Ok(
                await _service.Get(
                    startDate,
                    endDate,
                    Request.Headers.Authorization.FirstOrDefault()
                )
            );
        }
    }
}