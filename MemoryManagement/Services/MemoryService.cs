using MemoryManagement.DbContexts;
using MemoryManagement.Entity;
using MemoryManagement.Interfaces;
using MemoryManagement.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Reflection;

namespace MemoryManagement.Services
{
    public class MemoryService : IMemoryService
    {
        private readonly MemoryManagementContext _dbContext;

        private readonly IConfiguration _configuration;

        public MemoryService(MemoryManagementContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<Result<bool>> SetMemoryFileIsPrimary(long memoryFileId)
        {
            var result = new Result<bool>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var memoryFile = await _dbContext.MemoryFiles.Where(x => x.Id == memoryFileId && !x.IsDeleted).FirstOrDefaultAsync();

                    var memoryFiles = await _dbContext.MemoryFiles.Where(x => x.Id != memoryFileId && x.MemoryId == memoryFile.MemoryId && !x.IsDeleted).ToListAsync();
                    memoryFiles.ForEach(x => x.IsPrimary = false);
                    await _dbContext.SaveChangesAsync();

                    memoryFile.IsPrimary = true;
                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();

                    result.SetData(true);
                    result.SetIsSuccess(true);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }

            return result;
        }

        public async Task<Result<long>> GetMemoryCount(long userId)
        {
            var result = new Result<long>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var count = await _dbContext.Memories.Where(x => x.UserId == userId && !x.IsDeleted).CountAsync();
                    result.SetData(count);
                    result.SetIsSuccess(true);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }

            return result;
        }

        public async Task<Result<Memory>> GetById(long id)
        {
            var result = new Result<Memory>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var memory = await _dbContext.Memories.Include(x => x.Category).Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
                    if (memory != null)
                    {
                        memory.HasDonation = HasDonation(memory.UserId).Result;

                        memory.YoutubeLinks = await _dbContext.MemoryYoutubeLinks.Where(x => x.MemoryId == id && !x.IsDeleted).ToListAsync();

                        memory.Files = await _dbContext.MemoryFiles.Where(x => x.MemoryId == id && !x.IsDeleted).ToListAsync();
                        memory.Files.ForEach(x => x.File = GetFile(x.FileId).Result);
                        memory.Files.ForEach(x => x.FileName = GetFileName(x.FileId).Result);

                        memory.Candles = await _dbContext.MemoryCandles.Where(x => x.MemoryId == id && !x.IsDeleted).GroupBy(x => x.UserId != null ? x.UserId.ToString() : x.NameSurname).Select(g => g.First()).ToListAsync();
                        memory.CandlesCount = await _dbContext.MemoryCandles.Where(x => x.MemoryId == id && !x.IsDeleted).GroupBy(x => x.UserId != null ? x.UserId.ToString() : x.NameSurname).CountAsync();
                        memory.Candles.ForEach(x => x.UserName = x.UserId.HasValue ? GetUserName(x.UserId.Value).Result : null);
                        memory.Candles.ForEach(x => x.UserAvatar = x.UserId.HasValue ? GetUserAvatar(x.UserId.Value).Result : null);

                        memory.Comments = await _dbContext.MemoryComments.Where(x => x.MemoryId == id && !x.IsDeleted).ToListAsync();
                        memory.CommentsCount = await _dbContext.MemoryComments.Where(x => x.MemoryId == id && !x.IsDeleted).CountAsync();
                        memory.Comments.ForEach(x => x.UserName = x.UserId.HasValue ? GetUserName(x.UserId.Value).Result : null);
                        memory.Comments.ForEach(x => x.UserAvatar = x.UserId.HasValue ? GetUserAvatar(x.UserId.Value).Result : null);

                        memory.Likes = await _dbContext.MemoryLikes.Where(x => x.MemoryId == id && !x.IsDeleted).ToListAsync();
                        memory.LikesCount = await _dbContext.MemoryLikes.Where(x => x.MemoryId == id && !x.IsDeleted).CountAsync();
                        memory.Likes.ForEach(x => x.UserName = GetUserName(x.UserId).Result);
                        memory.Likes.ForEach(x => x.UserAvatar = GetUserAvatar(x.UserId).Result);

                        memory.UserName = await GetUserName(memory.UserId);
                        memory.UserCityCountry = await GetUserCityCountry(memory.UserId);

                        result.SetData(memory);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Böyle bir kayıt bulunmamaktadır.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }

            return result;
        }

        public async Task<Result<bool>> SetBelongingIssuesToTrueUserMemory(long userId, long memoryId)
        {
            var result = new Result<bool>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var memories = await _dbContext.Memories.Include(x => x.Category).Where(x => x.UserId == userId && !x.IsDeleted && x.Id != memoryId).ToListAsync();
                    if (memories != null && memories.Count() > 0)
                    {
                        memories.ForEach(memory => memory.BelongingToOldPackage = true);
                        await _dbContext.SaveChangesAsync();

                        result.SetData(true);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetData(true);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<bool>> SetBelongingIssuesToFalseUserMemory(long userId)
        {
            var result = new Result<bool>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var memories = await _dbContext.Memories.Include(x => x.Category).Where(x => x.UserId == userId && !x.IsDeleted && x.BelongingToOldPackage).ToListAsync();
                    if (memories != null && memories.Count() > 0)
                    {
                        memories.ForEach(memory => memory.BelongingToOldPackage = false);
                        await _dbContext.SaveChangesAsync();

                        result.SetData(true);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetData(true);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }
        public async Task<Result<bool>> ActivateUserMemories(long userId)
        {
            var result = new Result<bool>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var memories = await _dbContext.Memories.Include(x => x.Category).Where(x => x.UserId == userId && x.IsDeleted).ToListAsync();
                    if (memories != null && memories.Count() > 0)
                    {
                        memories.ForEach(memory => memory.IsDeleted = false);
                        await _dbContext.SaveChangesAsync();

                        result.SetData(true);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetData(true);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<bool>> DeactivateUserMemories(long userId)
        {
            var result = new Result<bool>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var memories = await _dbContext.Memories.Include(x => x.Category).Where(x => x.UserId == userId && !x.IsDeleted).ToListAsync();
                    if (memories != null && memories.Count() > 0)
                    {
                        memories.ForEach(memory => memory.IsDeleted = true);
                        await _dbContext.SaveChangesAsync();

                        result.SetData(true);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetData(true);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<PagingResult<PagedList<Memory>>>> MemoriesPaginate(PagingParameter pagingParameter)
        {
            var result = new Result<PagingResult<PagedList<Memory>>>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {

                    string lowerFilterText = string.IsNullOrEmpty(pagingParameter.FilterText) ? null : pagingParameter.FilterText.ToLowerInvariant();
                    IQueryable<Memory> queryable;

                    if (pagingParameter.UserId.HasValue)
                    {
                        queryable = _dbContext.Memories.Include(x => x.Category)
                                                    .Where(x => !x.IsDeleted && x.UserId ==  pagingParameter.UserId &&
                                                    (String.IsNullOrEmpty(lowerFilterText) || (x.Name.ToLower().Contains(lowerFilterText))) &&
                                                                 (pagingParameter.CategoryId != null ? x.CategoryId == pagingParameter.CategoryId : true))
                        .Select(s => new Memory
                         {
                             Id = s.Id,
                             Name = s.Name,
                             BirthDate = s.BirthDate,
                             DeathDate = s.DeathDate,
                             UserId = s.UserId,
                             IsDeleted = s.IsDeleted,
                             Category = s.Category,
                             CategoryId = s.CategoryId,
                             IsOpenToComment = s.IsOpenToComment,
                             IsPrivate = s.IsPrivate,
                             IsLinkOnly = s.IsLinkOnly,
                             PostDate = s.PostDate,
                             Text = s.Text,
                             CandlesCount = _dbContext.MemoryCandles.Where(x => x.MemoryId == s.Id && !x.IsDeleted).GroupBy(x => x.UserId).Count(),
                             Candles = _dbContext.MemoryCandles.Where(x => x.MemoryId == s.Id && !x.IsDeleted).GroupBy(x => x.UserId).Select(g => g.First()).ToList(),
                             CommentsCount = _dbContext.MemoryComments.Where(x => x.MemoryId == s.Id && !x.IsDeleted).Count(),
                             Comments = _dbContext.MemoryComments.Where(x => x.MemoryId == s.Id && !x.IsDeleted).ToList(),
                             Likes = _dbContext.MemoryLikes.Where(x => x.MemoryId == s.Id && !x.IsDeleted).ToList(),
                             LikesCount = _dbContext.MemoryLikes.Where(x => x.MemoryId == s.Id && !x.IsDeleted).Count(),
                             Files = _dbContext.MemoryFiles.Where(x => x.MemoryId == s.Id && !x.IsDeleted).ToList(),
                             YoutubeLinks = _dbContext.MemoryYoutubeLinks.Where(x => x.MemoryId == s.Id && !x.IsDeleted).ToList(),
                             BelongingToOldPackage = s.BelongingToOldPackage
                        });
                    }
                    else
                    {
                        queryable = _dbContext.Memories.Include(x => x.Category)
                                                           .Where(x => !x.IsDeleted && !x.IsLinkOnly &&
                                                                 (String.IsNullOrEmpty(lowerFilterText) || (x.Name.ToLower().Contains(lowerFilterText))) &&
                                                                 (pagingParameter.CategoryId != null ? x.CategoryId == pagingParameter.CategoryId : true)
                                                            )
                        .Select(s => new Memory
                        {
                            Id = s.Id,
                            Name = s.Name,
                            BirthDate = s.BirthDate,
                            DeathDate = s.DeathDate,
                            UserId = s.UserId,
                            IsDeleted = s.IsDeleted,
                            Category = s.Category,
                            CategoryId = s.CategoryId,
                            IsOpenToComment = s.IsOpenToComment,
                            IsPrivate = s.IsPrivate,
                            PostDate = s.PostDate,
                            Text = s.Text,
                            CandlesCount = _dbContext.MemoryCandles.Where(x => x.MemoryId == s.Id && !x.IsDeleted).GroupBy(x => x.UserId != null ? x.UserId.ToString() : x.NameSurname).Count(),
                            Candles = _dbContext.MemoryCandles.Where(x => x.MemoryId == s.Id && !x.IsDeleted).GroupBy(x => x.UserId != null ? x.UserId.ToString() : x.NameSurname).Select(g => g.First()).ToList(),
                            CommentsCount = _dbContext.MemoryComments.Where(x => x.MemoryId == s.Id && !x.IsDeleted).Count(),
                            Comments = _dbContext.MemoryComments.Where(x => x.MemoryId == s.Id && !x.IsDeleted).ToList(),
                            Likes = _dbContext.MemoryLikes.Where(x => x.MemoryId == s.Id && !x.IsDeleted).ToList(),
                            LikesCount = _dbContext.MemoryLikes.Where(x => x.MemoryId == s.Id && !x.IsDeleted).Count(),
                            Files = _dbContext.MemoryFiles.Where(x => x.MemoryId == s.Id && !x.IsDeleted).ToList(),
                            YoutubeLinks = _dbContext.MemoryYoutubeLinks.Where(x => x.MemoryId == s.Id && !x.IsDeleted).ToList(),
                            BelongingToOldPackage = s.BelongingToOldPackage
                        });
                    }

                    var pagination = PagedList<Memory>.ToPagedList(queryable, pagingParameter.PageNumber, pagingParameter.PageSize);
                    pagination.ForEach(x => x.HasDonation = HasDonation(x.UserId).Result);
                    pagination.ForEach(x => x.Files.ForEach(y => y.File = GetFile(y.FileId).Result));
                    pagination.ForEach(x => x.UserName = GetUserName(x.UserId).Result);
                    pagination.ForEach(x => x.UserAvatar = GetUserAvatar(x.UserId).Result);
                    pagination.ForEach(x => x.UserCityCountry = GetUserCityCountry(x.UserId).Result);
                    pagination.ForEach(x => x.Candles.ForEach(y => y.UserName = y.UserId.HasValue ? GetUserName(y.UserId.Value).Result : null));
                    pagination.ForEach(x => x.Candles.ForEach(y => y.UserAvatar = y.UserId.HasValue ? GetUserAvatar(y.UserId.Value).Result: null));
                    pagination.ForEach(x => x.Comments.ForEach(y => y.UserName = y.UserId.HasValue ? GetUserName(y.UserId.Value).Result : null));
                    pagination.ForEach(x => x.Comments.ForEach(y => y.UserAvatar = y.UserId.HasValue ? GetUserAvatar(y.UserId.Value).Result : null));
                    pagination.ForEach(x => x.Likes.ForEach(y => y.UserName = GetUserName(y.UserId).Result));
                    pagination.ForEach(x => x.Likes.ForEach(y => y.UserAvatar = GetUserAvatar(y.UserId).Result));

                    result.SetData(new PagingResult<PagedList<Memory>>()
                    {
                        Items = pagination,
                        TotalCount = pagination.TotalCount,
                        TotalPages = pagination.TotalPages
                    });

                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<Memory>> Save(Memory memory)
        {
            var result = new Result<Memory>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    memory.IsDeleted = false;
                    memory.PostDate = DateTime.UtcNow;
                    _dbContext.Memories.Add(memory);
                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();

                    result.SetData(memory);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                   
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<Memory>> Update(Memory memory)
        {
            var result = new Result<Memory>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var oldMemory = await _dbContext.Memories.Where(x => x.Id == memory.Id && !x.IsDeleted).FirstOrDefaultAsync();

                    if (oldMemory != null)
                    {
                        if (!_dbContext.Memories.Where(x => x.Id != oldMemory.Id && (x.Name == memory.Name) && !x.IsDeleted).Any())
                        {
                            oldMemory.Name = memory.Name;
                            oldMemory.IsPrivate = memory.IsPrivate;
                            oldMemory.IsLinkOnly = memory.IsLinkOnly;
                            oldMemory.BirthDate = memory.BirthDate;
                            oldMemory.DeathDate = memory.DeathDate;
                            oldMemory.CategoryId = memory.CategoryId;
                            oldMemory.IsOpenToComment = memory.IsOpenToComment;
                            oldMemory.Text = memory.Text;

                            await _dbContext.SaveChangesAsync();
                            transaction.Commit();

                            result.SetData(memory);
                            result.SetMessage("İşlem başarı ile gerçekleşti.");
                        }
                        else
                        {
                            result.SetIsSuccess(false);
                            result.SetMessage("Aynı isim veya kodla tanımlı bir yetki bulunmaktadır.");
                        }
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Böyle bir kayıt bulunmamaktadır.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<MemoryYoutubeLink>> MemoryYoutubeLinkAdd(MemoryYoutubeLink memoryYoutubeLink)
        {
            var result = new Result<MemoryYoutubeLink>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    _dbContext.Add(memoryYoutubeLink);
                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();

                    result.SetData(memoryYoutubeLink);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<MemoryYoutubeLink>> MemoryYoutubeLinkDelete(long id)
        {
            var result = new Result<MemoryYoutubeLink>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var oldMemoryYoutubeLink = await _dbContext.MemoryYoutubeLinks.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
                    if (oldMemoryYoutubeLink != null)
                    {
                        oldMemoryYoutubeLink.IsDeleted = true;
                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();

                        result.SetData(oldMemoryYoutubeLink);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Böyle bir kayıt bulunmamaktadır.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<MemoryFile>> MemoryFileAdd(MemoryFile memoryFile)
        {
            var result = new Result<MemoryFile>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    _dbContext.Add(memoryFile);
                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();

                    result.SetData(memoryFile);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<MemoryFile>> MemoryFileDelete(long id)
        {
            var result = new Result<MemoryFile>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var oldMemoryFile = await _dbContext.MemoryFiles.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
                    if (oldMemoryFile != null)
                    {
                        oldMemoryFile.IsDeleted = true;
                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();

                        result.SetData(oldMemoryFile);
                        result.SetMessage("İşlem başarı ile gerçekleşti.");
                    }
                    else
                    {
                        result.SetIsSuccess(false);
                        result.SetMessage("Böyle bir kayıt bulunmamaktadır.");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }
        public async Task<Result<List<MemoryCandle>>> CandleAll(long memoryId)
        {
            var result = new Result<List<MemoryCandle>>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var queryable = await _dbContext.MemoryCandles.Include(x => x.Memory)
                                        .Where(x => x.MemoryId == memoryId && !x.IsDeleted)
                                        .Select(s => new MemoryCandle
                                        {
                                            Date = s.Date,
                                            Id = s.Id,
                                            IsDeleted = s.IsDeleted,
                                            UserId = s.UserId,
                                            MemoryId = memoryId,
                                            NameSurname = s.NameSurname
                                        })
                                        .GroupBy(x => x.UserId != null ? x.UserId.ToString() : x.NameSurname)
                                        .Select(s => s.First())
                                        .ToListAsync();

                    queryable.ForEach(x => x.UserName = x.UserId.HasValue ? GetUserName(x.UserId.Value).Result : null);
                    queryable.ForEach(x => x.UserAvatar = x.UserId.HasValue ? GetUserAvatar(x.UserId.Value).Result : null);

                    result.SetData(queryable);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }
        public async Task<Result<List<MemoryComment>>> CommentAll(long memoryId)
        {
            var result = new Result<List<MemoryComment>>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var queryable = await _dbContext.MemoryComments.Include(x => x.Memory)
                                        .Where(x => x.MemoryId == memoryId && !x.IsDeleted && (!string.IsNullOrEmpty(x.Comment) && !string.IsNullOrWhiteSpace(x.Comment)))
                                        .Select(s => new MemoryComment
                                        {
                                            Comment = s.Comment,
                                            Date = s.Date,
                                            Id = s.Id,
                                            IsDeleted = s.IsDeleted,
                                            UserId = s.UserId,
                                            MemoryId = memoryId,
                                            IsApproved = s.IsApproved,
                                            NameSurname = s.NameSurname
                                        }).ToListAsync();

                    queryable.ForEach(x => x.UserName = x.UserId.HasValue ? GetUserName(x.UserId.Value).Result : null);
                    queryable.ForEach(x => x.UserAvatar = x.UserId.HasValue ? GetUserAvatar(x.UserId.Value).Result : null);
                    
                    result.SetData(queryable);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<List<MemoryLike>>> LikeAll(long memoryId)
        {
            var result = new Result<List<MemoryLike>>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var queryable = await _dbContext.MemoryLikes.Include(x => x.Memory)
                                        .Where(x => x.MemoryId == memoryId && !x.IsDeleted)
                                        .Select(s => new MemoryLike
                                        {
                                            Date = s.Date,
                                            Id = s.Id,
                                            IsDeleted = s.IsDeleted,
                                            UserId = s.UserId,
                                            MemoryId = memoryId
                                        }).ToListAsync();

                    queryable.ForEach(x => x.UserName = GetUserName(x.UserId).Result);
                    queryable.ForEach(x => x.UserAvatar = GetUserAvatar(x.UserId).Result);

                    result.SetData(queryable);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }
        public async Task<Result<MemoryComment>> AddComment(MemoryComment memoryComment)
        {
            var result = new Result<MemoryComment>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    memoryComment.Date = DateTime.UtcNow;
                    _dbContext.MemoryComments.Add(memoryComment);
                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();

                    result.SetData(memoryComment);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<MemoryComment>> ApproveComment(long id)
        {
            var result = new Result<MemoryComment>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var memoryComment = await _dbContext.MemoryComments.Where(x => x.Id == id).FirstOrDefaultAsync();

                    memoryComment.IsApproved = true;
                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();

                    result.SetData(memoryComment);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<MemoryLike>> Like(MemoryLike memoryLike)
        {
            var result = new Result<MemoryLike>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    memoryLike.Date = DateTime.UtcNow;
                    _dbContext.MemoryLikes.Add(memoryLike);
                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();

                    result.SetData(memoryLike);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<MemoryLike>> Dislike(long memoryId, long userId)
        {
            var result = new Result<MemoryLike>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var memoryLike = await _dbContext.MemoryLikes.Where(x => x.MemoryId == memoryId && x.UserId == userId && !x.IsDeleted).FirstOrDefaultAsync();
                    memoryLike.IsDeleted = true;

                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();

                    result.SetData(memoryLike);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<MemoryComment>> DeleteComment(long commentId)
        {
            var result = new Result<MemoryComment>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var memoryComment = await _dbContext.MemoryComments.Where(x => x.Id == commentId).FirstOrDefaultAsync();
                    memoryComment.IsDeleted = true;

                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();

                    result.SetData(memoryComment);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<MemoryCandle>> LightCandle(MemoryCandle memoryCandle)
        {
            var result = new Result<MemoryCandle>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    memoryCandle.Date = DateTime.UtcNow;
                    _dbContext.MemoryCandles.Add(memoryCandle);
                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();

                    result.SetData(memoryCandle);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<MemoryCandle>> UpdateCandle(MemoryCandle memoryCandle)
        {
            var result = new Result<MemoryCandle>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    if (await _dbContext.MemoryCandles.Where(x => x.Id == memoryCandle.Id).AnyAsync())
                    {
                        var newCandle = await _dbContext.MemoryCandles.Where(x => x.Id == memoryCandle.Id).FirstOrDefaultAsync();
                        newCandle.Shelter = memoryCandle.Shelter;
                        newCandle.Donation = memoryCandle.Donation;

                        await _dbContext.SaveChangesAsync();
                        transaction.Commit();
                    }

                    result.SetData(memoryCandle);
                    result.SetMessage("İşlem başarı ile gerçekleşti.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        private async Task<bool> HasDonation(long id)
        {
            HttpClient client = new HttpClient();

            var response = await client.GetAsync(_configuration["AppSettings:ApiUrl"] + "/api/User/" + id);

            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseStr))
                {
                    try
                    {
                        Result<User> result = JsonConvert.DeserializeObject<Result<User>>(responseStr);
                        if (result.GetData().IsTrial && result.GetData().TrialExpirationDate < DateTime.UtcNow)
                        {
                            return false;
                        }
                        else
                        {
                            return true;

                        }
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private async Task<string> GetUserCityCountry(long id)
        {
            HttpClient client = new HttpClient();

            var response = await client.GetAsync(_configuration["AppSettings:ApiUrl"] + "/api/User/GetPrimaryUserAddressById" + id);

            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseStr))
                {
                    try
                    {
                        Result<UserAddress> result = JsonConvert.DeserializeObject<Result<UserAddress>>(responseStr);
                        string userCityCountry = result.GetData().City + "/" + result.GetData().Country;

                        return userCityCountry;
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }

                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        private async Task<Model.File> GetUserAvatar(long id)
        {
            HttpClient client = new HttpClient();

            var response = await client.GetAsync(_configuration["AppSettings:ApiUrl"] + "/api/User/" + id);

            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseStr))
                {
                    try
                    {
                        Result<User> result = JsonConvert.DeserializeObject<Result<User>>(responseStr);

                        if (result.GetData().FileId.HasValue) {
                            return await GetFile(result.GetData().FileId.Value);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }

                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


        private async Task<string> GetUserName(long id)
        {
            HttpClient client = new HttpClient();

            var response = await client.GetAsync(_configuration["AppSettings:ApiUrl"] + "/api/User/" + id);

            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseStr))
                {
                    try
                    {
                        Result<User> result = JsonConvert.DeserializeObject<Result<User>>(responseStr);
                        string userName = result.GetData().Name + " " + result.GetData().Surname;

                        return userName;
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }

                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        private async Task<string> GetFileName(long id)
        {
            HttpClient client = new HttpClient();

            var response = await client.GetAsync(_configuration["AppSettings:ApiUrl"] + "/api/File/" + id);

            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseStr))
                {
                    try
                    {
                        Result<Model.File> result = JsonConvert.DeserializeObject<Result<Model.File>>(responseStr);

                        if (result.GetData() != null)
                        {
                            return result.GetData().Name;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }

                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            return null;
        }

        private async Task<Model.File> GetFile(long id)
        {
            HttpClient client = new HttpClient();

            var response = await client.GetAsync(_configuration["AppSettings:ApiUrl"] + "/api/File/" + id);

            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseStr))
                {
                    try
                    {
                        Result<Model.File> result = JsonConvert.DeserializeObject<Result<Model.File>>(responseStr);

                        if (result.GetData() != null)
                        {
                            return result.GetData();
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }

                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            return null;
        }
    }
}
