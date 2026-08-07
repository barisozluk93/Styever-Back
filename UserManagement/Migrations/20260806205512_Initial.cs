using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserManagement.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsSystemData = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsSystemData = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShopierPayments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Reference = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    PlanId = table.Column<long>(type: "bigint", nullable: false),
                    MemoryId = table.Column<long>(type: "bigint", nullable: false),
                    PurchaseType = table.Column<string>(type: "text", nullable: false),
                    ProductId = table.Column<string>(type: "text", nullable: false),
                    ProductUrl = table.Column<string>(type: "text", nullable: false),
                    BuyerEmail = table.Column<string>(type: "text", nullable: false),
                    GiftPayload = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ShopierOrderId = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopierPayments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAgreementAcceptances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AgreementType = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Version = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Context = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    DocumentUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ContentSnapshot = table.Column<string>(type: "text", nullable: true),
                    RelatedReference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AcceptedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAgreementAcceptances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Salt = table.Column<byte[]>(type: "bytea", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsSystemData = table.Column<bool>(type: "boolean", nullable: false),
                    FileId = table.Column<long>(type: "bigint", nullable: true),
                    IsTrial = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TrialExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    PermissionId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: false),
                    RefreshTokenExpireDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAddresses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Country = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    District = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    AddressHeader = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAddresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPayments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlanId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPayments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PermissionId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserVouchers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlanId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    SenderEmail = table.Column<string>(type: "text", nullable: true),
                    ReceiverEmail = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Voucher = table.Column<Guid>(type: "uuid", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVouchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserVouchers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code", "IsDeleted", "IsSystemData", "Name" },
                values: new object[,]
                {
                    { 1L, "PermissionScene.Paging.Permission", false, true, "Yetki Ekranı Sayfalama Yetkisi" },
                    { 2L, "PermissionScene.Save.Permission", false, true, "Yetki Ekranı Kayıt Yetkisi" },
                    { 3L, "PermissionScene.Edit.Permission", false, true, "Yetki Ekranı Güncelleme Yetkisi" },
                    { 4L, "PermissionScene.Delete.Permission", false, true, "Yetki Ekranı Silme Yetkisi" },
                    { 5L, "PermissionScene.List.Permission", false, true, "Yetki Ekranı Listeleme Yetkisi" },
                    { 6L, "PermissionScene.GetById.Permission", false, true, "Yetki Ekranı Yetki Alma Yetkisi" },
                    { 7L, "RoleScene.Paging.Permission", false, true, "Rol Ekranı Sayfalama Yetkisi" },
                    { 8L, "RoleScene.Save.Permission", false, true, "Rol Ekranı Kayıt Yetkisi" },
                    { 9L, "RoleScene.Edit.Permission", false, true, "Rol Ekranı Güncelleme Yetkisi" },
                    { 10L, "RoleScene.Delete.Permission", false, true, "Rol Ekranı Silme Yetkisi" },
                    { 11L, "RoleScene.List.Permission", false, true, "Rol Ekranı Listeleme Yetkisi" },
                    { 12L, "RoleScene.GetById.Permission", false, true, "Rol Ekranı Rol Alma Yetkisi" },
                    { 13L, "UserScene.Paging.Permission", false, true, "Kullanıcı Ekranı Sayfalama Yetkisi" },
                    { 14L, "UserScene.Save.Permission", false, true, "Kullanıcı Ekranı Kayıt Yetkisi" },
                    { 15L, "UserScene.Edit.Permission", false, true, "Kullanıcı Ekranı Güncelleme Yetkisi" },
                    { 16L, "UserScene.Delete.Permission", false, true, "Kullanıcı Ekranı Silme Yetkisi" },
                    { 17L, "UserScene.List.Permission", false, true, "Kullanıcı Ekranı Listeleme Yetkisi" },
                    { 18L, "ProfileScene.ChangePw.Permission", false, true, "Profil Ekranı Şifre Değiştirme Yetkisi" },
                    { 19L, "ProfileScene.Edit.Permission", false, true, "Profil Ekranı Güncelleme Yetkisi" },
                    { 20L, "ProfileScene.AvatarEdit.Permission", false, true, "Profil Ekranı Avatar Güncelleme Yetkisi" },
                    { 21L, "ProfileScene.ListAddress.Permission", false, true, "Profil Ekranı Adres Listeleme Yetkisi" },
                    { 22L, "ProfileScene.SaveAddress.Permission", false, true, "Profil Ekranı Adres Kayıt Yetkisi" },
                    { 23L, "ProfileScene.EditAddress.Permission", false, true, "Profil Ekranı Adres Güncelleme Yetkisi" },
                    { 24L, "ProfileScene.DeletAddress.Permission", false, true, "Profil Ekranı Adres Silme Yetkisi" },
                    { 25L, "ProfileScene.GetAddressById.Permission", false, true, "Profil Ekranı Adres Alma Yetkisi" },
                    { 26L, "PaymentScene.MembershipPayment.Permission", false, true, "Ödeme Ekranı Üyelik Ödeme Yetkisi" },
                    { 27L, "PaymentScene.BuyMembership.Permission", false, true, "Ödeme Ekranı Üyelik Satın Alma Yetkisi" },
                    { 28L, "File.Save.Permission", false, true, "Dosya Yükleme Yetkisi" },
                    { 29L, "File.Delete.Permission", false, true, "Dosya Silme Yetkisi" },
                    { 30L, "MemoryScene.Save.Permission", false, true, "Hatıra Ekranı Kayıt Yetkisi" },
                    { 31L, "MemoryScene.Edit.Permission", false, true, "Hatıra Ekranı Güncelleme Yetkisi" },
                    { 32L, "MemoryScene.Count.Permission", false, true, "Hatıra Ekranı Sayaç Yetkisi" },
                    { 33L, "MemoryScene.FileUpdate.Permission", false, true, "Hatıra Ekranı Dosya Güncelleme Yetkisi" },
                    { 34L, "MemoryScene.FileAdd.Permission", false, true, "Hatıra Ekranı Dosya Ekleme Yetkisi" },
                    { 35L, "MemoryScene.FileDelete.Permission", false, true, "Hatıra Ekranı Dosya Silme Yetkisi" },
                    { 36L, "MemoryScene.LightCandle.Permission", false, true, "Hatıra Ekranı Mum Yakma Yetkisi" },
                    { 37L, "MemoryScene.UpdateCandle.Permission", false, true, "Hatıra Ekranı Mum Yakma Güncelleme Yetkisi" },
                    { 38L, "MemoryScene.AddComment.Permission", false, true, "Hatıra Ekranı Yorum Yapma Yetkisi" },
                    { 39L, "MemoryScene.DeleteComment.Permission", false, true, "Hatıra Ekranı Yorum Silme Yetkisi" },
                    { 40L, "MemoryScene.Like.Permission", false, true, "Hatıra Ekranı Beğeni Yetkisi" },
                    { 41L, "MemoryScene.Dislike.Permission", false, true, "Hatıra Ekranı Beğeni Silme Yetkisi" },
                    { 42L, "MemoryScene.ApproveComment.Permission", false, true, "Hatıra Ekranı Yorum Onaylama Yetkisi" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "IsDeleted", "IsSystemData", "Name" },
                values: new object[,]
                {
                    { 1L, false, true, "SuperAdmin" },
                    { 2L, false, true, "Memory" },
                    { 3L, false, true, "Tribute" },
                    { 4L, false, true, "Eternal" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedDate", "Email", "ExpirationDate", "FileId", "IsActive", "IsDeleted", "IsSystemData", "IsTrial", "Name", "Password", "Phone", "Salt", "Surname", "TrialExpirationDate", "Username" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 8, 6, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(32), "super@test.com", new DateTime(2027, 8, 6, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(40), null, true, false, true, false, "SuperAdmin", "DBD9DCE9DB51E56E1468B18F44233EB1FF625ADCECAAE2D7E9776BC714AF69D2A360B57CDB7C4E098C6225543BF83C50DAEC23A8DAADF9212BADF6F26760911C", "+905077352772", new byte[] { 3, 251, 182, 108, 1, 165, 5, 95, 117, 7, 42, 45, 196, 160, 190, 194, 65, 169, 48, 49, 99, 22, 120, 177, 165, 246, 57, 186, 94, 216, 59, 80, 48, 229, 210, 31, 5, 173, 219, 134, 83, 73, 90, 196, 220, 216, 163, 14, 219, 106, 52, 183, 13, 250, 15, 143, 154, 208, 85, 45, 29, 52, 13, 105 }, "SuperAdmin", new DateTime(2026, 8, 13, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(35), "superadmin" },
                    { 2L, new DateTime(2026, 8, 6, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(65), "memory@test.com", new DateTime(2027, 8, 6, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(66), 1L, true, false, true, false, "Memory", "DBD9DCE9DB51E56E1468B18F44233EB1FF625ADCECAAE2D7E9776BC714AF69D2A360B57CDB7C4E098C6225543BF83C50DAEC23A8DAADF9212BADF6F26760911C", "+905077352772", new byte[] { 3, 251, 182, 108, 1, 165, 5, 95, 117, 7, 42, 45, 196, 160, 190, 194, 65, 169, 48, 49, 99, 22, 120, 177, 165, 246, 57, 186, 94, 216, 59, 80, 48, 229, 210, 31, 5, 173, 219, 134, 83, 73, 90, 196, 220, 216, 163, 14, 219, 106, 52, 183, 13, 250, 15, 143, 154, 208, 85, 45, 29, 52, 13, 105 }, "User", new DateTime(2026, 8, 13, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(65), "memoryuser" },
                    { 3L, new DateTime(2026, 8, 6, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(72), "tribute@test.com", new DateTime(2027, 8, 6, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(73), 2L, true, false, true, false, "Tribute", "DBD9DCE9DB51E56E1468B18F44233EB1FF625ADCECAAE2D7E9776BC714AF69D2A360B57CDB7C4E098C6225543BF83C50DAEC23A8DAADF9212BADF6F26760911C", "+905077352772", new byte[] { 3, 251, 182, 108, 1, 165, 5, 95, 117, 7, 42, 45, 196, 160, 190, 194, 65, 169, 48, 49, 99, 22, 120, 177, 165, 246, 57, 186, 94, 216, 59, 80, 48, 229, 210, 31, 5, 173, 219, 134, 83, 73, 90, 196, 220, 216, 163, 14, 219, 106, 52, 183, 13, 250, 15, 143, 154, 208, 85, 45, 29, 52, 13, 105 }, "User", new DateTime(2026, 8, 13, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(72), "tributeuser" },
                    { 4L, new DateTime(2026, 8, 6, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(77), "eternal@test.com", new DateTime(2027, 8, 6, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(77), 3L, true, false, true, false, "Eternal", "DBD9DCE9DB51E56E1468B18F44233EB1FF625ADCECAAE2D7E9776BC714AF69D2A360B57CDB7C4E098C6225543BF83C50DAEC23A8DAADF9212BADF6F26760911C", "+905077352772", new byte[] { 3, 251, 182, 108, 1, 165, 5, 95, 117, 7, 42, 45, 196, 160, 190, 194, 65, 169, 48, 49, 99, 22, 120, 177, 165, 246, 57, 186, 94, 216, 59, 80, 48, 229, 210, 31, 5, 173, 219, 134, 83, 73, 90, 196, 220, 216, 163, 14, 219, 106, 52, 183, 13, 250, 15, 143, 154, 208, 85, 45, 29, 52, 13, 105 }, "User", new DateTime(2026, 8, 13, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(77), "eternaluser" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "IsDeleted", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 1L, false, 1L, 1L },
                    { 2L, false, 2L, 1L },
                    { 3L, false, 3L, 1L },
                    { 4L, false, 4L, 1L },
                    { 5L, false, 5L, 1L },
                    { 6L, false, 6L, 1L },
                    { 7L, false, 7L, 1L },
                    { 8L, false, 8L, 1L },
                    { 9L, false, 9L, 1L },
                    { 10L, false, 10L, 1L },
                    { 11L, false, 11L, 1L },
                    { 12L, false, 12L, 1L },
                    { 13L, false, 13L, 1L },
                    { 14L, false, 14L, 1L },
                    { 15L, false, 15L, 1L },
                    { 16L, false, 16L, 1L },
                    { 17L, false, 17L, 1L },
                    { 18L, false, 18L, 1L },
                    { 19L, false, 19L, 1L },
                    { 20L, false, 20L, 1L },
                    { 21L, false, 21L, 1L },
                    { 22L, false, 22L, 1L },
                    { 23L, false, 23L, 1L },
                    { 24L, false, 24L, 1L },
                    { 25L, false, 25L, 1L },
                    { 26L, false, 26L, 1L },
                    { 27L, false, 27L, 1L },
                    { 28L, false, 28L, 1L },
                    { 29L, false, 29L, 1L },
                    { 30L, false, 30L, 1L },
                    { 31L, false, 31L, 1L },
                    { 32L, false, 32L, 1L },
                    { 33L, false, 33L, 1L },
                    { 34L, false, 34L, 1L },
                    { 35L, false, 35L, 1L },
                    { 36L, false, 36L, 1L },
                    { 37L, false, 37L, 1L },
                    { 38L, false, 38L, 1L },
                    { 39L, false, 39L, 1L },
                    { 40L, false, 40L, 1L },
                    { 41L, false, 41L, 1L },
                    { 42L, false, 1L, 2L },
                    { 43L, false, 2L, 2L },
                    { 44L, false, 3L, 2L },
                    { 45L, false, 4L, 2L },
                    { 46L, false, 5L, 2L },
                    { 47L, false, 6L, 2L },
                    { 48L, false, 7L, 2L },
                    { 49L, false, 8L, 2L },
                    { 50L, false, 9L, 2L },
                    { 51L, false, 10L, 2L },
                    { 52L, false, 11L, 2L },
                    { 53L, false, 12L, 2L },
                    { 54L, false, 13L, 2L },
                    { 55L, false, 14L, 2L },
                    { 56L, false, 15L, 2L },
                    { 57L, false, 16L, 2L },
                    { 58L, false, 17L, 2L },
                    { 59L, false, 18L, 2L },
                    { 60L, false, 19L, 2L },
                    { 61L, false, 20L, 2L },
                    { 62L, false, 21L, 2L },
                    { 63L, false, 22L, 2L },
                    { 64L, false, 23L, 2L },
                    { 65L, false, 24L, 2L },
                    { 66L, false, 25L, 2L },
                    { 67L, false, 26L, 2L },
                    { 68L, false, 27L, 2L },
                    { 69L, false, 28L, 2L },
                    { 70L, false, 29L, 2L },
                    { 71L, false, 30L, 2L },
                    { 72L, false, 31L, 2L },
                    { 73L, false, 32L, 2L },
                    { 74L, false, 33L, 2L },
                    { 75L, false, 34L, 2L },
                    { 76L, false, 35L, 2L },
                    { 77L, false, 36L, 2L },
                    { 78L, false, 37L, 2L },
                    { 79L, false, 38L, 2L },
                    { 80L, false, 39L, 2L },
                    { 81L, false, 40L, 2L },
                    { 82L, false, 41L, 2L },
                    { 83L, false, 1L, 3L },
                    { 84L, false, 2L, 3L },
                    { 85L, false, 3L, 3L },
                    { 86L, false, 4L, 3L },
                    { 87L, false, 5L, 3L },
                    { 88L, false, 6L, 3L },
                    { 89L, false, 7L, 3L },
                    { 90L, false, 8L, 3L },
                    { 91L, false, 9L, 3L },
                    { 92L, false, 10L, 3L },
                    { 93L, false, 11L, 3L },
                    { 94L, false, 12L, 3L },
                    { 95L, false, 13L, 3L },
                    { 96L, false, 14L, 3L },
                    { 97L, false, 15L, 3L },
                    { 98L, false, 16L, 3L },
                    { 99L, false, 17L, 3L },
                    { 100L, false, 18L, 3L },
                    { 101L, false, 19L, 3L },
                    { 102L, false, 20L, 3L },
                    { 103L, false, 21L, 3L },
                    { 104L, false, 22L, 3L },
                    { 105L, false, 23L, 3L },
                    { 106L, false, 24L, 3L },
                    { 107L, false, 25L, 3L },
                    { 108L, false, 26L, 3L },
                    { 109L, false, 27L, 3L },
                    { 110L, false, 28L, 3L },
                    { 111L, false, 29L, 3L },
                    { 112L, false, 30L, 3L },
                    { 113L, false, 31L, 3L },
                    { 114L, false, 32L, 3L },
                    { 115L, false, 33L, 3L },
                    { 116L, false, 34L, 3L },
                    { 117L, false, 35L, 3L },
                    { 118L, false, 36L, 3L },
                    { 119L, false, 37L, 3L },
                    { 120L, false, 38L, 3L },
                    { 121L, false, 39L, 3L },
                    { 122L, false, 40L, 3L },
                    { 123L, false, 41L, 3L },
                    { 124L, false, 1L, 4L },
                    { 125L, false, 2L, 4L },
                    { 126L, false, 3L, 4L },
                    { 127L, false, 4L, 4L },
                    { 128L, false, 5L, 4L },
                    { 129L, false, 6L, 4L },
                    { 130L, false, 7L, 4L },
                    { 131L, false, 8L, 4L },
                    { 132L, false, 9L, 4L },
                    { 133L, false, 10L, 4L },
                    { 134L, false, 11L, 4L },
                    { 135L, false, 12L, 4L },
                    { 136L, false, 13L, 4L },
                    { 137L, false, 14L, 4L },
                    { 138L, false, 15L, 4L },
                    { 139L, false, 16L, 4L },
                    { 140L, false, 17L, 4L },
                    { 141L, false, 18L, 4L },
                    { 142L, false, 19L, 4L },
                    { 143L, false, 20L, 4L },
                    { 144L, false, 21L, 4L },
                    { 145L, false, 22L, 4L },
                    { 146L, false, 23L, 4L },
                    { 147L, false, 24L, 4L },
                    { 148L, false, 25L, 4L },
                    { 149L, false, 26L, 4L },
                    { 150L, false, 27L, 4L },
                    { 151L, false, 28L, 4L },
                    { 152L, false, 29L, 4L },
                    { 153L, false, 30L, 4L },
                    { 154L, false, 31L, 4L },
                    { 155L, false, 32L, 4L },
                    { 156L, false, 33L, 4L },
                    { 157L, false, 34L, 4L },
                    { 158L, false, 35L, 4L },
                    { 159L, false, 36L, 4L },
                    { 160L, false, 37L, 4L },
                    { 161L, false, 38L, 4L },
                    { 162L, false, 39L, 4L },
                    { 163L, false, 40L, 4L },
                    { 164L, false, 41L, 4L },
                    { 165L, false, 42L, 1L },
                    { 166L, false, 42L, 2L },
                    { 167L, false, 42L, 3L },
                    { 168L, false, 42L, 4L }
                });

            migrationBuilder.InsertData(
                table: "UserPermissions",
                columns: new[] { "Id", "IsDeleted", "PermissionId", "UserId" },
                values: new object[,]
                {
                    { 1L, false, 1L, 1L },
                    { 2L, false, 2L, 1L },
                    { 3L, false, 3L, 1L },
                    { 4L, false, 4L, 1L },
                    { 5L, false, 5L, 1L },
                    { 6L, false, 6L, 1L },
                    { 7L, false, 7L, 1L },
                    { 8L, false, 8L, 1L },
                    { 9L, false, 9L, 1L },
                    { 10L, false, 10L, 1L },
                    { 11L, false, 11L, 1L },
                    { 12L, false, 12L, 1L },
                    { 13L, false, 13L, 1L },
                    { 14L, false, 14L, 1L },
                    { 15L, false, 15L, 1L },
                    { 16L, false, 16L, 1L },
                    { 17L, false, 17L, 1L },
                    { 18L, false, 18L, 1L },
                    { 19L, false, 19L, 1L },
                    { 20L, false, 20L, 1L },
                    { 21L, false, 21L, 1L },
                    { 22L, false, 22L, 1L },
                    { 23L, false, 23L, 1L },
                    { 24L, false, 24L, 1L },
                    { 25L, false, 25L, 1L },
                    { 26L, false, 26L, 1L },
                    { 27L, false, 27L, 1L },
                    { 28L, false, 28L, 1L },
                    { 29L, false, 29L, 1L },
                    { 30L, false, 30L, 1L },
                    { 31L, false, 31L, 1L },
                    { 32L, false, 32L, 1L },
                    { 33L, false, 33L, 1L },
                    { 34L, false, 34L, 1L },
                    { 35L, false, 35L, 1L },
                    { 36L, false, 36L, 1L },
                    { 37L, false, 37L, 1L },
                    { 38L, false, 38L, 1L },
                    { 39L, false, 39L, 1L },
                    { 40L, false, 40L, 1L },
                    { 41L, false, 41L, 1L },
                    { 42L, false, 1L, 2L },
                    { 43L, false, 2L, 2L },
                    { 44L, false, 3L, 2L },
                    { 45L, false, 4L, 2L },
                    { 46L, false, 5L, 2L },
                    { 47L, false, 6L, 2L },
                    { 48L, false, 7L, 2L },
                    { 49L, false, 8L, 2L },
                    { 50L, false, 9L, 2L },
                    { 51L, false, 10L, 2L },
                    { 52L, false, 11L, 2L },
                    { 53L, false, 12L, 2L },
                    { 54L, false, 13L, 2L },
                    { 55L, false, 14L, 2L },
                    { 56L, false, 15L, 2L },
                    { 57L, false, 16L, 2L },
                    { 58L, false, 17L, 2L },
                    { 59L, false, 18L, 2L },
                    { 60L, false, 19L, 2L },
                    { 61L, false, 20L, 2L },
                    { 62L, false, 21L, 2L },
                    { 63L, false, 22L, 2L },
                    { 64L, false, 23L, 2L },
                    { 65L, false, 24L, 2L },
                    { 66L, false, 25L, 2L },
                    { 67L, false, 26L, 2L },
                    { 68L, false, 27L, 2L },
                    { 69L, false, 28L, 2L },
                    { 70L, false, 29L, 2L },
                    { 71L, false, 30L, 2L },
                    { 72L, false, 31L, 2L },
                    { 73L, false, 32L, 2L },
                    { 74L, false, 33L, 2L },
                    { 75L, false, 34L, 2L },
                    { 76L, false, 35L, 2L },
                    { 77L, false, 36L, 2L },
                    { 78L, false, 37L, 2L },
                    { 79L, false, 38L, 2L },
                    { 80L, false, 39L, 2L },
                    { 81L, false, 40L, 2L },
                    { 82L, false, 41L, 2L },
                    { 83L, false, 1L, 3L },
                    { 84L, false, 2L, 3L },
                    { 85L, false, 3L, 3L },
                    { 86L, false, 4L, 3L },
                    { 87L, false, 5L, 3L },
                    { 88L, false, 6L, 3L },
                    { 89L, false, 7L, 3L },
                    { 90L, false, 8L, 3L },
                    { 91L, false, 9L, 3L },
                    { 92L, false, 10L, 3L },
                    { 93L, false, 11L, 3L },
                    { 94L, false, 12L, 3L },
                    { 95L, false, 13L, 3L },
                    { 96L, false, 14L, 3L },
                    { 97L, false, 15L, 3L },
                    { 98L, false, 16L, 3L },
                    { 99L, false, 17L, 3L },
                    { 100L, false, 18L, 3L },
                    { 101L, false, 19L, 3L },
                    { 102L, false, 20L, 3L },
                    { 103L, false, 21L, 3L },
                    { 104L, false, 22L, 3L },
                    { 105L, false, 23L, 3L },
                    { 106L, false, 24L, 3L },
                    { 107L, false, 25L, 3L },
                    { 108L, false, 26L, 3L },
                    { 109L, false, 27L, 3L },
                    { 110L, false, 28L, 3L },
                    { 111L, false, 29L, 3L },
                    { 112L, false, 30L, 3L },
                    { 113L, false, 31L, 3L },
                    { 114L, false, 32L, 3L },
                    { 115L, false, 33L, 3L },
                    { 116L, false, 34L, 3L },
                    { 117L, false, 35L, 3L },
                    { 118L, false, 36L, 3L },
                    { 119L, false, 37L, 3L },
                    { 120L, false, 38L, 3L },
                    { 121L, false, 39L, 3L },
                    { 122L, false, 40L, 3L },
                    { 123L, false, 41L, 3L },
                    { 124L, false, 1L, 4L },
                    { 125L, false, 2L, 4L },
                    { 126L, false, 3L, 4L },
                    { 127L, false, 4L, 4L },
                    { 128L, false, 5L, 4L },
                    { 129L, false, 6L, 4L },
                    { 130L, false, 7L, 4L },
                    { 131L, false, 8L, 4L },
                    { 132L, false, 9L, 4L },
                    { 133L, false, 10L, 4L },
                    { 134L, false, 11L, 4L },
                    { 135L, false, 12L, 4L },
                    { 136L, false, 13L, 4L },
                    { 137L, false, 14L, 4L },
                    { 138L, false, 15L, 4L },
                    { 139L, false, 16L, 4L },
                    { 140L, false, 17L, 4L },
                    { 141L, false, 18L, 4L },
                    { 142L, false, 19L, 4L },
                    { 143L, false, 20L, 4L },
                    { 144L, false, 21L, 4L },
                    { 145L, false, 22L, 4L },
                    { 146L, false, 23L, 4L },
                    { 147L, false, 24L, 4L },
                    { 148L, false, 25L, 4L },
                    { 149L, false, 26L, 4L },
                    { 150L, false, 27L, 4L },
                    { 151L, false, 28L, 4L },
                    { 152L, false, 29L, 4L },
                    { 153L, false, 30L, 4L },
                    { 154L, false, 31L, 4L },
                    { 155L, false, 32L, 4L },
                    { 156L, false, 33L, 4L },
                    { 157L, false, 34L, 4L },
                    { 158L, false, 35L, 4L },
                    { 159L, false, 36L, 4L },
                    { 160L, false, 37L, 4L },
                    { 161L, false, 38L, 4L },
                    { 162L, false, 39L, 4L },
                    { 163L, false, 40L, 4L },
                    { 164L, false, 41L, 4L },
                    { 165L, false, 42L, 1L },
                    { 166L, false, 42L, 2L },
                    { 167L, false, 42L, 3L },
                    { 168L, false, 42L, 4L }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "IsDeleted", "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1L, false, 1L, 1L },
                    { 2L, false, 2L, 2L },
                    { 3L, false, 3L, 3L },
                    { 4L, false, 4L, 4L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_UserId",
                table: "ApplicationUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopierPayments_Reference",
                table: "ShopierPayments",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShopierPayments_ShopierOrderId",
                table: "ShopierPayments",
                column: "ShopierOrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_UserId",
                table: "UserAddresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAgreementAcceptances_UserId_AcceptedDate",
                table: "UserAgreementAcceptances",
                columns: new[] { "UserId", "AcceptedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPayments_UserId",
                table: "UserPayments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_PermissionId",
                table: "UserPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserId",
                table: "UserPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserVouchers_UserId",
                table: "UserVouchers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUsers");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "ShopierPayments");

            migrationBuilder.DropTable(
                name: "UserAddresses");

            migrationBuilder.DropTable(
                name: "UserAgreementAcceptances");

            migrationBuilder.DropTable(
                name: "UserPayments");

            migrationBuilder.DropTable(
                name: "UserPermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserVouchers");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
