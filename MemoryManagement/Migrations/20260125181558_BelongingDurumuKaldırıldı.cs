using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemoryManagement.Migrations
{
    /// <inheritdoc />
    public partial class BelongingDurumuKaldırıldı : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BelongToOldPackage",
                table: "Memories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BelongToOldPackage",
                table: "Memories",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
