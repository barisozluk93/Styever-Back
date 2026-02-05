using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserManagement.Migrations
{
    /// <inheritdoc />
    public partial class NewPermissionAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code", "IsDeleted", "IsSystemData", "Name" },
                values: new object[] { 42L, "MemoryScene.ApproveComment.Permission", false, true, "Hatıra Ekranı Yorum Onaylama Yetkisi" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedDate", "ExpirationDate", "TrialExpirationDate" },
                values: new object[] { new DateTime(2026, 2, 3, 11, 7, 24, 345, DateTimeKind.Utc).AddTicks(773), new DateTime(2027, 2, 3, 11, 7, 24, 345, DateTimeKind.Utc).AddTicks(780), new DateTime(2026, 2, 10, 11, 7, 24, 345, DateTimeKind.Utc).AddTicks(774) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedDate", "ExpirationDate", "TrialExpirationDate" },
                values: new object[] { new DateTime(2026, 2, 3, 11, 7, 24, 345, DateTimeKind.Utc).AddTicks(810), new DateTime(2027, 2, 3, 11, 7, 24, 345, DateTimeKind.Utc).AddTicks(810), new DateTime(2026, 2, 10, 11, 7, 24, 345, DateTimeKind.Utc).AddTicks(810) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedDate", "ExpirationDate", "TrialExpirationDate" },
                values: new object[] { new DateTime(2026, 2, 3, 11, 7, 24, 345, DateTimeKind.Utc).AddTicks(817), new DateTime(2027, 2, 3, 11, 7, 24, 345, DateTimeKind.Utc).AddTicks(817), new DateTime(2026, 2, 10, 11, 7, 24, 345, DateTimeKind.Utc).AddTicks(817) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedDate", "ExpirationDate", "TrialExpirationDate" },
                values: new object[] { new DateTime(2026, 2, 3, 11, 7, 24, 345, DateTimeKind.Utc).AddTicks(855), new DateTime(2027, 2, 3, 11, 7, 24, 345, DateTimeKind.Utc).AddTicks(856), new DateTime(2026, 2, 10, 11, 7, 24, 345, DateTimeKind.Utc).AddTicks(856) });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "IsDeleted", "PermissionId", "RoleId" },
                values: new object[,]
                {
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
                    { 165L, false, 42L, 1L },
                    { 166L, false, 42L, 2L },
                    { 167L, false, 42L, 3L },
                    { 168L, false, 42L, 4L }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 165L);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 166L);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 167L);

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 168L);

            migrationBuilder.DeleteData(
                table: "UserPermissions",
                keyColumn: "Id",
                keyValue: 165L);

            migrationBuilder.DeleteData(
                table: "UserPermissions",
                keyColumn: "Id",
                keyValue: 166L);

            migrationBuilder.DeleteData(
                table: "UserPermissions",
                keyColumn: "Id",
                keyValue: 167L);

            migrationBuilder.DeleteData(
                table: "UserPermissions",
                keyColumn: "Id",
                keyValue: 168L);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 42L);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedDate", "ExpirationDate", "TrialExpirationDate" },
                values: new object[] { new DateTime(2026, 1, 25, 18, 12, 47, 583, DateTimeKind.Utc).AddTicks(6865), new DateTime(2027, 1, 25, 18, 12, 47, 583, DateTimeKind.Utc).AddTicks(6873), new DateTime(2026, 2, 1, 18, 12, 47, 583, DateTimeKind.Utc).AddTicks(6867) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedDate", "ExpirationDate", "TrialExpirationDate" },
                values: new object[] { new DateTime(2026, 1, 25, 18, 12, 47, 583, DateTimeKind.Utc).AddTicks(6898), new DateTime(2027, 1, 25, 18, 12, 47, 583, DateTimeKind.Utc).AddTicks(6899), new DateTime(2026, 2, 1, 18, 12, 47, 583, DateTimeKind.Utc).AddTicks(6898) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedDate", "ExpirationDate", "TrialExpirationDate" },
                values: new object[] { new DateTime(2026, 1, 25, 18, 12, 47, 583, DateTimeKind.Utc).AddTicks(6905), new DateTime(2027, 1, 25, 18, 12, 47, 583, DateTimeKind.Utc).AddTicks(6906), new DateTime(2026, 2, 1, 18, 12, 47, 583, DateTimeKind.Utc).AddTicks(6905) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedDate", "ExpirationDate", "TrialExpirationDate" },
                values: new object[] { new DateTime(2026, 1, 25, 18, 12, 47, 583, DateTimeKind.Utc).AddTicks(6909), new DateTime(2027, 1, 25, 18, 12, 47, 583, DateTimeKind.Utc).AddTicks(6910), new DateTime(2026, 2, 1, 18, 12, 47, 583, DateTimeKind.Utc).AddTicks(6909) });
        }
    }
}
