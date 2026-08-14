using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Authorization;
using UserManagement.Entity;
using UserManagement.Interfaces;

namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LegalContentController : ControllerBase
    {
        private readonly ILegalContentService _service;
        public LegalContentController(ILegalContentService service) => _service = service;

        [HttpGet("GetAll")] 
        [AllowAnonymous]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAll());

        [HttpGet("GetBySlug/{slug}")] 
        [AllowAnonymous]
        public async Task<IActionResult> GetBySlug(string slug) => Ok(await _service.GetBySlug(slug));

        [HttpGet("Paginate")]
        [Authorize]
        [HasPermission("LegalScene.Paging.Permission")]

        public async Task<IActionResult> Paginate([FromQuery] UserManagement.Model.PagingParameter pagingParameter)
            => Ok(await _service.Paginate(pagingParameter));

        [HttpGet("AdminGetAll")] 
        [Authorize]
        [HasPermission("LegalScene.Paging.Permission")]

        public async Task<IActionResult> AdminGetAll() => Ok(await _service.GetAll(true));

        [HttpGet("Get/{id}")] 
        [Authorize]

        public async Task<IActionResult> Get(long id) => Ok(await _service.GetById(id));

        [HttpPost("Save")] [Authorize]
        [HasPermission("LegalScene.Save.Permission")]

        public async Task<IActionResult> Save([FromBody] LegalContent item) => Ok(await _service.Save(item));

        [HttpPost("Update")] [Authorize]
        [HasPermission("LegalScene.Edit.Permission")]

        public async Task<IActionResult> Update([FromBody] LegalContent item) => Ok(await _service.Update(item));

        [HttpDelete("Delete/{id}")] [Authorize]
        [HasPermission("LegalScene.Delete.Permission")]

        public async Task<IActionResult> Delete(long id) => Ok(await _service.Delete(id));
    }
}
