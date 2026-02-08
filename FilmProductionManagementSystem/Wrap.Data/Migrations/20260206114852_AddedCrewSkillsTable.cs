using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wrap.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedCrewSkillsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrewSkill_CrewMembers_CrewMemberId",
                table: "CrewSkill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CrewSkill",
                table: "CrewSkill");

            migrationBuilder.RenameTable(
                name: "CrewSkill",
                newName: "CrewSkills");

            migrationBuilder.RenameIndex(
                name: "IX_CrewSkill_CrewMemberId",
                table: "CrewSkills",
                newName: "IX_CrewSkills_CrewMemberId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CrewSkills",
                table: "CrewSkills",
                column: "Id");
            
            migrationBuilder.AddForeignKey(
                name: "FK_CrewSkills_CrewMembers_CrewMemberId",
                table: "CrewSkills",
                column: "CrewMemberId",
                principalTable: "CrewMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrewSkills_CrewMembers_CrewMemberId",
                table: "CrewSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CrewSkills",
                table: "CrewSkills");

            migrationBuilder.RenameTable(
                name: "CrewSkills",
                newName: "CrewSkill");

            migrationBuilder.RenameIndex(
                name: "IX_CrewSkills_CrewMemberId",
                table: "CrewSkill",
                newName: "IX_CrewSkill_CrewMemberId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CrewSkill",
                table: "CrewSkill",
                column: "Id");
            
            migrationBuilder.AddForeignKey(
                name: "FK_CrewSkill_CrewMembers_CrewMemberId",
                table: "CrewSkill",
                column: "CrewMemberId",
                principalTable: "CrewMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
