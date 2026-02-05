using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemoryManagement.Migrations
{
    /// <inheritdoc />
    public partial class CommentEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "MemoryComments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "MemoryComments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NameSurname",
                table: "MemoryComments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "MemoryComments");

            migrationBuilder.DropColumn(
                name: "NameSurname",
                table: "MemoryComments");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "MemoryComments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
