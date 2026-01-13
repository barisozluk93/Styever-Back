using FileManagement.Authorization;
using FileManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromForm] IFormFile file, [FromForm] int type)
        {
            var result = await _fileService.Save(file, type);
            return new OkObjectResult(result);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await _fileService.Delete(id);
            return new OkObjectResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _fileService.GetById(id);

            return new OkObjectResult(result);
        }

    }
}
