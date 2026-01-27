using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemoryManagement.Migrations
{
    /// <inheritdoc />
    public partial class LinkOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLinkOnly",
                table: "Memories",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLinkOnly",
                table: "Memories");
        }
    }
}
