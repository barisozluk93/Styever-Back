using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MemoryManagement.Migrations
{
    /// <inheritdoc />
    public partial class Inital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    EnName = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Memories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DeathDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PostDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPrivate = table.Column<bool>(type: "boolean", nullable: false),
                    IsOpenToComment = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Memories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemoryComments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    MemoryId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemoryComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemoryComments_Memories_MemoryId",
                        column: x => x.MemoryId,
                        principalTable: "Memories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemoryFiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MemoryId = table.Column<long>(type: "bigint", nullable: false),
                    FileId = table.Column<long>(type: "bigint", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemoryFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemoryFiles_Memories_MemoryId",
                        column: x => x.MemoryId,
                        principalTable: "Memories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemoryLikes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MemoryId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemoryLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemoryLikes_Memories_MemoryId",
                        column: x => x.MemoryId,
                        principalTable: "Memories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "EnName", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1L, "Bird", false, "Kuş" },
                    { 2L, "Cat", false, "Kedi" },
                    { 3L, "Dog", false, "Köpek" },
                    { 4L, "Fish", false, "Balık" },
                    { 5L, "Hamster", false, "Fare" },
                    { 6L, "Horse", false, "At" },
                    { 7L, "Turtle", false, "Kaplumbağa" }
                });

            migrationBuilder.InsertData(
                table: "Memories",
                columns: new[] { "Id", "BirthDate", "CategoryId", "DeathDate", "IsDeleted", "IsOpenToComment", "IsPrivate", "Name", "PostDate", "Text", "UserId" },
                values: new object[,]
                {
                    { 1L, new DateOnly(2023, 11, 12), 2L, new DateOnly(2024, 8, 28), false, true, false, "Queenie", new DateTime(2025, 11, 19, 10, 47, 4, 190, DateTimeKind.Utc).AddTicks(5353), "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n", 2L },
                    { 2L, new DateOnly(2020, 4, 4), 3L, new DateOnly(2025, 5, 5), false, true, false, "Ringo", new DateTime(2025, 11, 19, 10, 47, 4, 190, DateTimeKind.Utc).AddTicks(5360), "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n", 3L },
                    { 3L, new DateOnly(2024, 1, 12), 4L, new DateOnly(2024, 8, 18), false, false, false, "Bubbles", new DateTime(2025, 11, 19, 10, 47, 4, 190, DateTimeKind.Utc).AddTicks(5362), "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n", 4L },
                    { 4L, new DateOnly(2018, 2, 14), 6L, new DateOnly(2024, 11, 24), false, true, true, "George", new DateTime(2025, 11, 19, 10, 47, 4, 190, DateTimeKind.Utc).AddTicks(5364), "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n", 4L },
                    { 5L, new DateOnly(2018, 2, 14), 6L, new DateOnly(2024, 11, 24), false, true, false, "Fredy", new DateTime(2025, 11, 19, 10, 47, 4, 190, DateTimeKind.Utc).AddTicks(5422), "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n", 3L },
                    { 6L, new DateOnly(2018, 2, 14), 6L, new DateOnly(2024, 11, 24), false, true, false, "Hato", new DateTime(2025, 11, 19, 10, 47, 4, 190, DateTimeKind.Utc).AddTicks(5424), "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n", 3L },
                    { 7L, new DateOnly(2018, 2, 14), 6L, new DateOnly(2024, 11, 24), false, true, true, "Aaaaa", new DateTime(2025, 11, 19, 10, 47, 4, 190, DateTimeKind.Utc).AddTicks(5426), "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n", 3L },
                    { 8L, new DateOnly(2018, 2, 14), 6L, new DateOnly(2024, 11, 24), false, true, false, "Bbbbbb", new DateTime(2025, 11, 19, 10, 47, 4, 190, DateTimeKind.Utc).AddTicks(5427), "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n", 3L },
                    { 9L, new DateOnly(2018, 2, 14), 6L, new DateOnly(2024, 11, 24), false, true, false, "Cccccc", new DateTime(2025, 11, 19, 10, 47, 4, 190, DateTimeKind.Utc).AddTicks(5429), "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n", 2L },
                    { 10L, new DateOnly(2018, 2, 14), 6L, new DateOnly(2024, 11, 24), false, true, false, "Ddddddd", new DateTime(2025, 11, 19, 10, 47, 4, 190, DateTimeKind.Utc).AddTicks(5430), "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n", 2L },
                    { 11L, new DateOnly(2018, 2, 14), 6L, new DateOnly(2024, 11, 24), false, true, false, "Eeeeeee", new DateTime(2025, 11, 19, 10, 47, 4, 190, DateTimeKind.Utc).AddTicks(5432), "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n", 3L },
                    { 12L, new DateOnly(2018, 2, 14), 6L, new DateOnly(2024, 11, 24), false, true, false, "Ffffff", new DateTime(2025, 11, 19, 10, 47, 4, 190, DateTimeKind.Utc).AddTicks(5433), "Our precious Queenie crossed the Rainbow Bridge at 13 years old, leaving behind gentle pawprints on our hearts. Graceful, kind and full of quiet love, she filled our home with warmth and calm. Her soft purrs, curious eyes and tender ways brought comfort to every day and joy to every corner. Though the house feels still without her, we see her everywhere :- in the sunlight on the floor, in the quiet of the evening, and in every happy memory she gave us. Sleep softly, dear Queenie. You’ll always be part of our hearts and our home. Forever loved and sadly missed by Bryan, Lisa and Sarah\r\n", 3L }
                });

            migrationBuilder.InsertData(
                table: "MemoryComments",
                columns: new[] { "Id", "Comment", "Date", "IsDeleted", "MemoryId", "UserId" },
                values: new object[,]
                {
                    { 1L, "Bir bir yorumdur.", new DateTime(2025, 11, 19, 10, 47, 4, 190, DateTimeKind.Utc).AddTicks(5510), false, 1L, 3L },
                    { 2L, "Bir bir yorumdur. (2)", new DateTime(2025, 11, 19, 10, 47, 4, 190, DateTimeKind.Utc).AddTicks(5516), false, 1L, 1L },
                    { 3L, "Bir bir yorumdur. (3)", new DateTime(2025, 11, 19, 10, 47, 4, 190, DateTimeKind.Utc).AddTicks(5517), false, 1L, 4L },
                    { 4L, "Bir bir yorumdur. (4)", new DateTime(2025, 11, 19, 10, 47, 4, 190, DateTimeKind.Utc).AddTicks(5518), false, 1L, 1L }
                });

            migrationBuilder.InsertData(
                table: "MemoryFiles",
                columns: new[] { "Id", "FileId", "IsDeleted", "IsPrimary", "MemoryId" },
                values: new object[,]
                {
                    { 1L, 1L, false, true, 1L },
                    { 2L, 2L, false, true, 2L },
                    { 3L, 3L, false, true, 3L },
                    { 4L, 4L, false, true, 4L },
                    { 5L, 5L, false, false, 4L },
                    { 6L, 6L, false, false, 4L },
                    { 7L, 2L, false, true, 6L },
                    { 8L, 3L, false, true, 7L },
                    { 9L, 1L, false, true, 8L },
                    { 10L, 4L, false, true, 9L },
                    { 11L, 6L, false, true, 10L },
                    { 12L, 5L, false, true, 11L },
                    { 13L, 6L, false, true, 12L },
                    { 14L, 6L, false, true, 5L }
                });

            migrationBuilder.InsertData(
                table: "MemoryLikes",
                columns: new[] { "Id", "Date", "IsDeleted", "MemoryId", "UserId" },
                values: new object[,]
                {
                    { 1L, new DateTime(2025, 11, 19, 10, 47, 4, 190, DateTimeKind.Utc).AddTicks(5544), false, 1L, 1L },
                    { 2L, new DateTime(2025, 11, 19, 10, 47, 4, 190, DateTimeKind.Utc).AddTicks(5545), false, 1L, 3L },
                    { 3L, new DateTime(2025, 11, 19, 10, 47, 4, 190, DateTimeKind.Utc).AddTicks(5546), false, 1L, 4L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Memories_CategoryId",
                table: "Memories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MemoryComments_MemoryId",
                table: "MemoryComments",
                column: "MemoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MemoryFiles_MemoryId",
                table: "MemoryFiles",
                column: "MemoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MemoryLikes_MemoryId",
                table: "MemoryLikes",
                column: "MemoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemoryComments");

            migrationBuilder.DropTable(
                name: "MemoryFiles");

            migrationBuilder.DropTable(
                name: "MemoryLikes");

            migrationBuilder.DropTable(
                name: "Memories");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
