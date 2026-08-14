using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArticleManagement.Migrations
{
    /// <inheritdoc />
    public partial class FileIdsChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "FileId",
                value: 4L);

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "FileId",
                value: 5L);

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 3L,
                column: "FileId",
                value: 6L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 1L,
                column: "FileId",
                value: 7L);

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "FileId",
                value: 8L);

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Id",
                keyValue: 3L,
                column: "FileId",
                value: 9L);
        }
    }
}
