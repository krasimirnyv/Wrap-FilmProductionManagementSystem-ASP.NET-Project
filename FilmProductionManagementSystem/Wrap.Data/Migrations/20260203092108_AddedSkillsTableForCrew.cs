#nullable disable

namespace Wrap.Data.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;
    
    /// <inheritdoc />
    public partial class AddedSkillsTableForCrew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CrewSkill",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CrewMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrewSkill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrewSkill_CrewMembers_CrewMemberId",
                        column: x => x.CrewMemberId,
                        principalTable: "CrewMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CrewSkill_CrewMemberId",
                table: "CrewSkill",
                column: "CrewMemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrewSkill");
        }
    }
}
