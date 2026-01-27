using ContactUsManagement.Entity;
using Microsoft.EntityFrameworkCore;


namespace ContactUsManagement.DbContexts

{
    public class ContactUsManagementContext : DbContext
    {
        public ContactUsManagementContext(DbContextOptions<ContactUsManagementContext> options) : base(options)
        {
           
        }

        public DbSet<ContactUs> ContactUs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
