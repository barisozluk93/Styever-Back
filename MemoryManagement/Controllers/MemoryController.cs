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
            var result = await _memoryService.MemoriesPaginate(pagingParameter);
            return new OkObjectResult(result);
        }

        [HttpPost("Save")]
        [Authorize]
        [HasPermission("MemoryScene.Save.Permission")]
        public async Task<IActionResult> Save([FromBody] Memory memory)
        {
            var result = await _memoryService.Save(memory);
            return new OkObjectResult(result);
        }

        [HttpPost("Update")]
        [Authorize]
        [HasPermission("MemoryScene.Edit.Permission")]
        public async Task<IActionResult> Update([FromBody] Memory memory)
        {
            var result = await _memoryService.Update(memory);
            return new OkObjectResult(result);
        }

        [HttpGet("ChangeBelongingIssuesUserMemory/{userId}/{memoryId}")]
        public async Task<IActionResult> ChangeBelongingIssuesUserMemory(long userId, long memoryId)
        {
            var result = await _memoryService.ChangeBelongingIssuesUserMemory(userId, memoryId);
            return new OkObjectResult(result);
        }

        [HttpGet("ActivateUserMemories/{userId}")]
        public async Task<IActionResult> ActivateUserMemories(long userId)
        {
            var result = await _memoryService.ActivateUserMemories(userId);
            return new OkObjectResult(result);
        }

        [HttpGet("DeactivateUserMemories/{userId}")]
        public async Task<IActionResult> DeactivateUserMemories(long userId)
        {
            var result = await _memoryService.DeactivateUserMemories(userId);
            return new OkObjectResult(result);
        }


        [HttpGet("{id}")]

        public async Task<IActionResult> GetById(long id)
        {
            var result = await _memoryService.GetById(id);
            return new OkObjectResult(result);
        }

        [HttpGet("GetMemoryCount/{userId}")]
        [Authorize]
        [HasPermission("MemoryScene.Count.Permission")]
        public async Task<IActionResult> GetMemoryCount(long userId)
        {
            var result = await _memoryService.GetMemoryCount(userId);
            return new OkObjectResult(result);
        }

        [HttpGet("SetMemoryFileIsPrimary/{memoryFileId}")]
        [Authorize]
        [HasPermission("MemoryScene.FileUpdate.Permission")]
        public async Task<IActionResult> SetMemoryFileIsPrimary(long memoryFileId)
        {
            var result = await _memoryService.SetMemoryFileIsPrimary(memoryFileId);
            return new OkObjectResult(result);
        }

        [HttpGet("LikeAll/{memoryId}")]
        public async Task<IActionResult> LikeAll(long memoryId)
        {
            var result = await _memoryService.LikeAll(memoryId);
            return new OkObjectResult(result);
        }

        [HttpGet("CandleAll/{memoryId}")]
        public async Task<IActionResult> CandleAll(long memoryId)
        {
            var result = await _memoryService.CandleAll(memoryId);
            return new OkObjectResult(result);
        }

        [HttpPost("LightCandle")]
        [Authorize]
        [HasPermission("MemoryScene.LightCandle.Permission")]
        public async Task<IActionResult> LightCandle([FromBody] MemoryCandle memoryCandle)
        {
            var result = await _memoryService.LightCandle(memoryCandle);
            return new OkObjectResult(result);
        }

        [HttpPost("UpdateCandle")]
        [Authorize]
        [HasPermission("MemoryScene.UpdateCandle.Permission")]
        public async Task<IActionResult> UpdateCandle([FromBody] MemoryCandle memoryCandle)
        {
            var result = await _memoryService.UpdateCandle(memoryCandle);
            return new OkObjectResult(result);
        }

        [HttpGet("CommentAll/{memoryId}")]
        public async Task<IActionResult> CommentAll(long memoryId)
        {
            var result = await _memoryService.CommentAll(memoryId);
            return new OkObjectResult(result);
        }

        [HttpPost("AddComment")]
        [Authorize]
        [HasPermission("MemoryScene.AddComment.Permission")]
        public async Task<IActionResult> AddComment([FromBody] MemoryComment memoryComment)
        {
            var result = await _memoryService.AddComment(memoryComment);
            return new OkObjectResult(result);
        }

        [HttpGet("DeleteComment/{commentId}")]
        [Authorize]
        [HasPermission("MemoryScene.DeleteComment.Permission")]
        public async Task<IActionResult> DeleteComment(long commentId)
        {
            var result = await _memoryService.DeleteComment(commentId);
            return new OkObjectResult(result);
        }

        [HttpPost("Like")]
        [Authorize]
        [HasPermission("MemoryScene.Like.Permission")]
        public async Task<IActionResult> Like([FromBody] MemoryLike memoryLike)
        {
            var result = await _memoryService.Like(memoryLike);
            return new OkObjectResult(result);
        }

        [HttpGet("Dislike/{memoryId}/{userId}")]
        [Authorize]
        [HasPermission("MemoryScene.Dislike.Permission")]
        public async Task<IActionResult> Dislike(long memoryId, long userId)
        {
            var result = await _memoryService.Dislike(memoryId, userId);
            return new OkObjectResult(result);
        }

        [HttpPost("MemoryFileAdd")]
        [Authorize]
        [HasPermission("MemoryScene.FileAdd.Permission")]
        public async Task<IActionResult> MemoryFileAdd([FromBody] MemoryFile memoryFile)
        {
            var result = await _memoryService.MemoryFileAdd(memoryFile);
            return new OkObjectResult(result);
        }

        [HttpDelete("MemoryFileDelete/{id}")]
        [Authorize]
        [HasPermission("MemoryScene.FileDelete.Permission")]
        public async Task<IActionResult> MemoryFileDelete(long id)
        {
            var result = await _memoryService.MemoryFileDelete(id);
            return new OkObjectResult(result);
        }

        [HttpPost("MemoryYoutubeLinkAdd")]
        [Authorize]
        [HasPermission("MemoryScene.FileAdd.Permission")]
        public async Task<IActionResult> MemoryYoutubeLinkAdd([FromBody] MemoryYoutubeLink memoryYoutubeLink)
        {
            var result = await _memoryService.MemoryYoutubeLinkAdd(memoryYoutubeLink);
            return new OkObjectResult(result);
        }

        [HttpDelete("MemoryYoutubeLinkDelete/{id}")]
        [Authorize]
        [HasPermission("MemoryScene.FileDelete.Permission")]
        public async Task<IActionResult> MemoryYoutubeLinkDelete(long id)
        {
            var result = await _memoryService.MemoryYoutubeLinkDelete(id);
            return new OkObjectResult(result);
        }
    }
}
