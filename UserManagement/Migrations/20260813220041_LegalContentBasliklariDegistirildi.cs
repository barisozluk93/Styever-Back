using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagement.Migrations
{
    /// <inheritdoc />
    public partial class LegalContentBasliklariDegistirildi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "LegalContents",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Title", "TitleEn" },
                values: new object[] { "Mesafeli Satış Sözleşmesi", "Distance Sales Agreement" });

            migrationBuilder.UpdateData(
                table: "LegalContents",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "Title", "TitleEn" },
                values: new object[] { "Çerez Politikası", "Cookie Policy" });

            migrationBuilder.UpdateData(
                table: "LegalContents",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "Title", "TitleEn" },
                values: new object[] { "Yasal Uyarı ve Sorumluluk Reddi Beyanı", "Legal Notice and Disclaimer" });

            migrationBuilder.UpdateData(
                table: "LegalContents",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "Title", "TitleEn" },
                values: new object[] { "Topluluk Kuralları", "Community Guidelines" });

            migrationBuilder.UpdateData(
                table: "LegalContents",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "Title", "TitleEn" },
                values: new object[] { "Moderasyon Politikası ve İçerik Yönetimi", "Moderation Policy and Content Review" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedDate", "ExpirationDate", "TrialExpirationDate" },
                values: new object[] { new DateTime(2026, 8, 13, 22, 0, 41, 205, DateTimeKind.Utc).AddTicks(603), new DateTime(2027, 8, 13, 22, 0, 41, 205, DateTimeKind.Utc).AddTicks(609), new DateTime(2026, 8, 20, 22, 0, 41, 205, DateTimeKind.Utc).AddTicks(605) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedDate", "ExpirationDate", "TrialExpirationDate" },
                values: new object[] { new DateTime(2026, 8, 13, 22, 0, 41, 205, DateTimeKind.Utc).AddTicks(637), new DateTime(2027, 8, 13, 22, 0, 41, 205, DateTimeKind.Utc).AddTicks(638), new DateTime(2026, 8, 20, 22, 0, 41, 205, DateTimeKind.Utc).AddTicks(638) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedDate", "ExpirationDate", "TrialExpirationDate" },
                values: new object[] { new DateTime(2026, 8, 13, 22, 0, 41, 205, DateTimeKind.Utc).AddTicks(645), new DateTime(2027, 8, 13, 22, 0, 41, 205, DateTimeKind.Utc).AddTicks(645), new DateTime(2026, 8, 20, 22, 0, 41, 205, DateTimeKind.Utc).AddTicks(645) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedDate", "ExpirationDate", "TrialExpirationDate" },
                values: new object[] { new DateTime(2026, 8, 13, 22, 0, 41, 205, DateTimeKind.Utc).AddTicks(650), new DateTime(2027, 8, 13, 22, 0, 41, 205, DateTimeKind.Utc).AddTicks(650), new DateTime(2026, 8, 20, 22, 0, 41, 205, DateTimeKind.Utc).AddTicks(650) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "LegalContents",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Title", "TitleEn" },
                values: new object[] { "MESAFELİ SATIŞ SÖZLEŞMESİ", "DISTANCE SALES AGREEMENT" });

            migrationBuilder.UpdateData(
                table: "LegalContents",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "Title", "TitleEn" },
                values: new object[] { "ÇEREZ POLİTİKASI", "COOKIE POLICY" });

            migrationBuilder.UpdateData(
                table: "LegalContents",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "Title", "TitleEn" },
                values: new object[] { "YASAL UYARI VE SORUMLULUK REDDİ BEYANI", "LEGAL NOTICE AND DISCLAIMER" });

            migrationBuilder.UpdateData(
                table: "LegalContents",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "Title", "TitleEn" },
                values: new object[] { "TOPLULUK KURALLARI", "COMMUNITY GUIDELINES" });

            migrationBuilder.UpdateData(
                table: "LegalContents",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "Title", "TitleEn" },
                values: new object[] { "MODERASYON POLİTİKASI VE İÇERİK DENETİMİ", "MODERATION POLICY AND CONTENT REVIEW" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedDate", "ExpirationDate", "TrialExpirationDate" },
                values: new object[] { new DateTime(2026, 8, 13, 21, 37, 40, 93, DateTimeKind.Utc).AddTicks(3681), new DateTime(2027, 8, 13, 21, 37, 40, 93, DateTimeKind.Utc).AddTicks(3688), new DateTime(2026, 8, 20, 21, 37, 40, 93, DateTimeKind.Utc).AddTicks(3683) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedDate", "ExpirationDate", "TrialExpirationDate" },
                values: new object[] { new DateTime(2026, 8, 13, 21, 37, 40, 93, DateTimeKind.Utc).AddTicks(3714), new DateTime(2027, 8, 13, 21, 37, 40, 93, DateTimeKind.Utc).AddTicks(3715), new DateTime(2026, 8, 20, 21, 37, 40, 93, DateTimeKind.Utc).AddTicks(3714) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedDate", "ExpirationDate", "TrialExpirationDate" },
                values: new object[] { new DateTime(2026, 8, 13, 21, 37, 40, 93, DateTimeKind.Utc).AddTicks(3723), new DateTime(2027, 8, 13, 21, 37, 40, 93, DateTimeKind.Utc).AddTicks(3723), new DateTime(2026, 8, 20, 21, 37, 40, 93, DateTimeKind.Utc).AddTicks(3723) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedDate", "ExpirationDate", "TrialExpirationDate" },
                values: new object[] { new DateTime(2026, 8, 13, 21, 37, 40, 93, DateTimeKind.Utc).AddTicks(3727), new DateTime(2027, 8, 13, 21, 37, 40, 93, DateTimeKind.Utc).AddTicks(3728), new DateTime(2026, 8, 20, 21, 37, 40, 93, DateTimeKind.Utc).AddTicks(3727) });
        }
    }
}
