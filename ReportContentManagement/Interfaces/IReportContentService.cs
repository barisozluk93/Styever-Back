using ReportContentManagement.Entity;
using ReportContentManagement.Model;

namespace ReportContentManagement.Interfaces
{
    public interface IReportContentService
    {
        Task<Result<ReportContent>> Save(ReportContent reportContent);
        Task<Result<long>> GetDashboardStats();
    }
}
