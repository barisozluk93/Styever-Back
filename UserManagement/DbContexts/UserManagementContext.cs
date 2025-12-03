using Microsoft.EntityFrameworkCore;
using UserManagement.Entity;

namespace UserManagement.DbContexts
{
    public class UserManagementContext : DbContext
    {
        public UserManagementContext(DbContextOptions<UserManagementContext> options) : base(options)
        {
        }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Name = "Yetki Ekranı Listeleme Yetkisi", Code = "PermissionScene.Paging.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 2, Name = "Yetki Ekranı Kayıt Yetkisi", Code = "PermissionScene.Save.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 3, Name = "Yetki Ekranı Güncelleme Yetkisi", Code = "PermissionScene.Edit.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 4, Name = "Yetki Ekranı Silme Yetkisi", Code = "PermissionScene.Delete.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 5, Name = "Rol Ekranı Listeleme Yetkisi", Code = "RoleScene.Paging.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 6, Name = "Rol Ekranı Kayıt Yetkisi", Code = "RoleScene.Save.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 7, Name = "Rol Ekranı Güncelleme Yetkisi", Code = "RoleScene.Edit.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 8, Name = "Rol Ekranı Silme Yetkisi", Code = "RoleScene.Delete.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 9, Name = "Kullanıcı Ekranı Listeleme Yetkisi", Code = "UserScene.Paging.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 10, Name = "Kullanıcı Ekranı Kayıt Yetkisi", Code = "UserScene.Save.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 11, Name = "Kullanıcı Ekranı Güncelleme Yetkisi", Code = "UserScene.Edit.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 12, Name = "Kullanıcı Ekranı Silme Yetkisi", Code = "UserScene.Delete.Permission", IsDeleted = false , IsSystemData = true },
                new Permission { Id = 13, Name = "Dosya Ekranı Kayıt Yetkisi", Code = "FileScene.Save.Permission", IsDeleted = false, IsSystemData = true },
                new Permission { Id = 14, Name = "Dosya Ekranı Silme Yetkisi", Code = "FileScene.Delete.Permission", IsDeleted = false, IsSystemData = true }
            );


            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "SuperAdmin", IsDeleted = false, IsSystemData = true },
                new Role { Id = 2, Name = "Standart", IsDeleted = false, IsSystemData = true },
                new Role { Id = 3, Name = "Premium", IsDeleted = false, IsSystemData = true },
                new Role { Id = 4, Name = "Ultra", IsDeleted = false, IsSystemData = true }
            );

            modelBuilder.Entity<RolePermission>().HasData(
                //SuperAdmin Role Perms
                new RolePermission { Id = 1, RoleId = 1, PermissionId = 1, IsDeleted = false },
                new RolePermission { Id = 2, RoleId = 1, PermissionId = 2, IsDeleted = false },
                new RolePermission { Id = 3, RoleId = 1, PermissionId = 3, IsDeleted = false },
                new RolePermission { Id = 4, RoleId = 1, PermissionId = 4, IsDeleted = false },
                new RolePermission { Id = 5, RoleId = 1, PermissionId = 5, IsDeleted = false },
                new RolePermission { Id = 6, RoleId = 1, PermissionId = 6, IsDeleted = false },
                new RolePermission { Id = 7, RoleId = 1, PermissionId = 7, IsDeleted = false },
                new RolePermission { Id = 8, RoleId = 1, PermissionId = 8, IsDeleted = false },
                new RolePermission { Id = 9, RoleId = 1, PermissionId = 9, IsDeleted = false },
                new RolePermission { Id = 10, RoleId = 1, PermissionId = 10, IsDeleted = false },
                new RolePermission { Id = 11, RoleId = 1, PermissionId = 11, IsDeleted = false },
                new RolePermission { Id = 12, RoleId = 1, PermissionId = 12, IsDeleted = false },
                new RolePermission { Id = 13, RoleId = 1, PermissionId = 13, IsDeleted = false },
                new RolePermission { Id = 14, RoleId = 1, PermissionId = 14, IsDeleted = false },
                //Standart Role Perms
                new RolePermission { Id = 15, RoleId = 2, PermissionId = 13, IsDeleted = false },
                new RolePermission { Id = 16, RoleId = 2, PermissionId = 14, IsDeleted = false },
                //Premium Role Perms
                new RolePermission { Id = 17, RoleId = 3, PermissionId = 13, IsDeleted = false },
                new RolePermission { Id = 18, RoleId = 3, PermissionId = 14, IsDeleted = false },
                //Ultra Role Perms
                new RolePermission { Id = 19, RoleId = 4, PermissionId = 13, IsDeleted = false },
                new RolePermission { Id = 20, RoleId = 4, PermissionId = 14, IsDeleted = false }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "SuperAdmin", Surname = "SuperAdmin", Email = "super@test.com",  IsTrial = false, CreatedDate = DateTime.UtcNow, 
                        TrialExpirationDate = DateTime.UtcNow.AddDays(7), ExpirationDate = DateTime.UtcNow.AddYears(1),
                        Password = "DBD9DCE9DB51E56E1468B18F44233EB1FF625ADCECAAE2D7E9776BC714AF69D2A360B57CDB7C4E098C6225543BF83C50DAEC23A8DAADF9212BADF6F26760911C", 
                        Phone = "+905077352772", Username = "superadmin", Salt = Convert.FromBase64String("A/u2bAGlBV91ByotxKC+wkGpMDFjFnixpfY5ul7YO1Aw5dIfBa3bhlNJWsTc2KMO22o0tw36D4+a0FUtHTQNaQ=="), IsDeleted = false, IsSystemData = true },
                new User
                {
                    Id = 2,
                    Name = "Standart",
                    Surname = "User",
                    Email = "standart@test.com",
                    Password = "DBD9DCE9DB51E56E1468B18F44233EB1FF625ADCECAAE2D7E9776BC714AF69D2A360B57CDB7C4E098C6225543BF83C50DAEC23A8DAADF9212BADF6F26760911C",
                    Phone = "+905077352772",
                    Username = "standart user",
                    CreatedDate = DateTime.UtcNow,
                    TrialExpirationDate = DateTime.UtcNow.AddDays(7),
                    ExpirationDate = DateTime.UtcNow.AddYears(1),
                    Salt = Convert.FromBase64String("A/u2bAGlBV91ByotxKC+wkGpMDFjFnixpfY5ul7YO1Aw5dIfBa3bhlNJWsTc2KMO22o0tw36D4+a0FUtHTQNaQ=="),
                    IsDeleted = false,
                    IsSystemData = true
                },
                new User
                {
                    Id = 3,
                    Name = "Premium",
                    Surname = "User",
                    Email = "premium@test.com",
                    Password = "DBD9DCE9DB51E56E1468B18F44233EB1FF625ADCECAAE2D7E9776BC714AF69D2A360B57CDB7C4E098C6225543BF83C50DAEC23A8DAADF9212BADF6F26760911C",
                    Phone = "+905077352772",
                    Username = "premium user",
                    CreatedDate = DateTime.UtcNow,
                    TrialExpirationDate = DateTime.UtcNow.AddDays(7),
                    ExpirationDate = DateTime.UtcNow.AddYears(1),
                    Salt = Convert.FromBase64String("A/u2bAGlBV91ByotxKC+wkGpMDFjFnixpfY5ul7YO1Aw5dIfBa3bhlNJWsTc2KMO22o0tw36D4+a0FUtHTQNaQ=="),
                    IsDeleted = false,
                    IsSystemData = true
                },
                new User
                {
                    Id = 4,
                    Name = "Ultra",
                    Surname = "User",
                    Email = "ultra@test.com",
                    Password = "DBD9DCE9DB51E56E1468B18F44233EB1FF625ADCECAAE2D7E9776BC714AF69D2A360B57CDB7C4E098C6225543BF83C50DAEC23A8DAADF9212BADF6F26760911C",
                    Phone = "+905077352772",
                    Username = "ultra user",
                    CreatedDate = DateTime.UtcNow,
                    TrialExpirationDate = DateTime.UtcNow.AddDays(7),
                    ExpirationDate = DateTime.UtcNow.AddYears(1),
                    Salt = Convert.FromBase64String("A/u2bAGlBV91ByotxKC+wkGpMDFjFnixpfY5ul7YO1Aw5dIfBa3bhlNJWsTc2KMO22o0tw36D4+a0FUtHTQNaQ=="),
                    IsDeleted = false,
                    IsSystemData = true
                }
            );

            modelBuilder.Entity<UserRole>().HasData(
                //SuperAdmin User Role
                new UserRole
                {
                    Id = 1,
                    RoleId = 1,
                    UserId = 1,
                    IsDeleted = false
                },
                //Standart User Role
                new UserRole
                {
                    Id = 2,
                    RoleId = 2,
                    UserId = 2,
                    IsDeleted = false
                },
                //Premium User Role
                new UserRole
                {
                    Id = 3,
                    RoleId = 3,
                    UserId = 3,
                    IsDeleted = false
                },
                //Ultra User Role
                new UserRole
                {
                    Id = 4,
                    RoleId = 4,
                    UserId = 4,
                    IsDeleted = false
                }
            );

            modelBuilder.Entity<UserPermission>().HasData(
                //SuperAdmin User Permissions
                new UserPermission { Id = 1, UserId = 1, PermissionId = 1, IsDeleted = false },
                new UserPermission { Id = 2, UserId = 1, PermissionId = 2, IsDeleted = false },
                new UserPermission { Id = 3, UserId = 1, PermissionId = 3, IsDeleted = false },
                new UserPermission { Id = 4, UserId = 1, PermissionId = 4, IsDeleted = false },
                new UserPermission { Id = 5, UserId = 1, PermissionId = 5, IsDeleted = false },
                new UserPermission { Id = 6, UserId = 1, PermissionId = 6, IsDeleted = false },
                new UserPermission { Id = 7, UserId = 1, PermissionId = 7, IsDeleted = false },
                new UserPermission { Id = 8, UserId = 1, PermissionId = 8, IsDeleted = false },
                new UserPermission { Id = 9, UserId = 1, PermissionId = 9, IsDeleted = false },
                new UserPermission { Id = 10, UserId = 1, PermissionId = 10, IsDeleted = false },
                new UserPermission { Id = 11, UserId = 1, PermissionId = 11, IsDeleted = false },
                new UserPermission { Id = 12, UserId = 1, PermissionId = 12, IsDeleted = false },
                new UserPermission { Id = 13, UserId = 1, PermissionId = 13, IsDeleted = false },
                new UserPermission { Id = 14, UserId = 1, PermissionId = 14, IsDeleted = false },
                //Standart User Permissions
                new UserPermission { Id = 15, UserId = 2, PermissionId = 13, IsDeleted = false },
                new UserPermission { Id = 16, UserId = 2, PermissionId = 14, IsDeleted = false },
                //Premium User Permissions
                new UserPermission { Id = 17, UserId = 3, PermissionId = 13, IsDeleted = false },
                new UserPermission { Id = 18, UserId = 3, PermissionId = 14, IsDeleted = false },
                 //Ultra User Permissions
                new UserPermission { Id = 19, UserId = 4, PermissionId = 13, IsDeleted = false },
                new UserPermission { Id = 20, UserId = 4, PermissionId = 14, IsDeleted = false }
            );
        }

    }
}
