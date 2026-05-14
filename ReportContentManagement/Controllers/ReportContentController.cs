using Microsoft.AspNetCore.Mvc;
using ReportContentManagement.Entity;
using ReportContentManagement.Interfaces;

namespace ReportContentManagement.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class ReportContentController : ControllerBase
    {
        private readonly IReportContentService _reportContentService;

        public ReportContentController(IReportContentService reportContentService)
        {
            _reportContentService = reportContentService;
        }

        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] ReportContent reportContent)
        {
            var result = await _reportContentService.Save(reportContent);
            return new OkObjectResult(result);
        }
    }
}
