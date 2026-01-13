using Microsoft.AspNetCore.Mvc;
using ArticleManagement.Entity;
using ArticleManagement.Model;

namespace ArticleManagement.Interfaces
{
    public interface IArticleService
    {
        Task<Result<Article>> GetById(long id, string token);
        Task<Result<List<Article>>> GetAll(string searchTerm, string language, string token);

    }
}
