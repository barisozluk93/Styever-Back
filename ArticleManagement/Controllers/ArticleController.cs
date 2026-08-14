using ArticleManagement.Authorization;
using ArticleManagement.Entity;
using ArticleManagement.Interfaces;
using ArticleManagement.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;

namespace ArticleManagement.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet("Paginate")]
        [Authorize]
        [HasPermission("SupportScene.Paging.Permission")]
        public async Task<IActionResult> Paginate([FromQuery] PagingParameter p)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _articleService.Paginate(p, token);
            return new OkObjectResult(result);
        }

        [HttpPost("Save")][Authorize][HasPermission("SupportScene.Save.Permission")] public async Task<IActionResult> Save([FromBody] Article item)=>Ok(await _articleService.Save(item));
        [HttpPost("Update")][Authorize][HasPermission("SupportScene.Edit.Permission")] public async Task<IActionResult> Update([FromBody] Article item)=>Ok(await _articleService.Update(item));
        [HttpDelete("Delete/{id:long}")][Authorize][HasPermission("SupportScene.Delete.Permission")] public async Task<IActionResult> Delete(long id)=>Ok(await _articleService.Delete(id));

        [HttpGet("{id}")]
        [Authorize]
        [HasPermission("SupportScene.GetById.Permission")]
        public async Task<IActionResult> GetById(long id)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _articleService.GetById(id, token);
            return new OkObjectResult(result);
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] PagingParameter pagingParameter)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();
            var result = await _articleService.GetAll(pagingParameter.FilterText, pagingParameter.Language, token);
            return new OkObjectResult(result);
        }
    }
}
