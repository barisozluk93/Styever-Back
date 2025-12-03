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

        public async Task<Result<Memory>> GetById(long id, string token)
        {
            var result = new Result<Memory>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var memory = await _dbContext.Memories.Include(x => x.Category).Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
                    if (memory != null)
                    {
                        memory.Files = await _dbContext.MemoryFiles.Where(x => x.MemoryId == id && !x.IsDeleted).ToListAsync();
                        memory.Files.ForEach(x => x.FileResult = GetFileResult(x.FileId, token).Result);
                        memory.Files.ForEach(x => x.FileName = GetFileName(x.FileId, token).Result);

                        memory.Comments = await _dbContext.MemoryComments.Where(x => x.MemoryId == id && !x.IsDeleted).ToListAsync();
                        memory.CommentsCount = await _dbContext.MemoryComments.Where(x => x.MemoryId == id && !x.IsDeleted).CountAsync();
                        memory.Comments.ForEach(x => x.UserName = GetUserName(x.UserId, token).Result);
                        memory.Comments.ForEach(x => x.UserAvatar = GetUserAvatar(x.UserId, token).Result);

                        memory.Likes = await _dbContext.MemoryLikes.Where(x => x.MemoryId == id && !x.IsDeleted).ToListAsync();
                        memory.LikesCount = await _dbContext.MemoryLikes.Where(x => x.MemoryId == id && !x.IsDeleted).CountAsync();
                        memory.Likes.ForEach(x => x.UserName = GetUserName(x.UserId, token).Result);
                        memory.Likes.ForEach(x => x.UserAvatar = GetUserAvatar(x.UserId, token).Result);

                        memory.UserName = await GetUserName(memory.UserId, token);
                        memory.UserCityCountry = await GetUserCityCountry(memory.UserId, token);

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

        public async Task<Result<PagingResult<PagedList<Memory>>>> MemoriesPaginate(PagingParameter pagingParameter, string token)
        {
            var result = new Result<PagingResult<PagedList<Memory>>>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    string lowerFilterText = string.IsNullOrEmpty(pagingParameter.FilterText) ? null : pagingParameter.FilterText.ToLower();


                    var queryable = _dbContext.Memories.Include(x => x.Category)
                                                       .Where(x => !x.IsDeleted && !x.IsPrivate &&
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
                        PostDate = s.PostDate,
                        Text = s.Text,
                        CommentsCount = _dbContext.MemoryComments.Where(x => x.MemoryId == s.Id && !x.IsDeleted).Count(),
                        Comments = _dbContext.MemoryComments.Where(x => x.MemoryId == s.Id && !x.IsDeleted).ToList(),
                        Likes = _dbContext.MemoryLikes.Where(x => x.MemoryId == s.Id && !x.IsDeleted).ToList(),
                        LikesCount = _dbContext.MemoryLikes.Where(x => x.MemoryId == s.Id && !x.IsDeleted).Count(),
                        Files = _dbContext.MemoryFiles.Where(x => x.MemoryId == s.Id && !x.IsDeleted).ToList()
                    });

                    var pagination = PagedList<Memory>.ToPagedList(queryable, pagingParameter.PageNumber, pagingParameter.PageSize);
                    pagination.ForEach(x => x.Files.ForEach(y => y.FileResult = GetFileResult(y.FileId, token).Result));
                    pagination.ForEach(x => x.UserName = GetUserName(x.UserId, token).Result);
                    pagination.ForEach(x => x.UserCityCountry = GetUserCityCountry(x.UserId, token).Result);
                    pagination.ForEach(x => x.Comments.ForEach(y => y.UserName = GetUserName(y.UserId, token).Result));
                    pagination.ForEach(x => x.Comments.ForEach(y => y.UserAvatar = GetUserAvatar(y.UserId, token).Result));
                    pagination.ForEach(x => x.Likes.ForEach(y => y.UserName = GetUserName(y.UserId, token).Result));
                    pagination.ForEach(x => x.Likes.ForEach(y => y.UserAvatar = GetUserAvatar(y.UserId, token).Result));

                    result.SetData(new PagingResult<PagedList<Memory>>()
                    {
                        Items = pagination,
                        TotalCount = pagination.TotalCount,
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

        public async Task<Result<List<MemoryComment>>> CommentAll(string token, long memoryId)
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
                                            MemoryId = memoryId
                                        }).ToListAsync();

                    queryable.ForEach(x => x.UserName = GetUserName(x.UserId, token).Result);
                    queryable.ForEach(x => x.UserAvatar = GetUserAvatar(x.UserId, token).Result);
                    
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

        public async Task<Result<List<MemoryLike>>> LikeAll(string token, long memoryId)
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

                    queryable.ForEach(x => x.UserName = GetUserName(x.UserId, token).Result);
                    queryable.ForEach(x => x.UserAvatar = GetUserAvatar(x.UserId, token).Result);

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
                    var memoryLike = await _dbContext.MemoryLikes.Where(x => x.MemoryId == memoryId && x.UserId == userId).FirstOrDefaultAsync();
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

        private async Task<string> GetUserCityCountry(long id, string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(_configuration["AppSettings:ApiUrl"] + "/api2/User/GetPrimaryUserAddressById" + id);

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

        private async Task<FileContentResult> GetUserAvatar(long id, string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(_configuration["AppSettings:ApiUrl"] + "/api2/User/" + id);

            if (response.IsSuccessStatusCode)
            {
                var responseStr = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseStr))
                {
                    try
                    {
                        Result<User> result = JsonConvert.DeserializeObject<Result<User>>(responseStr);

                        if (result.GetData().FileId.HasValue) {
                            return await GetFileResult(result.GetData().FileId.Value, token);
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


        private async Task<string> GetUserName(long id, string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(_configuration["AppSettings:ApiUrl"] + "/api2/User/" + id);

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

        private async Task<string> GetFileName(long id, string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(_configuration["AppSettings:ApiUrl"] + "/api2/File/" + id);

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


        private async Task<FileContentResult> GetFileResult(long id, string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(_configuration["AppSettings:ApiUrl"] + "/api2/File/" + id);

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
                            byte[] bytes = System.IO.File.ReadAllBytes(result.GetData().Path);
                            return new FileContentResult(bytes, result.GetData().ContentType);
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
