using Microsoft.AspNetCore.Mvc;
using MemoryManagement.Entity;
using MemoryManagement.Model;

namespace MemoryManagement.Interfaces
{
    public interface IMemoryService
    {
        Task<Result<PagingResult<PagedList<Memory>>>> MemoriesPaginate(PagingParameter pagingParameter, string token);
        Task<Result<Memory>> Save(Memory memory);
        Task<Result<Memory>> Update(Memory memory);
        Task<Result<Memory>> GetById(long id, string token);
        Task<Result<MemoryComment>> AddComment(MemoryComment memoryComment);
        Task<Result<MemoryComment>> DeleteComment(long commentId);
        Task<Result<MemoryLike>> Like(MemoryLike memoryLike);
        Task<Result<MemoryLike>> Dislike(long memoryId, long userId);
        Task<Result<List<MemoryComment>>> CommentAll(string token, long memoryId);
        Task<Result<List<MemoryLike>>> LikeAll(string token, long memoryId);
        Task<Result<MemoryFile>> MemoryFileAdd(MemoryFile memoryFile);
        Task<Result<MemoryFile>> MemoryFileDelete(long id);
        Task<Result<long>> GetMemoryCount(long userId);
        Task<Result<bool>> SetMemoryFileIsPrimary(long memoryFileId);

    }
}
