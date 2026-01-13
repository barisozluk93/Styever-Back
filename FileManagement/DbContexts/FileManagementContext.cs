using FileManagement.Entity;
using Microsoft.EntityFrameworkCore;


namespace FileManagement.DbContexts

{
    public class FileManagementContext : DbContext
    {
        public FileManagementContext(DbContextOptions<FileManagementContext> options) : base(options)
        {
        }

        public DbSet<Entity.File> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity.File>().HasData(
                new Entity.File { Id = 1, Name = "68a1a7ccdfe3241d0aa9f9ae_REF23", Length = 52501, Path = Path.Combine("C:/inetpub/api/FileManagement", "Uploads/Avatars", "68a1a7ccdfe3241d0aa9f9ae_REF23"), ContentType = "image/jpeg", Extension = ".jpeg", IsDeleted = false, },
                new Entity.File { Id = 2, Name = "68a1a7ccdfe3241d0aa9f9ae_REF28", Length = 52501, Path = Path.Combine("C:/inetpub/api/FileManagement", "Uploads/Avatars", "68a1a7ccdfe3241d0aa9f9ae_REF28"), ContentType = "image/jpeg", Extension = ".jpeg", IsDeleted = false, },
                new Entity.File { Id = 3, Name = "68a1a7ccdfe3241d0aa9f9ae_REF29", Length = 52501, Path = Path.Combine("C:/inetpub/api/FileManagement", "Uploads/Avatars", "68a1a7ccdfe3241d0aa9f9ae_REF29"), ContentType = "image/jpeg", Extension = ".jpeg", IsDeleted = false, },
                new Entity.File { Id = 7, Name = "68a1a7ccdfe3241d0aa9f9ae_REF22", Length = 270336, Path = Path.Combine("C:/inetpub/api/FileManagement", "Uploads/Articles", "68a1a7ccdfe3241d0aa9f9ae_REF22.jpg"), ContentType = "image/jpg", Extension = ".jpg", IsDeleted = false, },
                new Entity.File { Id = 8, Name = "68a1a8006928f76bab0bf47d_REF19", Length = 270336, Path = Path.Combine("C:/inetpub/api/FileManagement", "Uploads/Articles", "68a1a8006928f76bab0bf47d_REF19.jpg"), ContentType = "image/jpg", Extension = ".jpg", IsDeleted = false, },
                new Entity.File { Id = 9, Name = "68a1a990b816e33cfbd857ea_REF5", Length = 270336, Path = Path.Combine("C:/inetpub/api/FileManagement", "Uploads/Articles", "68a1a990b816e33cfbd857ea_REF5.jpg"), ContentType = "image/jpg", Extension = ".jpg", IsDeleted = false, }
            );
        }
    }
}
