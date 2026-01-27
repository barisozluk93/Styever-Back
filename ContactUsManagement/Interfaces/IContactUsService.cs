using Microsoft.AspNetCore.Mvc;
using ContactUsManagement.Entity;
using ContactUsManagement.Model;

namespace ContactUsManagement.Interfaces
{
    public interface IContactUsService
    {
        Task<Result<ContactUs>> Save(ContactUs contactUs);
    }
}
