using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MemoryManagement.Authorization;
using MemoryManagement.Entity;
using MemoryManagement.Model;
using System.Globalization;
using System.Text;
using MemoryManagement.Interfaces;

namespace MemoryManagement.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class MemoryController : ControllerBase
    {
        private readonly IMemoryService _memoryService;

        public MemoryController(IMemoryService memoryService)
        {
            _memoryService = memoryService;
        }

        [HttpGet("Paginate")]
        public async Task<IActionResult> MemoriesPaginate([FromQuery] PagingParameter pagingParameter)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();
            var result = await _memoryService.MemoriesPaginate(pagingParameter, token);
            return new OkObjectResult(result);
        }

        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] Memory memory)
        {
            var result = await _memoryService.Save(memory);
            return new OkObjectResult(result);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] Memory memory)
        {
            var result = await _memoryService.Update(memory);
            return new OkObjectResult(result);
        }


        [HttpGet("{id}")]

        public async Task<IActionResult> GetById(long id)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _memoryService.GetById(id, token);
            return new OkObjectResult(result);
        }

        [HttpGet("GetMemoryCount/{userId}")]
        public async Task<IActionResult> GetMemoryCount(long userId)
        {
            var result = await _memoryService.GetMemoryCount(userId);
            return new OkObjectResult(result);
        }

        [HttpGet("SetMemoryFileIsPrimary/{memoryFileId}")]
        public async Task<IActionResult> SetMemoryFileIsPrimary(long memoryFileId)
        {
            var result = await _memoryService.SetMemoryFileIsPrimary(memoryFileId);
            return new OkObjectResult(result);
        }

        [HttpGet("LikeAll/{memoryId}")]
        public async Task<IActionResult> LikeAll(long memoryId)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();
            var result = await _memoryService.LikeAll(token, memoryId);
            return new OkObjectResult(result);
        }

        [HttpGet("CommentAll/{memoryId}")]
        public async Task<IActionResult> CommentAll(long memoryId)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();
            var result = await _memoryService.CommentAll(token, memoryId);
            return new OkObjectResult(result);
        }

        [HttpPost("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] MemoryComment memoryComment)
        {
            var result = await _memoryService.AddComment(memoryComment);
            return new OkObjectResult(result);
        }

        [HttpGet("DeleteComment/{commentId}")]
        public async Task<IActionResult> DeleteComment(long commentId)
        {
            var result = await _memoryService.DeleteComment(commentId);
            return new OkObjectResult(result);
        }

        [HttpPost("Like")]
        public async Task<IActionResult> Like([FromBody] MemoryLike memoryLike)
        {
            var result = await _memoryService.Like(memoryLike);
            return new OkObjectResult(result);
        }

        [HttpGet("Dislike/{memoryId}/{userId}")]
        public async Task<IActionResult> Dislike(long memoryId, long userId)
        {
            var result = await _memoryService.Dislike(memoryId, userId);
            return new OkObjectResult(result);
        }

        [HttpPost("MemoryFileAdd")]
        public async Task<IActionResult> MemoryFileAdd([FromBody] MemoryFile memoryFile)
        {
            var result = await _memoryService.MemoryFileAdd(memoryFile);
            return new OkObjectResult(result);
        }

        [HttpDelete("MemoryFileDelete/{id}")]
        public async Task<IActionResult> MemoryFileDelete(long id)
        {
            var result = await _memoryService.MemoryFileDelete(id);
            return new OkObjectResult(result);
        }
    }
}
