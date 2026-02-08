using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wrap.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddScriptBlocksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScriptBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    BlockType = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    Metadata = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    ScriptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScriptBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScriptBlocks_Scripts_ScriptId",
                        column: x => x.ScriptId,
                        principalTable: "Scripts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScriptBlocks_ScriptId_OrderIndex",
                table: "ScriptBlocks",
                columns: new[] { "ScriptId", "OrderIndex" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScriptBlocks");
        }
    }
}
