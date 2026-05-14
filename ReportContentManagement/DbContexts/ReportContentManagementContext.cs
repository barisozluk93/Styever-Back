using Microsoft.EntityFrameworkCore;
using ReportContentManagement.Entity;

namespace ReportContentManagement.DbContexts
{
    public class ReportContentManagementContext : DbContext
    {
        public ReportContentManagementContext(DbContextOptions<ReportContentManagementContext> options) : base(options)
        {
        }

        public DbSet<ReportContent> ReportContent { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
