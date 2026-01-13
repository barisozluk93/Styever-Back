using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArticleManagement.Authorization;
using ArticleManagement.Entity;
using ArticleManagement.Model;
using System.Globalization;
using System.Text;
using ArticleManagement.Interfaces;

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

        [HttpGet("{id}")]
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
