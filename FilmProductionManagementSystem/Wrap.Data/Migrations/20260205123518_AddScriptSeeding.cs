using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Wrap.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddScriptSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Productions",
                columns: new[] { "Id", "Budget", "Description", "StatusEndDate", "StatusStartDate", "StatusType", "Thumbnail", "Title" },
                values: new object[] { new Guid("809f7100-5b1d-4eee-8b82-8e4084ef0928"), 1000.00m, "This is a test", new DateTime(2026, 2, 10, 12, 35, 17, 887, DateTimeKind.Utc).AddTicks(9200), new DateTime(2026, 2, 5, 12, 35, 17, 887, DateTimeKind.Utc).AddTicks(9200), 1, null, "Test Film" });

            migrationBuilder.InsertData(
                table: "Scripts",
                columns: new[] { "Id", "Content", "LastEditedAt", "ProductionId", "Title" },
                values: new object[] { new Guid("36ca9b58-d902-4195-bf80-2e6518ad3c6d"), null, new DateTime(2026, 2, 5, 12, 35, 17, 888, DateTimeKind.Utc).AddTicks(1990), new Guid("809f7100-5b1d-4eee-8b82-8e4084ef0928"), "Test Screenplay" });

            migrationBuilder.InsertData(
                table: "ScriptBlocks",
                columns: new[] { "Id", "BlockType", "Content", "CreatedAt", "LastModifiedAt", "Metadata", "OrderIndex", "ScriptId" },
                values: new object[,]
                {
                    { new Guid("3bed994a-cbee-4d60-b22f-a922b82eb841"), 7, "I'll have a double espresso. Make it strong.", new DateTime(2026, 2, 5, 12, 35, 17, 888, DateTimeKind.Utc).AddTicks(1070), new DateTime(2026, 2, 5, 12, 35, 17, 888, DateTimeKind.Utc).AddTicks(1070), null, 3, new Guid("36ca9b58-d902-4195-bf80-2e6518ad3c6d") },
                    { new Guid("65a5a61f-204d-4274-be48-bf4b440ff6a1"), 1, "INT. COFFEE SHOP - DAY", new DateTime(2026, 2, 5, 12, 35, 17, 888, DateTimeKind.Utc).AddTicks(1060), new DateTime(2026, 2, 5, 12, 35, 17, 888, DateTimeKind.Utc).AddTicks(1060), null, 0, new Guid("36ca9b58-d902-4195-bf80-2e6518ad3c6d") },
                    { new Guid("befd3f37-d237-4e46-8a21-e08704c6ef00"), 4, "JOHN", new DateTime(2026, 2, 5, 12, 35, 17, 888, DateTimeKind.Utc).AddTicks(1060), new DateTime(2026, 2, 5, 12, 35, 17, 888, DateTimeKind.Utc).AddTicks(1060), null, 2, new Guid("36ca9b58-d902-4195-bf80-2e6518ad3c6d") },
                    { new Guid("c5cc8272-c35d-4b4b-bb31-137df7fe86d5"), 3, "A bustling morning crowd. Steam rises from espresso machines.", new DateTime(2026, 2, 5, 12, 35, 17, 888, DateTimeKind.Utc).AddTicks(1060), new DateTime(2026, 2, 5, 12, 35, 17, 888, DateTimeKind.Utc).AddTicks(1060), null, 1, new Guid("36ca9b58-d902-4195-bf80-2e6518ad3c6d") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ScriptBlocks",
                keyColumn: "Id",
                keyValue: new Guid("3bed994a-cbee-4d60-b22f-a922b82eb841"));

            migrationBuilder.DeleteData(
                table: "ScriptBlocks",
                keyColumn: "Id",
                keyValue: new Guid("65a5a61f-204d-4274-be48-bf4b440ff6a1"));

            migrationBuilder.DeleteData(
                table: "ScriptBlocks",
                keyColumn: "Id",
                keyValue: new Guid("befd3f37-d237-4e46-8a21-e08704c6ef00"));

            migrationBuilder.DeleteData(
                table: "ScriptBlocks",
                keyColumn: "Id",
                keyValue: new Guid("c5cc8272-c35d-4b4b-bb31-137df7fe86d5"));

            migrationBuilder.DeleteData(
                table: "Scripts",
                keyColumn: "Id",
                keyValue: new Guid("36ca9b58-d902-4195-bf80-2e6518ad3c6d"));

            migrationBuilder.DeleteData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("809f7100-5b1d-4eee-8b82-8e4084ef0928"));
        }
    }
}
