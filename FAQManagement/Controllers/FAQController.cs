using FAQManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FAQManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FAQController : ControllerBase
    {
        private readonly IFAQService _faqService;

        public FAQController(IFAQService faqService)
        {
            _faqService = faqService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _faqService.GetAll();
            return new OkObjectResult(result);
        }
    }
}
