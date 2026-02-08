using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wrap.Data.Migrations
{
    /// <inheritdoc />
    public partial class SimplifiedDataBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentAmount",
                table: "CrewMembers");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "CrewMembers");

            migrationBuilder.DropColumn(
                name: "RoleType",
                table: "CrewMembers");

            migrationBuilder.DropColumn(
                name: "PaymentAmount",
                table: "CastMembers");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "CastMembers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PaymentAmount",
                table: "CrewMembers",
                type: "DECIMAL(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                table: "CrewMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoleType",
                table: "CrewMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PaymentAmount",
                table: "CastMembers",
                type: "DECIMAL(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                table: "CastMembers",
                type: "int",
                nullable: true);
        }
    }
}
