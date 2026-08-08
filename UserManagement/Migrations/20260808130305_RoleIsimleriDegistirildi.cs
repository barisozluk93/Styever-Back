using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagement.Migrations
{
    /// <inheritdoc />
    public partial class RoleIsimleriDegistirildi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Name",
                value: "Origin");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Name",
                value: "Heart");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4L,
                column: "Name",
                value: "Family");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedDate", "ExpirationDate", "TrialExpirationDate" },
                values: new object[] { new DateTime(2026, 8, 8, 13, 3, 4, 964, DateTimeKind.Utc).AddTicks(6842), new DateTime(2027, 8, 8, 13, 3, 4, 964, DateTimeKind.Utc).AddTicks(6849), new DateTime(2026, 8, 15, 13, 3, 4, 964, DateTimeKind.Utc).AddTicks(6844) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedDate", "Email", "ExpirationDate", "Name", "TrialExpirationDate", "Username" },
                values: new object[] { new DateTime(2026, 8, 8, 13, 3, 4, 964, DateTimeKind.Utc).AddTicks(6882), "origin@test.com", new DateTime(2027, 8, 8, 13, 3, 4, 964, DateTimeKind.Utc).AddTicks(6883), "Origin", new DateTime(2026, 8, 15, 13, 3, 4, 964, DateTimeKind.Utc).AddTicks(6883), "originuser" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedDate", "Email", "ExpirationDate", "Name", "TrialExpirationDate", "Username" },
                values: new object[] { new DateTime(2026, 8, 8, 13, 3, 4, 964, DateTimeKind.Utc).AddTicks(6889), "heart@test.com", new DateTime(2027, 8, 8, 13, 3, 4, 964, DateTimeKind.Utc).AddTicks(6890), "Heart", new DateTime(2026, 8, 15, 13, 3, 4, 964, DateTimeKind.Utc).AddTicks(6890), "heartuser" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedDate", "Email", "ExpirationDate", "Name", "TrialExpirationDate", "Username" },
                values: new object[] { new DateTime(2026, 8, 8, 13, 3, 4, 964, DateTimeKind.Utc).AddTicks(6894), "family@test.com", new DateTime(2027, 8, 8, 13, 3, 4, 964, DateTimeKind.Utc).AddTicks(6894), "Family", new DateTime(2026, 8, 15, 13, 3, 4, 964, DateTimeKind.Utc).AddTicks(6894), "familyuser" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Name",
                value: "Memory");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Name",
                value: "Tribute");

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4L,
                column: "Name",
                value: "Eternal");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedDate", "ExpirationDate", "TrialExpirationDate" },
                values: new object[] { new DateTime(2026, 8, 6, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(32), new DateTime(2027, 8, 6, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(40), new DateTime(2026, 8, 13, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(35) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedDate", "Email", "ExpirationDate", "Name", "TrialExpirationDate", "Username" },
                values: new object[] { new DateTime(2026, 8, 6, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(65), "memory@test.com", new DateTime(2027, 8, 6, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(66), "Memory", new DateTime(2026, 8, 13, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(65), "memoryuser" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedDate", "Email", "ExpirationDate", "Name", "TrialExpirationDate", "Username" },
                values: new object[] { new DateTime(2026, 8, 6, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(72), "tribute@test.com", new DateTime(2027, 8, 6, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(73), "Tribute", new DateTime(2026, 8, 13, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(72), "tributeuser" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedDate", "Email", "ExpirationDate", "Name", "TrialExpirationDate", "Username" },
                values: new object[] { new DateTime(2026, 8, 6, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(77), "eternal@test.com", new DateTime(2027, 8, 6, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(77), "Eternal", new DateTime(2026, 8, 13, 20, 55, 11, 855, DateTimeKind.Utc).AddTicks(77), "eternaluser" });
        }
    }
}
