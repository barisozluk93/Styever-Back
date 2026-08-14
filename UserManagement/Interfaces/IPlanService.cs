using UserManagement.Entity;
using UserManagement.Model;

namespace UserManagement.Interfaces
{
    public interface IPlanService
    {
        Task<Result<List<Plan>>> GetAll(bool includeDeleted = false);
        Task<Result<Plan>> GetById(long id);
        Task<Result<Plan>> Save(Plan plan);
        Task<Result<Plan>> Update(Plan plan);
        Task<Result<Plan>> Delete(long id);
    }
}
