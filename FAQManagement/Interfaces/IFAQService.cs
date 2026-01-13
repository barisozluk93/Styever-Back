using Microsoft.AspNetCore.Mvc;
using FAQManagement.Model;
using FAQManagement.Entity;

namespace FAQManagement.Interfaces
{
    public interface IFAQService
    {
        Task<Result<List<FAQ>>> GetAll();

    }
}
