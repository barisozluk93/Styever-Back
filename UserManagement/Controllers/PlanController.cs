using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Authorization;
using UserManagement.Entity;
using UserManagement.Interfaces;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanController : ControllerBase
    {
        private readonly IPlanService _planService;

        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }

        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
            => Ok(await _planService.GetAll());

        [HttpGet("AdminGetAll")]
        [Authorize]
        [HasPermission("PlanScene.Paging.Permission")]
        public async Task<IActionResult> AdminGetAll()
            => Ok(await _planService.GetAll(true));

        [HttpGet("Get/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(long id)
            => Ok(await _planService.GetById(id));

        [HttpPost("Save")]
        [Authorize]
        [HasPermission("PlanScene.Save.Permission")]
        public async Task<IActionResult> Save([FromBody] Plan plan)
            => Ok(await _planService.Save(plan));

        [HttpPost("Update")]
        [Authorize]
        [HasPermission("PlanScene.Edit.Permission")]
        public async Task<IActionResult> Update([FromBody] Plan plan)
            => Ok(await _planService.Update(plan));

        [HttpDelete("Delete/{id}")]
        [Authorize]
        [HasPermission("PlanScene.Delete.Permission")]
        public async Task<IActionResult> Delete(long id)
            => Ok(await _planService.Delete(id));
    }
}
