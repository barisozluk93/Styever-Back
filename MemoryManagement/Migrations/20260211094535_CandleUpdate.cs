using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemoryManagement.Migrations
{
    /// <inheritdoc />
    public partial class CandleUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "MemoryCandles",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "NameSurname",
                table: "MemoryCandles",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameSurname",
                table: "MemoryCandles");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "MemoryCandles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
