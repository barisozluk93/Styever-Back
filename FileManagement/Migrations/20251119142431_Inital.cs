using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FileManagement.Migrations
{
    /// <inheritdoc />
    public partial class Inital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    Extension = table.Column<string>(type: "text", nullable: false),
                    Length = table.Column<decimal>(type: "numeric", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Files",
                columns: new[] { "Id", "ContentType", "Extension", "IsDeleted", "Length", "Name", "Path" },
                values: new object[,]
                {
                    { 1L, "image/jpeg", ".jpeg", false, 52501m, "Queenie", "C:\\Users\\Asus\\Desktop\\RIPMyFriend-Backend\\FileManagement\\Uploads/Memories\\Queenie.jpeg" },
                    { 2L, "image/jpeg", ".jpeg", false, 12481m, "Ringo", "C:\\Users\\Asus\\Desktop\\RIPMyFriend-Backend\\FileManagement\\Uploads/Memories\\Ringo.jpeg" },
                    { 3L, "image/jpeg", ".jpeg", false, 118784m, "Bubbles", "C:\\Users\\Asus\\Desktop\\RIPMyFriend-Backend\\FileManagement\\Uploads/Memories\\Bubbles.jpeg" },
                    { 4L, "image/jpeg", ".jpeg", false, 276500m, "George", "C:\\Users\\Asus\\Desktop\\RIPMyFriend-Backend\\FileManagement\\Uploads/Memories\\George.jpeg" },
                    { 5L, "image/jpeg", ".jpeg", false, 542177m, "George(1)", "C:\\Users\\Asus\\Desktop\\RIPMyFriend-Backend\\FileManagement\\Uploads/Memories\\George(1).jpeg" },
                    { 6L, "image/jpeg", ".jpeg", false, 270336m, "George(2)", "C:\\Users\\Asus\\Desktop\\RIPMyFriend-Backend\\FileManagement\\Uploads/Memories\\George(2).jpeg" },
                    { 7L, "image/jpg", ".jpg", false, 270336m, "68a1a7ccdfe3241d0aa9f9ae_REF22", "C:\\Users\\Asus\\Desktop\\RIPMyFriend-Backend\\FileManagement\\Uploads/Articles\\68a1a7ccdfe3241d0aa9f9ae_REF22.jpg" },
                    { 8L, "image/jpg", ".jpg", false, 270336m, "68a1a8006928f76bab0bf47d_REF19", "C:\\Users\\Asus\\Desktop\\RIPMyFriend-Backend\\FileManagement\\Uploads/Articles\\68a1a8006928f76bab0bf47d_REF19.jpg" },
                    { 9L, "image/jpg", ".jpg", false, 270336m, "68a1a990b816e33cfbd857ea_REF5", "C:\\Users\\Asus\\Desktop\\RIPMyFriend-Backend\\FileManagement\\Uploads/Articles\\68a1a990b816e33cfbd857ea_REF5.jpg" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");
        }
    }
}
