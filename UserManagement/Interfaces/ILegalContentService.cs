using UserManagement.Entity;
using UserManagement.Model;

namespace UserManagement.Interfaces
{
    public interface ILegalContentService
    {
        Task<Result<PagingResult<PagedList<LegalContent>>>> Paginate(PagingParameter pagingParameter);
        Task<Result<List<LegalContent>>> GetAll(bool includeDeleted = false);
        Task<Result<LegalContent>> GetById(long id);
        Task<Result<LegalContent>> GetBySlug(string slug);
        Task<Result<LegalContent>> Save(LegalContent item);
        Task<Result<LegalContent>> Update(LegalContent item);
        Task<Result<LegalContent>> Delete(long id);
    }
}
