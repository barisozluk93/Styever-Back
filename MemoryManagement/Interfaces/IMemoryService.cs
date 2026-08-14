using Microsoft.AspNetCore.Mvc;
using MemoryManagement.Entity;
using MemoryManagement.Model;

namespace MemoryManagement.Interfaces
{
    public interface IMemoryService
    {
        Task<Result<PagingResult<PagedList<Memory>>>> MemoriesPaginate(PagingParameter pagingParameter);
        Task<Result<Memory>> Save(Memory memory);
        Task<Result<Memory>> Update(Memory memory);
        Task<Result<Memory>> GetById(long id);
        Task<Result<MemoryComment>> AddComment(MemoryComment memoryComment);
        Task<Result<MemoryComment>> ApproveComment(long id);
        Task<Result<MemoryComment>> DeleteComment(long commentId);
        Task<Result<MemoryLike>> Like(MemoryLike memoryLike);
        Task<Result<MemoryLike>> Dislike(long memoryId, long userId);
        Task<Result<List<MemoryComment>>> CommentAll(long memoryId);
        Task<Result<List<MemoryLike>>> LikeAll(long memoryId);
        Task<Result<MemoryFile>> MemoryFileAdd(MemoryFile memoryFile);
        Task<Result<MemoryFile>> MemoryFileDelete(long id);
        Task<Result<MemoryYoutubeLink>> MemoryYoutubeLinkAdd(MemoryYoutubeLink memoryFile);
        Task<Result<MemoryYoutubeLink>> MemoryYoutubeLinkDelete(long id);
        Task<Result<long>> GetMemoryCount(long userId);
        Task<Result<DashboardMemoryStats>> GetDashboardStats(DateTime? startDate, DateTime? endDate);
        Task<Result<bool>> SetMemoryFileIsPrimary(long memoryFileId);
        Task<Result<MemoryCandle>> LightCandle(MemoryCandle memoryCandle);
        Task<Result<MemoryCandle>> UpdateCandle(MemoryCandle memoryCandle);
        Task<Result<List<MemoryCandle>>> CandleAll(long memoryId);
        Task<Result<bool>> ActivateUserMemories(long userId);
        Task<Result<bool>> DeactivateUserMemories(long userId);
        Task<Result<bool>> SetBelongingIssuesToTrueUserMemory(long userId, long memoryId);
        Task<Result<bool>> SetBelongingIssuesToFalseUserMemory(long userId);

    }
}
