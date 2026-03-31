using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Wrap.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrationWithDataSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(27)", maxLength: 27, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Budget = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    Thumbnail = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StatusType = table.Column<int>(type: "int", nullable: false),
                    StatusStartDate = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    StatusEndDate = table.Column<DateTime>(type: "DATETIME2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CastMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Nickname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Biography = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CastMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CastMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CrewMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Nickname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Biography = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrewMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrewMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionsAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetType = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    ProductionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionsAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionsAssets_Productions_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Productions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Scenes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SceneNumber = table.Column<int>(type: "int", nullable: false),
                    SceneType = table.Column<int>(type: "int", nullable: false),
                    SceneName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ProductionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scenes_Productions_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Productions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Scripts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LastEditedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    StageType = table.Column<int>(type: "int", nullable: false),
                    RevisionType = table.Column<int>(type: "int", nullable: true),
                    ProductionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scripts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scripts_Productions_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Productions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShootingDays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "DATETIME2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    ProductionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShootingDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShootingDays_Productions_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Productions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionsCastMembers",
                columns: table => new
                {
                    ProductionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CastMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionsCastMembers", x => new { x.ProductionId, x.CastMemberId });
                    table.ForeignKey(
                        name: "FK_ProductionsCastMembers_CastMembers_CastMemberId",
                        column: x => x.CastMemberId,
                        principalTable: "CastMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductionsCastMembers_Productions_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Productions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CrewSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleType = table.Column<int>(type: "int", nullable: false),
                    CrewMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrewSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrewSkills_CrewMembers_CrewMemberId",
                        column: x => x.CrewMemberId,
                        principalTable: "CrewMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionsCrewMembers",
                columns: table => new
                {
                    ProductionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CrewMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionsCrewMembers", x => new { x.ProductionId, x.CrewMemberId });
                    table.ForeignKey(
                        name: "FK_ProductionsCrewMembers_CrewMembers_CrewMemberId",
                        column: x => x.CrewMemberId,
                        principalTable: "CrewMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductionsCrewMembers_Productions_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "Productions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenesCastMembers",
                columns: table => new
                {
                    SceneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CastMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenesCastMembers", x => new { x.SceneId, x.CastMemberId });
                    table.ForeignKey(
                        name: "FK_ScenesCastMembers_CastMembers_CastMemberId",
                        column: x => x.CastMemberId,
                        principalTable: "CastMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenesCastMembers_Scenes_SceneId",
                        column: x => x.SceneId,
                        principalTable: "Scenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenesCrewMembers",
                columns: table => new
                {
                    SceneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CrewMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenesCrewMembers", x => new { x.SceneId, x.CrewMemberId });
                    table.ForeignKey(
                        name: "FK_ScenesCrewMembers_CrewMembers_CrewMemberId",
                        column: x => x.CrewMemberId,
                        principalTable: "CrewMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScenesCrewMembers_Scenes_SceneId",
                        column: x => x.SceneId,
                        principalTable: "Scenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "ShootingDaysScenes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ShootingDayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SceneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShootingDaysScenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShootingDaysScenes_Scenes_SceneId",
                        column: x => x.SceneId,
                        principalTable: "Scenes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShootingDaysScenes_ShootingDays_ShootingDayId",
                        column: x => x.ShootingDayId,
                        principalTable: "ShootingDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("47a394d7-2ddb-4511-8355-a489dc8ea3f5"), 0, "seed-cast-con-2", "georgi.ivanov@wrap.local", true, false, null, "GEORGI.IVANOV@WRAP.LOCAL", "GEORGI.IVANOV", null, "+359888100002", true, "seed-cast-sec-2", false, "georgi.ivanov" },
                    { new Guid("5150238b-cd37-482d-ab11-ef66bae0128f"), 0, "seed-crew-con-3", "ivan.dimitrov@wrap.local", true, false, null, "IVAN.DIMITROV@WRAP.LOCAL", "IVAN.DIMITROV", null, "+359888000003", true, "seed-crew-sec-3", false, "ivan.dimitrov" },
                    { new Guid("5f279043-b38f-4f5f-b990-12fd90dcc30e"), 0, "seed-cast-con-1", "elena.stoyanova@wrap.local", true, false, null, "ELENA.STOYANOVA@WRAP.LOCAL", "ELENA.STOYANOVA", null, "+359888100001", true, "seed-cast-sec-1", false, "elena.stoyanova" },
                    { new Guid("a1093e45-6993-4d59-8e3f-4fe0b29e7dc5"), 0, "seed-crew-con-1", "alex.petrov@wrap.local", true, false, null, "ALEX.PETROV@WRAP.LOCAL", "ALEX.PETROV", null, "+359888000001", true, "seed-crew-sec-1", false, "alex.petrov" },
                    { new Guid("a77cc134-28f3-45d3-8e46-ba041a5371d0"), 0, "seed-cast-con-3", "siyana.petrova@wrap.local", true, false, null, "SIYANA.PETROVA@WRAP.LOCAL", "SIYANA.PETROVA", null, "+359888100003", true, "seed-cast-sec-3", false, "siyana.petrova" },
                    { new Guid("f704cf4b-b01a-4b5d-a052-7043caffd93e"), 0, "seed-crew-con-2", "maria.georgieva@wrap.local", true, false, null, "MARIA.GEORGIEVA@WRAP.LOCAL", "MARIA.GEORGIEVA", null, "+359888000002", true, "seed-crew-sec-2", false, "maria.georgieva" }
                });

            migrationBuilder.InsertData(
                table: "Productions",
                columns: new[] { "Id", "Budget", "Description", "StatusEndDate", "StatusStartDate", "StatusType", "Thumbnail", "Title" },
                values: new object[,]
                {
                    { new Guid("019aa8c4-7b21-4b93-a1b4-18228fa10027"), 155000.00m, "An artist spirals into obsession while preparing the final exhibition of his career.", new DateTime(2026, 9, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 10, 0, 0, 0, 0, DateTimeKind.Utc), 21, "/img/thumbnail/default-thumbnail.png", "Burning Canvas" },
                    { new Guid("12abb9d5-8c32-4ca4-b2c5-293390b20028"), 500000.00m, "A post-apocalyptic survival story abandoned after loss of primary financing.", new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), 22, "/img/thumbnail/default-thumbnail.png", "No Signal Tomorrow" },
                    { new Guid("1e57a6d4-7f88-41bb-a836-83bb97b89008"), 110000.00m, "A quiet character-driven story of renewal in a small coastal town.", new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 18, 0, 0, 0, 0, DateTimeKind.Utc), 11, "/img/thumbnail/default-thumbnail.png", "After the Rain" },
                    { new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d01"), 250000.00m, "A neo-noir mystery unfolding over one sleepless night.", new DateTime(2026, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), 2, "/img/thumbnail/default-thumbnail.png", "Midnight Dreams" },
                    { new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d02"), 120000.00m, "A coming-of-age drama about ambition, fear, and first love.", new DateTime(2026, 3, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc), 5, "/img/thumbnail/default-thumbnail.png", "Paper Planes" },
                    { new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d03"), 180000.00m, "A comedic behind-the-scenes story about a cursed film set.", new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), 3, "/img/thumbnail/default-thumbnail.png", "The Last Take" },
                    { new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d04"), 750000.00m, "A sci-fi thriller set in a near-future megacity.", new DateTime(2026, 1, 25, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, "/img/thumbnail/default-thumbnail.png", "Neon Skyline" },
                    { new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d05"), 90000.00m, "A documentary journey following musicians on the road.", new DateTime(2026, 2, 18, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, "/img/thumbnail/default-thumbnail.png", "Wild Tracks" },
                    { new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d06"), 60000.00m, "A character study about burnout and reinvention.", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), 9, "/img/thumbnail/default-thumbnail.png", "On Hold" },
                    { new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d07"), 320000.00m, "A tense drama about fixing a film before the deadline.", new DateTime(2026, 4, 25, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc), 10, "/img/thumbnail/default-thumbnail.png", "Reshoot Season" },
                    { new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d08"), 140000.00m, "An art-house film exploring memory through sound and color.", new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), 14, "/img/thumbnail/default-thumbnail.png", "Color & Noise" },
                    { new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d09"), 210000.00m, "A finished film preparing for its festival circuit debut.", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 19, "/img/thumbnail/default-thumbnail.png", "Festival Run" },
                    { new Guid("3e8a1c57-6d21-4bcb-8dc4-2f6b42a19002"), 540000.00m, "A visually driven sci-fi drama set on the edge of a collapsing city.", new DateTime(2026, 1, 28, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 8, 0, 0, 0, 0, DateTimeKind.Utc), 5, "/img/thumbnail/default-thumbnail.png", "Glass Horizon" },
                    { new Guid("5b7f9e22-8a11-47c1-bf28-47db53d49004"), 280000.00m, "A stylish thriller following a late-night radio host drawn into a conspiracy.", new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), 6, "/img/thumbnail/default-thumbnail.png", "Velvet Signal" },
                    { new Guid("6d91c8f4-5e21-4b69-8a5d-61ed75f69006"), 410000.00m, "A crime thriller in which a photographer uncovers evidence hidden in old negatives.", new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), 10, "/img/thumbnail/default-thumbnail.png", "Broken Frame" },
                    { new Guid("7c5c7d63-1f42-4d89-9b91-1b5d1f3a9001"), 175000.00m, "A psychological drama about memory, guilt, and unresolved loss.", new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), 4, "/img/thumbnail/default-thumbnail.png", "Silent Echo" },
                    { new Guid("819f7100-5b1d-4eee-8b82-8e4084ef0928"), 1000.00m, "This is a test", new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "/img/thumbnail/default-thumbnail.png", "Test Film" },
                    { new Guid("9f2d8a11-3c4e-4d7b-a2d4-38ce72c39003"), 130000.00m, "A nostalgic indie drama about friendship fractured by a long-buried secret.", new DateTime(2026, 3, 28, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), 22, "/img/thumbnail/default-thumbnail.png", "Ashes of Summer" },
                    { new Guid("a13e4b90-2c74-4d3a-91c8-5cfc64e59005"), 95000.00m, "A bittersweet romance between two artists during the final week of summer.", new DateTime(2026, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), 3, "/img/thumbnail/default-thumbnail.png", "Fading Lights" },
                    { new Guid("a5d7ecf8-bf65-4fd7-d5e8-55b6c4e50015"), 150000.00m, "A winter mystery centered around a box of unsent letters discovered in an abandoned house.", new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 22, 0, 0, 0, 0, DateTimeKind.Utc), 6, "/img/thumbnail/default-thumbnail.png", "Frozen Letters" },
                    { new Guid("ab34425e-15cb-453d-b45e-b21c2a4b0021"), 240000.00m, "A road thriller about a failed musician and a runaway chasing redemption.", new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 12, "/img/thumbnail/default-thumbnail.png", "Dust and Neon" },
                    { new Guid("b6e8fd09-c076-40e8-e6f9-66c7d5f60016"), 980000.00m, "A historical war drama about espionage, sacrifice, and impossible choices.", new DateTime(2026, 3, 18, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7, "/img/thumbnail/default-thumbnail.png", "Iron Veil" },
                    { new Guid("bc45536f-26dc-464e-c56f-c32d3b5c0022"), 185000.00m, "A suspense drama in which a photographer discovers incriminating details in developed film.", new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 10, 0, 0, 0, 0, DateTimeKind.Utc), 13, "/img/thumbnail/default-thumbnail.png", "Second Exposure" },
                    { new Guid("bc6f0d83-9a52-43de-bc71-72aa86a79007"), 160000.00m, "A road movie about estranged siblings forced to travel together across the country.", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 14, 0, 0, 0, 0, DateTimeKind.Utc), 6, "/img/thumbnail/default-thumbnail.png", "Northbound" },
                    { new Guid("c1f3a8d4-7b21-4b93-91a4-11d2f0a10011"), 430000.00m, "A noir thriller about a detective uncovering corruption in a decaying port city.", new DateTime(2026, 2, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 18, 0, 0, 0, 0, DateTimeKind.Utc), 2, "/img/thumbnail/default-thumbnail.png", "Crimson Harbor" },
                    { new Guid("c7f90e1a-d187-41f9-f70a-77d8e6070017"), 120000.00m, "A light coming-of-age dramedy about three friends reliving their last summer together.", new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 5, 0, 0, 0, 0, DateTimeKind.Utc), 8, "/img/thumbnail/default-thumbnail.png", "Summer Rewind" },
                    { new Guid("cd566480-37ed-475f-d670-d43e4c6d0023"), 410000.00m, "A subtle sci-fi drama about loneliness, distance, and first contact.", new DateTime(2026, 7, 18, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 20, 0, 0, 0, 0, DateTimeKind.Utc), 15, "/img/thumbnail/default-thumbnail.png", "Quiet Signal" },
                    { new Guid("d2a4b9e5-8c32-4ca4-a2b5-22e3f1b20012"), 275000.00m, "A courtroom drama about a musician fighting to reclaim her voice and reputation.", new DateTime(2026, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), 4, "/img/thumbnail/default-thumbnail.png", "Shattered Silence" },
                    { new Guid("d2c49f75-0b33-4a2f-96a1-94cc08c99009"), 145000.00m, "An experimental ensemble film weaving together intersecting monologues.", new DateTime(2026, 6, 28, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), 13, "/img/thumbnail/default-thumbnail.png", "Parallel Voices" },
                    { new Guid("d8011f2b-e298-420a-812b-88e9f7180018"), 650000.00m, "A dystopian thriller about a journalist navigating a city emptied by fear and surveillance.", new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 14, 0, 0, 0, 0, DateTimeKind.Utc), 9, "/img/thumbnail/default-thumbnail.png", "Hollow City" },
                    { new Guid("de677591-48fe-4860-e781-e54f5d7e0024"), 105000.00m, "A chamber drama about neighbors whose lives intersect after a tragic accident.", new DateTime(2026, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), 17, "/img/thumbnail/default-thumbnail.png", "The Open Window" },
                    { new Guid("e3b5cad6-9d43-4db5-b3c6-33f4a2c30013"), 95000.00m, "A heartfelt drama following a family photographer confronting the passage of time.", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 8, 0, 0, 0, 0, DateTimeKind.Utc), 5, "/img/thumbnail/default-thumbnail.png", "Golden Hour" },
                    { new Guid("e912203c-f3a9-431b-923c-99fa08290019"), 360000.00m, "A mystery-thriller where eerie radio transmissions begin predicting violent events.", new DateTime(2026, 5, 24, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), 10, "/img/thumbnail/default-thumbnail.png", "Blue Static" },
                    { new Guid("ef7886a2-590f-4971-f892-f6606e8f0025"), 195000.00m, "A tense airport-set drama about missed chances, final goodbyes, and unexpected reunions.", new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 7, 15, 0, 0, 0, 0, DateTimeKind.Utc), 18, "/img/thumbnail/default-thumbnail.png", "Last Departure" },
                    { new Guid("f08997b3-6a10-4a82-90a3-07117f900026"), 70000.00m, "A minimalist drama about two strangers connected by a late-night emergency call.", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), 20, "/img/thumbnail/default-thumbnail.png", "Echo Line" },
                    { new Guid("f4c6dbe7-ae54-4ec6-c4d7-44a5b3d40014"), 210000.00m, "An intimate psychological drama about fame, obsession, and self-destruction.", new DateTime(2026, 3, 25, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 25, 0, 0, 0, 0, DateTimeKind.Utc), 3, "/img/thumbnail/default-thumbnail.png", "Velvet Ashes" },
                    { new Guid("f84a72b1-6c95-45ad-b417-a5dd19da9010"), 85000.00m, "A chamber drama centered on family tension during the reading of a final will.", new DateTime(2026, 8, 10, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), 18, "/img/thumbnail/default-thumbnail.png", "Final Arrangement" },
                    { new Guid("fa23314d-04ba-442c-a34d-a10b193a0020"), 80000.00m, "A poetic indie romance unfolding over a single night in Sofia.", new DateTime(2026, 6, 12, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Utc), 11, "/img/thumbnail/default-thumbnail.png", "Paper Moonlight" }
                });

            migrationBuilder.InsertData(
                table: "CastMembers",
                columns: new[] { "Id", "Biography", "BirthDate", "FirstName", "Gender", "IsActive", "IsDeleted", "LastName", "Nickname", "ProfileImagePath", "UserId" },
                values: new object[,]
                {
                    { new Guid("076fd414-bd51-44a5-9d19-1f0a321e4d8e"), "Stage and screen actor, passionate about drama and indie productions.", new DateTime(1998, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Elena", 2, true, false, "Stoyanova", "Eli", "/img/profile/default-profile.png", new Guid("5f279043-b38f-4f5f-b990-12fd90dcc30e") },
                    { new Guid("80e32159-612c-4f6d-9eee-fe91afe5f619"), "Aspiring actor, experienced in commercials and short films.", new DateTime(2001, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Siyana", 2, true, false, "Petrova", "Sisi", "/img/profile/default-profile.png", new Guid("a77cc134-28f3-45d3-8e46-ba041a5371d0") },
                    { new Guid("dd6182e2-0e3b-4a74-8386-fdcfa8522a1f"), "Film actor with a focus on action and thrillers.", new DateTime(1995, 9, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Georgi", 1, true, false, "Ivanov", null, "/img/profile/default-profile.png", new Guid("47a394d7-2ddb-4511-8355-a489dc8ea3f5") }
                });

            migrationBuilder.InsertData(
                table: "CrewMembers",
                columns: new[] { "Id", "Biography", "FirstName", "IsActive", "IsDeleted", "LastName", "Nickname", "ProfileImagePath", "UserId" },
                values: new object[,]
                {
                    { new Guid("5bc88034-ac2f-49cf-b39c-20a54ae09c50"), "Production coordinator with experience in budgeting, call sheets and logistics.", "Maria", true, false, "Georgieva", "Maya", "/img/profile/default-profile.png", new Guid("f704cf4b-b01a-4b5d-a052-7043caffd93e") },
                    { new Guid("8019d5fc-23a7-49d8-80ac-b3edd8a7e398"), "Director of Photography focused on natural light and handheld storytelling.", "Alex", true, false, "Petrov", "AP/DP", "/img/profile/default-profile.png", new Guid("a1093e45-6993-4d59-8e3f-4fe0b29e7dc5") },
                    { new Guid("ec228657-ad62-4e57-a749-8becaccce6dd"), "Sound recordist and boom operator. Location sound, ADR planning, and on-set workflow.", "Ivan", true, false, "Dimitrov", null, "/img/profile/default-profile.png", new Guid("5150238b-cd37-482d-ab11-ef66bae0128f") }
                });

            migrationBuilder.InsertData(
                table: "ProductionsAssets",
                columns: new[] { "Id", "AssetType", "Description", "FilePath", "ProductionId", "Title", "UploadedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-cccc-cccc-cccc-111111111111"), 1, "Basic scene coverage and camera beats.", "/img/assets/default-asset.png", new Guid("819f7100-5b1d-4eee-8b82-8e4084ef0928"), "Storyboard v1", new DateTime(2026, 3, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-cccc-cccc-cccc-222222222222"), 2, "Lighting references and overall vibe.", "/img/assets/default-asset.png", new Guid("819f7100-5b1d-4eee-8b82-8e4084ef0928"), "Moodboard - Visual Tone", new DateTime(2026, 3, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("33333333-cccc-cccc-cccc-333333333333"), 3, "Primary palette for day interiors.", "/img/assets/default-asset.png", new Guid("819f7100-5b1d-4eee-8b82-8e4084ef0928"), "Color Palette - Day Scenes", new DateTime(2026, 3, 2, 11, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("44444444-cccc-cccc-cccc-444444444444"), 4, "Reference stills for framing and composition.", "/img/assets/default-asset.png", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d01"), "Reference Frames Pack", new DateTime(2026, 3, 2, 11, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("55555555-cccc-cccc-cccc-555555555555"), 1, "Action sequence beats and movement.", "/img/assets/default-asset.png", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d01"), "Storyboard - Chase Sequence", new DateTime(2026, 3, 3, 9, 40, 0, 0, DateTimeKind.Utc) },
                    { new Guid("66666666-cccc-cccc-cccc-666666666666"), 99, "Draft PDF with lens & shot notes.", "/img/assets/default-asset.png", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d01"), "Shot List Draft", new DateTime(2026, 3, 3, 9, 40, 0, 0, DateTimeKind.Utc) },
                    { new Guid("77777777-cccc-cccc-cccc-777777777777"), 2, "Neon, fog, wet streets.", "/img/assets/default-asset.png", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d02"), "Moodboard - Night Atmosphere", new DateTime(2026, 3, 3, 9, 40, 0, 0, DateTimeKind.Utc) },
                    { new Guid("88888888-cccc-cccc-cccc-888888888888"), 3, "Cold shadows with warm practicals.", "/img/assets/default-asset.png", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d02"), "Color Palette - Night Scenes", new DateTime(2026, 3, 4, 14, 5, 0, 0, DateTimeKind.Utc) },
                    { new Guid("99999999-cccc-cccc-cccc-999999999999"), 4, "Props and set dressing ideas.", "/img/assets/default-asset.png", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d02"), "Set Dressing References", new DateTime(2026, 3, 4, 14, 5, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aaaaaaaa-cccc-cccc-cccc-aaaaaaaaaaaa"), 1, "Final sequence beats and transitions.", "/img/assets/default-asset.png", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d03"), "Storyboard - Finale", new DateTime(2026, 3, 4, 14, 5, 0, 0, DateTimeKind.Utc) },
                    { new Guid("bbbbbbbb-cccc-cccc-cccc-bbbbbbbbbbbb"), 99, "Scouting photos for final location.", "/img/assets/default-asset.png", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d03"), "Location Photos", new DateTime(2026, 3, 4, 14, 5, 0, 0, DateTimeKind.Utc) },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), 4, "Wardrobe references for key characters.", "/img/assets/default-asset.png", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d03"), "Costume References", new DateTime(2026, 3, 4, 14, 5, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Scenes",
                columns: new[] { "Id", "Description", "Location", "ProductionId", "SceneName", "SceneNumber", "SceneType" },
                values: new object[,]
                {
                    { new Guid("11111111-bbbb-bbbb-bbbb-111111111111"), "A tense meeting sets the stakes for the story.", "Downtown Coffee Shop", new Guid("819f7100-5b1d-4eee-8b82-8e4084ef0928"), "Coffee Shop Conversation", 1, 1 },
                    { new Guid("22222222-bbbb-bbbb-bbbb-222222222222"), "Fast-paced pursuit through the city streets.", "Main Boulevard", new Guid("819f7100-5b1d-4eee-8b82-8e4084ef0928"), "Street Chase", 2, 2 },
                    { new Guid("33333333-bbbb-bbbb-bbbb-333333333333"), "Key evidence is discovered.", "Character A Apartment", new Guid("819f7100-5b1d-4eee-8b82-8e4084ef0928"), "Apartment Reveal", 3, 3 },
                    { new Guid("44444444-bbbb-bbbb-bbbb-444444444444"), "A suspect breaks under pressure.", "Police Station", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d01"), "Interrogation Room", 1, 1 },
                    { new Guid("55555555-bbbb-bbbb-bbbb-555555555555"), "Conflict escalates with a dramatic skyline backdrop.", "Office Building Rooftop", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d01"), "Rooftop Argument", 2, 2 },
                    { new Guid("66666666-bbbb-bbbb-bbbb-666666666666"), "A mysterious figure appears in the shadows.", "Old Town Alley", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d02"), "Night Alley Encounter", 1, 2 },
                    { new Guid("77777777-bbbb-bbbb-bbbb-777777777777"), "The team plans the next move.", "Garage Workshop", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d02"), "Workshop Planning", 2, 1 },
                    { new Guid("88888888-bbbb-bbbb-bbbb-888888888888"), "Emotional turning point for the lead character.", "City Hospital", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d03"), "Hospital Corridor", 1, 1 },
                    { new Guid("99999999-bbbb-bbbb-bbbb-999999999999"), "Two characters reconcile after a fallout.", "Central Park", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d03"), "Park Reunion", 2, 2 },
                    { new Guid("aaaaaaaa-bbbb-bbbb-bbbb-aaaaaaaaaaaa"), "Closing moment with a hopeful sunrise.", "Cliffside Overlook", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d03"), "Finale: Sunrise Cliff", 3, 2 }
                });

            migrationBuilder.InsertData(
                table: "Scripts",
                columns: new[] { "Id", "LastEditedAt", "ProductionId", "RevisionType", "StageType", "Title" },
                values: new object[,]
                {
                    { new Guid("11111111-2222-3333-4444-555555555555"), new DateTime(2026, 3, 26, 13, 22, 5, 0, DateTimeKind.Utc), new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d01"), null, 2, "Midnight Dreams — Draft" },
                    { new Guid("22222222-3333-4444-5555-666666666666"), new DateTime(2026, 3, 26, 13, 22, 5, 0, DateTimeKind.Utc), new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d02"), 3, 7, "Paper Planes — Shooting Script" },
                    { new Guid("33333333-4444-5555-6666-777777777777"), new DateTime(2026, 3, 26, 13, 22, 5, 0, DateTimeKind.Utc), new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d03"), 2, 8, "The Last Take — Blue Revision" },
                    { new Guid("36ca9b58-d902-4195-bf80-2e6518ad3c6d"), new DateTime(2026, 3, 26, 13, 22, 5, 0, DateTimeKind.Utc), new Guid("819f7100-5b1d-4eee-8b82-8e4084ef0928"), null, 2, "Test Screenplay" },
                    { new Guid("44444444-5555-6666-7777-888888888888"), new DateTime(2026, 3, 26, 13, 22, 5, 0, DateTimeKind.Utc), new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d04"), null, 1, "Neon Skyline — Outline" },
                    { new Guid("55555555-6666-7777-8888-999999999999"), new DateTime(2026, 3, 26, 13, 22, 5, 0, DateTimeKind.Utc), new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d05"), 4, 4, "Wild Tracks — Polish" }
                });

            migrationBuilder.InsertData(
                table: "ShootingDays",
                columns: new[] { "Id", "Date", "Notes", "ProductionId" },
                values: new object[,]
                {
                    { new Guid("10000000-aaaa-aaaa-aaaa-100000000001"), new DateTime(2026, 3, 20, 8, 0, 0, 0, DateTimeKind.Utc), "Day 1: Coffee shop scene coverage.", new Guid("819f7100-5b1d-4eee-8b82-8e4084ef0928") },
                    { new Guid("10000000-aaaa-aaaa-aaaa-100000000002"), new DateTime(2026, 3, 21, 8, 0, 0, 0, DateTimeKind.Utc), "Day 2: Pickups and insert shots.", new Guid("819f7100-5b1d-4eee-8b82-8e4084ef0928") },
                    { new Guid("20000000-aaaa-aaaa-aaaa-200000000001"), new DateTime(2026, 3, 23, 8, 0, 0, 0, DateTimeKind.Utc), "Night exterior sequence.", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d01") },
                    { new Guid("20000000-aaaa-aaaa-aaaa-200000000002"), new DateTime(2026, 3, 24, 8, 0, 0, 0, DateTimeKind.Utc), "Studio interiors + dialogue scenes.", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d01") },
                    { new Guid("30000000-aaaa-aaaa-aaaa-300000000001"), new DateTime(2026, 3, 26, 8, 0, 0, 0, DateTimeKind.Utc), "School hallway + classroom scenes.", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d02") },
                    { new Guid("40000000-aaaa-aaaa-aaaa-400000000001"), new DateTime(2026, 3, 28, 8, 0, 0, 0, DateTimeKind.Utc), "Stage scenes and rehearsal coverage.", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d03") },
                    { new Guid("40000000-aaaa-aaaa-aaaa-400000000002"), new DateTime(2026, 3, 29, 8, 0, 0, 0, DateTimeKind.Utc), "Close-ups, alt takes, safety shots.", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d03") },
                    { new Guid("50000000-aaaa-aaaa-aaaa-500000000001"), new DateTime(2026, 3, 31, 8, 0, 0, 0, DateTimeKind.Utc), "City night montage (neon locations).", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d04") },
                    { new Guid("60000000-aaaa-aaaa-aaaa-600000000001"), new DateTime(2026, 4, 2, 8, 0, 0, 0, DateTimeKind.Utc), "Outdoor travel sequence.", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d05") },
                    { new Guid("70000000-aaaa-aaaa-aaaa-700000000001"), new DateTime(2026, 4, 4, 8, 0, 0, 0, DateTimeKind.Utc), "Planned shoot (project currently on hold).", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d06") },
                    { new Guid("80000000-aaaa-aaaa-aaaa-800000000001"), new DateTime(2026, 4, 6, 8, 0, 0, 0, DateTimeKind.Utc), "Reshoot day: continuity fixes + inserts.", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d07") },
                    { new Guid("90000000-aaaa-aaaa-aaaa-900000000001"), new DateTime(2026, 4, 8, 8, 0, 0, 0, DateTimeKind.Utc), "B-roll day for post-production needs.", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d08") },
                    { new Guid("a0000000-aaaa-aaaa-aaaa-a00000000001"), new DateTime(2026, 4, 10, 8, 0, 0, 0, DateTimeKind.Utc), "Press / promo shoot day.", new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d09") }
                });

            migrationBuilder.InsertData(
                table: "CrewSkills",
                columns: new[] { "Id", "CrewMemberId", "RoleType" },
                values: new object[,]
                {
                    { new Guid("a0000000-0000-0000-0000-000000000001"), new Guid("8019d5fc-23a7-49d8-80ac-b3edd8a7e398"), 1 },
                    { new Guid("a0000000-0000-0000-0000-000000000002"), new Guid("8019d5fc-23a7-49d8-80ac-b3edd8a7e398"), 2 },
                    { new Guid("a0000000-0000-0000-0000-000000000003"), new Guid("8019d5fc-23a7-49d8-80ac-b3edd8a7e398"), 20 },
                    { new Guid("a0000000-0000-0000-0000-000000000004"), new Guid("8019d5fc-23a7-49d8-80ac-b3edd8a7e398"), 21 },
                    { new Guid("a0000000-0000-0000-0000-000000000005"), new Guid("5bc88034-ac2f-49cf-b39c-20a54ae09c50"), 50 },
                    { new Guid("a0000000-0000-0000-0000-000000000006"), new Guid("5bc88034-ac2f-49cf-b39c-20a54ae09c50"), 51 },
                    { new Guid("a0000000-0000-0000-0000-000000000007"), new Guid("5bc88034-ac2f-49cf-b39c-20a54ae09c50"), 60 },
                    { new Guid("a0000000-0000-0000-0000-000000000008"), new Guid("5bc88034-ac2f-49cf-b39c-20a54ae09c50"), 62 },
                    { new Guid("a0000000-0000-0000-0000-000000000009"), new Guid("ec228657-ad62-4e57-a749-8becaccce6dd"), 70 },
                    { new Guid("a0000000-0000-0000-0000-000000000010"), new Guid("ec228657-ad62-4e57-a749-8becaccce6dd"), 71 },
                    { new Guid("a0000000-0000-0000-0000-000000000011"), new Guid("ec228657-ad62-4e57-a749-8becaccce6dd"), 80 },
                    { new Guid("a0000000-0000-0000-0000-000000000012"), new Guid("ec228657-ad62-4e57-a749-8becaccce6dd"), 82 }
                });

            migrationBuilder.InsertData(
                table: "ProductionsCastMembers",
                columns: new[] { "CastMemberId", "ProductionId", "Role" },
                values: new object[,]
                {
                    { new Guid("80e32159-612c-4f6d-9eee-fe91afe5f619"), new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d01"), "Lead Actress" },
                    { new Guid("076fd414-bd51-44a5-9d19-1f0a321e4d8e"), new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d02"), "Private Investigator" },
                    { new Guid("dd6182e2-0e3b-4a74-8386-fdcfa8522a1f"), new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d02"), "Antagonist" },
                    { new Guid("dd6182e2-0e3b-4a74-8386-fdcfa8522a1f"), new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d03"), "Supporting Role" },
                    { new Guid("076fd414-bd51-44a5-9d19-1f0a321e4d8e"), new Guid("819f7100-5b1d-4eee-8b82-8e4084ef0928"), "Detective Ivanov" },
                    { new Guid("dd6182e2-0e3b-4a74-8386-fdcfa8522a1f"), new Guid("819f7100-5b1d-4eee-8b82-8e4084ef0928"), "Witness" }
                });

            migrationBuilder.InsertData(
                table: "ProductionsCrewMembers",
                columns: new[] { "CrewMemberId", "ProductionId", "RoleType" },
                values: new object[,]
                {
                    { new Guid("5bc88034-ac2f-49cf-b39c-20a54ae09c50"), new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d03"), 82 },
                    { new Guid("ec228657-ad62-4e57-a749-8becaccce6dd"), new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d03"), 71 },
                    { new Guid("5bc88034-ac2f-49cf-b39c-20a54ae09c50"), new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d05"), 21 },
                    { new Guid("8019d5fc-23a7-49d8-80ac-b3edd8a7e398"), new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d06"), 1 },
                    { new Guid("5bc88034-ac2f-49cf-b39c-20a54ae09c50"), new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d09"), 20 },
                    { new Guid("8019d5fc-23a7-49d8-80ac-b3edd8a7e398"), new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d09"), 1 },
                    { new Guid("ec228657-ad62-4e57-a749-8becaccce6dd"), new Guid("2a6d6e6d-5a3a-4b7c-9e9a-0b0f5e5c4d09"), 70 }
                });

            migrationBuilder.InsertData(
                table: "ScenesCastMembers",
                columns: new[] { "CastMemberId", "SceneId", "Role" },
                values: new object[,]
                {
                    { new Guid("076fd414-bd51-44a5-9d19-1f0a321e4d8e"), new Guid("11111111-bbbb-bbbb-bbbb-111111111111"), "Detective Ivanov" },
                    { new Guid("dd6182e2-0e3b-4a74-8386-fdcfa8522a1f"), new Guid("11111111-bbbb-bbbb-bbbb-111111111111"), "Witness" },
                    { new Guid("076fd414-bd51-44a5-9d19-1f0a321e4d8e"), new Guid("22222222-bbbb-bbbb-bbbb-222222222222"), "Detective Ivanov" },
                    { new Guid("80e32159-612c-4f6d-9eee-fe91afe5f619"), new Guid("44444444-bbbb-bbbb-bbbb-444444444444"), "Lead Actress" },
                    { new Guid("076fd414-bd51-44a5-9d19-1f0a321e4d8e"), new Guid("66666666-bbbb-bbbb-bbbb-666666666666"), "Private Investigator" },
                    { new Guid("dd6182e2-0e3b-4a74-8386-fdcfa8522a1f"), new Guid("66666666-bbbb-bbbb-bbbb-666666666666"), "Antagonist" },
                    { new Guid("dd6182e2-0e3b-4a74-8386-fdcfa8522a1f"), new Guid("88888888-bbbb-bbbb-bbbb-888888888888"), "Supporting Role" }
                });

            migrationBuilder.InsertData(
                table: "ScenesCrewMembers",
                columns: new[] { "CrewMemberId", "SceneId", "RoleType" },
                values: new object[,]
                {
                    { new Guid("5bc88034-ac2f-49cf-b39c-20a54ae09c50"), new Guid("11111111-bbbb-bbbb-bbbb-111111111111"), 20 },
                    { new Guid("8019d5fc-23a7-49d8-80ac-b3edd8a7e398"), new Guid("11111111-bbbb-bbbb-bbbb-111111111111"), 1 },
                    { new Guid("ec228657-ad62-4e57-a749-8becaccce6dd"), new Guid("11111111-bbbb-bbbb-bbbb-111111111111"), 70 },
                    { new Guid("5bc88034-ac2f-49cf-b39c-20a54ae09c50"), new Guid("22222222-bbbb-bbbb-bbbb-222222222222"), 21 },
                    { new Guid("8019d5fc-23a7-49d8-80ac-b3edd8a7e398"), new Guid("22222222-bbbb-bbbb-bbbb-222222222222"), 1 },
                    { new Guid("5bc88034-ac2f-49cf-b39c-20a54ae09c50"), new Guid("33333333-bbbb-bbbb-bbbb-333333333333"), 22 },
                    { new Guid("ec228657-ad62-4e57-a749-8becaccce6dd"), new Guid("33333333-bbbb-bbbb-bbbb-333333333333"), 71 },
                    { new Guid("5bc88034-ac2f-49cf-b39c-20a54ae09c50"), new Guid("44444444-bbbb-bbbb-bbbb-444444444444"), 20 },
                    { new Guid("8019d5fc-23a7-49d8-80ac-b3edd8a7e398"), new Guid("44444444-bbbb-bbbb-bbbb-444444444444"), 1 },
                    { new Guid("5bc88034-ac2f-49cf-b39c-20a54ae09c50"), new Guid("55555555-bbbb-bbbb-bbbb-555555555555"), 30 },
                    { new Guid("ec228657-ad62-4e57-a749-8becaccce6dd"), new Guid("55555555-bbbb-bbbb-bbbb-555555555555"), 72 },
                    { new Guid("5bc88034-ac2f-49cf-b39c-20a54ae09c50"), new Guid("66666666-bbbb-bbbb-bbbb-666666666666"), 20 },
                    { new Guid("8019d5fc-23a7-49d8-80ac-b3edd8a7e398"), new Guid("66666666-bbbb-bbbb-bbbb-666666666666"), 7 },
                    { new Guid("8019d5fc-23a7-49d8-80ac-b3edd8a7e398"), new Guid("77777777-bbbb-bbbb-bbbb-777777777777"), 2 },
                    { new Guid("ec228657-ad62-4e57-a749-8becaccce6dd"), new Guid("77777777-bbbb-bbbb-bbbb-777777777777"), 70 },
                    { new Guid("5bc88034-ac2f-49cf-b39c-20a54ae09c50"), new Guid("88888888-bbbb-bbbb-bbbb-888888888888"), 21 },
                    { new Guid("ec228657-ad62-4e57-a749-8becaccce6dd"), new Guid("88888888-bbbb-bbbb-bbbb-888888888888"), 71 },
                    { new Guid("5bc88034-ac2f-49cf-b39c-20a54ae09c50"), new Guid("99999999-bbbb-bbbb-bbbb-999999999999"), 20 },
                    { new Guid("8019d5fc-23a7-49d8-80ac-b3edd8a7e398"), new Guid("99999999-bbbb-bbbb-bbbb-999999999999"), 1 },
                    { new Guid("8019d5fc-23a7-49d8-80ac-b3edd8a7e398"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-aaaaaaaaaaaa"), 7 },
                    { new Guid("ec228657-ad62-4e57-a749-8becaccce6dd"), new Guid("aaaaaaaa-bbbb-bbbb-bbbb-aaaaaaaaaaaa"), 73 }
                });

            migrationBuilder.InsertData(
                table: "ScriptBlocks",
                columns: new[] { "Id", "BlockType", "Content", "CreatedAt", "LastModifiedAt", "Metadata", "OrderIndex", "ScriptId" },
                values: new object[,]
                {
                    { new Guid("3bed994a-cbee-4d60-b22f-a922b82eb841"), 7, "I'll have a double espresso. Make it strong.", new DateTime(2026, 3, 9, 13, 22, 5, 0, DateTimeKind.Utc), new DateTime(2026, 3, 9, 13, 22, 5, 0, DateTimeKind.Utc), null, 3, new Guid("36ca9b58-d902-4195-bf80-2e6518ad3c6d") },
                    { new Guid("65a5a61f-204d-4274-be48-bf4b440ff6a1"), 1, "INT. COFFEE SHOP - DAY", new DateTime(2026, 3, 9, 13, 22, 5, 0, DateTimeKind.Utc), new DateTime(2026, 3, 9, 13, 22, 5, 0, DateTimeKind.Utc), null, 0, new Guid("36ca9b58-d902-4195-bf80-2e6518ad3c6d") },
                    { new Guid("befd3f37-d237-4e46-8a21-e08704c6ef00"), 4, "JOHN", new DateTime(2026, 3, 9, 13, 22, 5, 0, DateTimeKind.Utc), new DateTime(2026, 3, 9, 13, 22, 5, 0, DateTimeKind.Utc), null, 2, new Guid("36ca9b58-d902-4195-bf80-2e6518ad3c6d") },
                    { new Guid("c5cc8272-c35d-4b4b-bb31-137df7fe86d5"), 3, "A bustling morning crowd. Steam rises from espresso machines.", new DateTime(2026, 3, 9, 13, 22, 5, 0, DateTimeKind.Utc), new DateTime(2026, 3, 9, 13, 22, 5, 0, DateTimeKind.Utc), null, 1, new Guid("36ca9b58-d902-4195-bf80-2e6518ad3c6d") }
                });

            migrationBuilder.InsertData(
                table: "ShootingDaysScenes",
                columns: new[] { "Id", "Order", "SceneId", "ShootingDayId" },
                values: new object[,]
                {
                    { new Guid("b1000000-cccc-cccc-cccc-b10000000001"), 1, new Guid("11111111-bbbb-bbbb-bbbb-111111111111"), new Guid("10000000-aaaa-aaaa-aaaa-100000000001") },
                    { new Guid("b1000000-cccc-cccc-cccc-b10000000002"), 2, new Guid("22222222-bbbb-bbbb-bbbb-222222222222"), new Guid("10000000-aaaa-aaaa-aaaa-100000000001") },
                    { new Guid("b1000000-cccc-cccc-cccc-b10000000003"), 1, new Guid("33333333-bbbb-bbbb-bbbb-333333333333"), new Guid("10000000-aaaa-aaaa-aaaa-100000000002") },
                    { new Guid("b2000000-cccc-cccc-cccc-b20000000001"), 1, new Guid("44444444-bbbb-bbbb-bbbb-444444444444"), new Guid("20000000-aaaa-aaaa-aaaa-200000000001") },
                    { new Guid("b2000000-cccc-cccc-cccc-b20000000002"), 1, new Guid("55555555-bbbb-bbbb-bbbb-555555555555"), new Guid("20000000-aaaa-aaaa-aaaa-200000000002") },
                    { new Guid("b3000000-cccc-cccc-cccc-b30000000001"), 1, new Guid("66666666-bbbb-bbbb-bbbb-666666666666"), new Guid("30000000-aaaa-aaaa-aaaa-300000000001") },
                    { new Guid("b3000000-cccc-cccc-cccc-b30000000002"), 2, new Guid("77777777-bbbb-bbbb-bbbb-777777777777"), new Guid("30000000-aaaa-aaaa-aaaa-300000000001") },
                    { new Guid("b4000000-cccc-cccc-cccc-b40000000001"), 1, new Guid("88888888-bbbb-bbbb-bbbb-888888888888"), new Guid("40000000-aaaa-aaaa-aaaa-400000000001") },
                    { new Guid("b4000000-cccc-cccc-cccc-b40000000002"), 2, new Guid("99999999-bbbb-bbbb-bbbb-999999999999"), new Guid("40000000-aaaa-aaaa-aaaa-400000000001") },
                    { new Guid("b4000000-cccc-cccc-cccc-b40000000003"), 1, new Guid("aaaaaaaa-bbbb-bbbb-bbbb-aaaaaaaaaaaa"), new Guid("40000000-aaaa-aaaa-aaaa-400000000002") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CastMembers_UserId",
                table: "CastMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CrewMembers_UserId",
                table: "CrewMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CrewSkills_CrewMemberId",
                table: "CrewSkills",
                column: "CrewMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionsAssets_ProductionId_AssetType",
                table: "ProductionsAssets",
                columns: new[] { "ProductionId", "AssetType" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductionsCastMembers_CastMemberId",
                table: "ProductionsCastMembers",
                column: "CastMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionsCrewMembers_CrewMemberId",
                table: "ProductionsCrewMembers",
                column: "CrewMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Scenes_ProductionId_SceneNumber",
                table: "Scenes",
                columns: new[] { "ProductionId", "SceneNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_ScenesCastMembers_CastMemberId",
                table: "ScenesCastMembers",
                column: "CastMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenesCrewMembers_CrewMemberId",
                table: "ScenesCrewMembers",
                column: "CrewMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ScriptBlocks_ScriptId_OrderIndex",
                table: "ScriptBlocks",
                columns: new[] { "ScriptId", "OrderIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scripts_ProductionId",
                table: "Scripts",
                column: "ProductionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShootingDays_ProductionId",
                table: "ShootingDays",
                column: "ProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_ShootingDaysScenes_SceneId",
                table: "ShootingDaysScenes",
                column: "SceneId");

            migrationBuilder.CreateIndex(
                name: "IX_ShootingDaysScenes_ShootingDayId_SceneId",
                table: "ShootingDaysScenes",
                columns: new[] { "ShootingDayId", "SceneId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CrewSkills");

            migrationBuilder.DropTable(
                name: "ProductionsAssets");

            migrationBuilder.DropTable(
                name: "ProductionsCastMembers");

            migrationBuilder.DropTable(
                name: "ProductionsCrewMembers");

            migrationBuilder.DropTable(
                name: "ScenesCastMembers");

            migrationBuilder.DropTable(
                name: "ScenesCrewMembers");

            migrationBuilder.DropTable(
                name: "ScriptBlocks");

            migrationBuilder.DropTable(
                name: "ShootingDaysScenes");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CastMembers");

            migrationBuilder.DropTable(
                name: "CrewMembers");

            migrationBuilder.DropTable(
                name: "Scripts");

            migrationBuilder.DropTable(
                name: "Scenes");

            migrationBuilder.DropTable(
                name: "ShootingDays");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Productions");
        }
    }
}
