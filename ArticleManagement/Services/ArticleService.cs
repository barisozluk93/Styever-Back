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
using System.Linq;
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
                        article.File = await GetFile(article.FileId, token);

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
                    result.SetIsSuccess(false);
                    result.SetMessage(ex.Message);
                }
            }

            return result;
        }

        public async Task<Result<List<Article>>> GetAll(string searchTerm, string language, string token)
        {
            var result = new Result<List<Article>>();

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                try
                {
                    var queryable = await _dbContext.Articles
                                        .Where(x => !x.IsDeleted && (!string.IsNullOrEmpty(searchTerm) ? (language == "tr" ? x.Header.ToLower().Contains(searchTerm.ToLower()) : x.HeaderEn.ToLower().Contains(searchTerm.ToLowerInvariant())) : true))
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

                    queryable.ForEach(x => x.File = GetFile(x.FileId, token).Result);
                    
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

        
        public async Task<Result<PagingResult<PagedList<Article>>>> Paginate(PagingParameter p, string token)
        {
            var r = new Result<PagingResult<PagedList<Article>>>();
            try
            {
                var q = _dbContext.Articles.AsNoTracking().AsQueryable();

                if (!string.IsNullOrWhiteSpace(p.FilterText))
                {
                    var f = p.FilterText.ToLower();
                    q = q.Where(x => x.Header.ToLower().Contains(f) || x.HeaderEn.ToLower().Contains(f) || x.SubHeader.ToLower().Contains(f) || x.SubHeaderEn.ToLower().Contains(f));
                }
                if (!string.IsNullOrWhiteSpace(p.Header)) q = q.Where(x => x.Header.ToLower().Contains(p.Header.ToLower()));
                if (!string.IsNullOrWhiteSpace(p.HeaderEn)) q = q.Where(x => x.HeaderEn.ToLower().Contains(p.HeaderEn.ToLower()));
                if (!string.IsNullOrWhiteSpace(p.SubHeader)) q = q.Where(x => x.SubHeader.ToLower().Contains(p.SubHeader.ToLower()));
                if (!string.IsNullOrWhiteSpace(p.SubHeaderEn)) q = q.Where(x => x.SubHeaderEn.ToLower().Contains(p.SubHeaderEn.ToLower()));
                if (p.IsDeleted.HasValue) q = q.Where(x => x.IsDeleted == p.IsDeleted.Value);
                var pagination = PagedList<Article>.ToPagedList(q.OrderByDescending(x => x.Id), p.PageNumber, p.PageSize);

                pagination.ForEach(x => x.File = GetFile(x.FileId, token).Result);

                r.SetData(new PagingResult<PagedList<Article>>()
                {
                    Items = pagination,
                    TotalCount = pagination.TotalCount,
                });

                r.SetMessage("İşlem başarı ile gerçekleşti.");
            }
            catch (Exception ex) { r.SetIsSuccess(false); r.SetMessage(ex.Message); }
            return r;
        }
        public async Task<Result<Article>> Save(Article item){var r=new Result<Article>();try{item.Id=0;item.IsDeleted=false;_dbContext.Articles.Add(item);await _dbContext.SaveChangesAsync();r.SetData(item);r.SetMessage("İşlem başarı ile gerçekleşti.");}catch(Exception ex){r.SetIsSuccess(false);r.SetMessage(ex.Message);}return r;}
        public async Task<Result<Article>> Update(Article item){var r=new Result<Article>();try{var db=await _dbContext.Articles.FirstOrDefaultAsync(x=>x.Id==item.Id);if(db==null){r.SetIsSuccess(false);r.SetMessage("Kayıt bulunamadı.");return r;}db.FileId=item.FileId;db.Header=item.Header;db.HeaderEn=item.HeaderEn;db.SubHeader=item.SubHeader;db.SubHeaderEn=item.SubHeaderEn;db.Content=item.Content;db.ContentEn=item.ContentEn;await _dbContext.SaveChangesAsync();r.SetData(db);r.SetMessage("İşlem başarı ile gerçekleşti.");}catch(Exception ex){r.SetIsSuccess(false);r.SetMessage(ex.Message);}return r;}
        public async Task<Result<Article>> Delete(long id){var r=new Result<Article>();try{var db=await _dbContext.Articles.FirstOrDefaultAsync(x=>x.Id==id);if(db==null){r.SetIsSuccess(false);r.SetMessage("Kayıt bulunamadı.");return r;}db.IsDeleted=true;await _dbContext.SaveChangesAsync();r.SetData(db);r.SetMessage("İşlem başarı ile gerçekleşti.");}catch(Exception ex){r.SetIsSuccess(false);r.SetMessage(ex.Message);}return r;}

        private async Task<Model.File> GetFile(long id, string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var fileUrl = _configuration["AppSettings:ApiUrl"] + "/api/File/" + id;
            var response = await client.GetAsync(fileUrl);

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
