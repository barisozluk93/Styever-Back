using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContactUsManagement.Authorization;
using ContactUsManagement.Entity;
using ContactUsManagement.Model;
using System.Globalization;
using System.Text;
using ContactUsManagement.Interfaces;

namespace ContactUsManagement.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        private readonly IContactUsService _contactUsService;

        public ContactUsController(IContactUsService contactUsService)
        {
            _contactUsService = contactUsService;
        }

        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] ContactUs contactUs)
        {
            var result = await _contactUsService.Save(contactUs);
            return new OkObjectResult(result);
        }
    }
}
