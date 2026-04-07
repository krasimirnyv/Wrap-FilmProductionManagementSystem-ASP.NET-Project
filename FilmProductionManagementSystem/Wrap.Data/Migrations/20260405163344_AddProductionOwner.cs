using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wrap.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Productions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("019aa8c4-7b21-4b93-a1b4-18228fa10027"),
                column: "CreatedByUserId",
                value: new Guid("a1093e45-6993-4d59-8e3f-4fe0b29e7dc5"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("12abb9d5-8c32-4ca4-b2c5-293390b20028"),
                column: "CreatedByUserId",
                value: new Guid("f704cf4b-b01a-4b5d-a052-7043caffd93e"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("1e57a6d4-7f88-41bb-a836-83bb97b89008"),
                column: "CreatedByUserId",
                value: new Guid("5150238b-cd37-482d-ab11-ef66bae0128f"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d01"),
                column: "CreatedByUserId",
                value: new Guid("f704cf4b-b01a-4b5d-a052-7043caffd93e"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d02"),
                column: "CreatedByUserId",
                value: new Guid("5150238b-cd37-482d-ab11-ef66bae0128f"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d03"),
                column: "CreatedByUserId",
                value: new Guid("a1093e45-6993-4d59-8e3f-4fe0b29e7dc5"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d04"),
                column: "CreatedByUserId",
                value: new Guid("f704cf4b-b01a-4b5d-a052-7043caffd93e"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d05"),
                column: "CreatedByUserId",
                value: new Guid("5150238b-cd37-482d-ab11-ef66bae0128f"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d06"),
                column: "CreatedByUserId",
                value: new Guid("a1093e45-6993-4d59-8e3f-4fe0b29e7dc5"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d07"),
                column: "CreatedByUserId",
                value: new Guid("f704cf4b-b01a-4b5d-a052-7043caffd93e"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d08"),
                column: "CreatedByUserId",
                value: new Guid("5150238b-cd37-482d-ab11-ef66bae0128f"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d09"),
                column: "CreatedByUserId",
                value: new Guid("a1093e45-6993-4d59-8e3f-4fe0b29e7dc5"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("3e8a1c57-6d21-4bcb-8dc4-2f6b42a19002"),
                column: "CreatedByUserId",
                value: new Guid("5150238b-cd37-482d-ab11-ef66bae0128f"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("5b7f9e22-8a11-47c1-bf28-47db53d49004"),
                column: "CreatedByUserId",
                value: new Guid("f704cf4b-b01a-4b5d-a052-7043caffd93e"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("6d91c8f4-5e21-4b69-8a5d-61ed75f69006"),
                column: "CreatedByUserId",
                value: new Guid("a1093e45-6993-4d59-8e3f-4fe0b29e7dc5"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("7c5c7d63-1f42-4d89-9b91-1b5d1f3a9001"),
                column: "CreatedByUserId",
                value: new Guid("f704cf4b-b01a-4b5d-a052-7043caffd93e"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("819f7100-5b1d-4eee-8b82-8e4084ef0928"),
                column: "CreatedByUserId",
                value: new Guid("a1093e45-6993-4d59-8e3f-4fe0b29e7dc5"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("9f2d8a11-3c4e-4d7b-a2d4-38ce72c39003"),
                column: "CreatedByUserId",
                value: new Guid("a1093e45-6993-4d59-8e3f-4fe0b29e7dc5"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("a13e4b90-2c74-4d3a-91c8-5cfc64e59005"),
                column: "CreatedByUserId",
                value: new Guid("5150238b-cd37-482d-ab11-ef66bae0128f"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("a5d7ecf8-bf65-4fd7-d5e8-55b6c4e50015"),
                column: "CreatedByUserId",
                value: new Guid("a1093e45-6993-4d59-8e3f-4fe0b29e7dc5"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("ab34425e-15cb-453d-b45e-b21c2a4b0021"),
                column: "CreatedByUserId",
                value: new Guid("a1093e45-6993-4d59-8e3f-4fe0b29e7dc5"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("b6e8fd09-c076-40e8-e6f9-66c7d5f60016"),
                column: "CreatedByUserId",
                value: new Guid("f704cf4b-b01a-4b5d-a052-7043caffd93e"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("bc45536f-26dc-464e-c56f-c32d3b5c0022"),
                column: "CreatedByUserId",
                value: new Guid("f704cf4b-b01a-4b5d-a052-7043caffd93e"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("bc6f0d83-9a52-43de-bc71-72aa86a79007"),
                column: "CreatedByUserId",
                value: new Guid("f704cf4b-b01a-4b5d-a052-7043caffd93e"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("c1f3a8d4-7b21-4b93-91a4-11d2f0a10011"),
                column: "CreatedByUserId",
                value: new Guid("5150238b-cd37-482d-ab11-ef66bae0128f"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("c7f90e1a-d187-41f9-f70a-77d8e6070017"),
                column: "CreatedByUserId",
                value: new Guid("5150238b-cd37-482d-ab11-ef66bae0128f"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("cd566480-37ed-475f-d670-d43e4c6d0023"),
                column: "CreatedByUserId",
                value: new Guid("5150238b-cd37-482d-ab11-ef66bae0128f"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("d2a4b9e5-8c32-4ca4-a2b5-22e3f1b20012"),
                column: "CreatedByUserId",
                value: new Guid("a1093e45-6993-4d59-8e3f-4fe0b29e7dc5"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("d2c49f75-0b33-4a2f-96a1-94cc08c99009"),
                column: "CreatedByUserId",
                value: new Guid("a1093e45-6993-4d59-8e3f-4fe0b29e7dc5"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("d8011f2b-e298-420a-812b-88e9f7180018"),
                column: "CreatedByUserId",
                value: new Guid("a1093e45-6993-4d59-8e3f-4fe0b29e7dc5"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("de677591-48fe-4860-e781-e54f5d7e0024"),
                column: "CreatedByUserId",
                value: new Guid("a1093e45-6993-4d59-8e3f-4fe0b29e7dc5"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("e3b5cad6-9d43-4db5-b3c6-33f4a2c30013"),
                column: "CreatedByUserId",
                value: new Guid("f704cf4b-b01a-4b5d-a052-7043caffd93e"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("e912203c-f3a9-431b-923c-99fa08290019"),
                column: "CreatedByUserId",
                value: new Guid("f704cf4b-b01a-4b5d-a052-7043caffd93e"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("ef7886a2-590f-4971-f892-f6606e8f0025"),
                column: "CreatedByUserId",
                value: new Guid("f704cf4b-b01a-4b5d-a052-7043caffd93e"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("f08997b3-6a10-4a82-90a3-07117f900026"),
                column: "CreatedByUserId",
                value: new Guid("5150238b-cd37-482d-ab11-ef66bae0128f"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("f4c6dbe7-ae54-4ec6-c4d7-44a5b3d40014"),
                column: "CreatedByUserId",
                value: new Guid("5150238b-cd37-482d-ab11-ef66bae0128f"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("f84a72b1-6c95-45ad-b417-a5dd19da9010"),
                column: "CreatedByUserId",
                value: new Guid("f704cf4b-b01a-4b5d-a052-7043caffd93e"));

            migrationBuilder.UpdateData(
                table: "Productions",
                keyColumn: "Id",
                keyValue: new Guid("fa23314d-04ba-442c-a34d-a10b193a0020"),
                column: "CreatedByUserId",
                value: new Guid("5150238b-cd37-482d-ab11-ef66bae0128f"));

            migrationBuilder.CreateIndex(
                name: "IX_Productions_CreatedByUserId",
                table: "Productions",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productions_AspNetUsers_CreatedByUserId",
                table: "Productions",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productions_AspNetUsers_CreatedByUserId",
                table: "Productions");

            migrationBuilder.DropIndex(
                name: "IX_Productions_CreatedByUserId",
                table: "Productions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Productions");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "AspNetRoles",
                type: "nvarchar(27)",
                maxLength: 27,
                nullable: true);
        }
    }
}
