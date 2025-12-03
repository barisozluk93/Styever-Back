using ArticleManagement.DbContexts;
using ArticleManagement.Entity;
using ArticleManagement.Interfaces;
using ArticleManagement.Model;
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

namespace ArticleManagement.Services
{
    public class ArticleService : IArticleService
    {
        private readonly ArticleManagementContext _dbContext;

        private readonly IConfiguration _configuration;

        public ArticleService(ArticleManagementContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<Result<Article>> GetById(long id, string token)
        {
            var result = new Result<Article>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var article = await _dbContext.Articles.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
                    if (article != null)
                    {
                        article.FileResult = GetFileResult(article.FileId, token).Result;

                        result.SetData(article);
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

        public async Task<Result<List<Article>>> GetAll(string searchTerm, string token)
        {
            var result = new Result<List<Article>>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var queryable = await _dbContext.Articles
                                        .Where(x => !x.IsDeleted && (!string.IsNullOrEmpty(searchTerm) ? x.Header.Contains(searchTerm) : true))
                                        .Select(s => new Article
                                        {
                                            Header = s.Header,
                                            HeaderEn = s.HeaderEn,
                                            Id = s.Id,
                                            IsDeleted = s.IsDeleted,
                                            FileId = s.FileId,
                                            SubHeader = s.SubHeader,
                                            SubHeaderEn = s.SubHeaderEn,
                                            Content = s.Content,
                                            ContentEn = s.ContentEn
                                        }).ToListAsync();

                    queryable.ForEach(x => x.FileResult = GetFileResult(x.FileId, token).Result);
                    
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
