using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComponentLibTests.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
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
                    table.PrimaryKey("PK_ApplicationUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntityInternRiskLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RiskLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sort = table.Column<short>(type: "smallint", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityInternRiskLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntityMatchStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sort = table.Column<short>(type: "smallint", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityMatchStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntitySources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntitySources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfileRiskLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RiskLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sort = table.Column<short>(type: "smallint", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileRiskLevels", x => x.Id);
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
                        name: "FK_AspNetUserClaims_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_AspNetUserRoles_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityInternEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Entity = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EntityInternRiskLevelId = table.Column<int>(type: "int", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDateMatcher = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityInternEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityInternEntities_EntityInternRiskLevels_EntityInternRiskLevelId",
                        column: x => x.EntityInternRiskLevelId,
                        principalTable: "EntityInternRiskLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProfilesExtern",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Entity = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ProfileRiskLevelId = table.Column<int>(type: "int", nullable: true),
                    EntitySourceId = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfilesExtern", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfilesExtern_EntitySources_EntitySourceId",
                        column: x => x.EntitySourceId,
                        principalTable: "EntitySources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfilesExtern_ProfileRiskLevels_ProfileRiskLevelId",
                        column: x => x.ProfileRiskLevelId,
                        principalTable: "ProfileRiskLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EntityMatchResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityInternEntitiesId = table.Column<int>(type: "int", nullable: false),
                    ProfilesExternId = table.Column<int>(type: "int", nullable: false),
                    MatchingValue = table.Column<double>(type: "float", nullable: false),
                    EntityMatchStatusId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityMatchResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityMatchResults_EntityInternEntities_EntityInternEntitiesId",
                        column: x => x.EntityInternEntitiesId,
                        principalTable: "EntityInternEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityMatchResults_EntityMatchStatuses_EntityMatchStatusId",
                        column: x => x.EntityMatchStatusId,
                        principalTable: "EntityMatchStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityMatchResults_ProfilesExtern_ProfilesExternId",
                        column: x => x.ProfilesExternId,
                        principalTable: "ProfilesExtern",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatchResultStateChanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchResultId = table.Column<int>(type: "int", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NewMatchStatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchResultStateChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchResultStateChanges_ApplicationUsers_UpdateUserId",
                        column: x => x.UpdateUserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchResultStateChanges_EntityMatchResults_MatchResultId",
                        column: x => x.MatchResultId,
                        principalTable: "EntityMatchResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchResultStateChanges_EntityMatchStatuses_NewMatchStatusId",
                        column: x => x.NewMatchStatusId,
                        principalTable: "EntityMatchStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "ApplicationUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "ApplicationUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

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
                name: "IX_EntityInternEntities_Active",
                table: "EntityInternEntities",
                column: "Active");

            migrationBuilder.CreateIndex(
                name: "IX_EntityInternEntities_Entity",
                table: "EntityInternEntities",
                column: "Entity");

            migrationBuilder.CreateIndex(
                name: "IX_EntityInternEntities_EntityInternRiskLevelId",
                table: "EntityInternEntities",
                column: "EntityInternRiskLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityMatchResults_Created",
                table: "EntityMatchResults",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_EntityMatchResults_EntityInternEntitiesId",
                table: "EntityMatchResults",
                column: "EntityInternEntitiesId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityMatchResults_EntityMatchStatusId",
                table: "EntityMatchResults",
                column: "EntityMatchStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityMatchResults_MatchingValue",
                table: "EntityMatchResults",
                column: "MatchingValue");

            migrationBuilder.CreateIndex(
                name: "IX_EntityMatchResults_ProfilesExternId",
                table: "EntityMatchResults",
                column: "ProfilesExternId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchResultStateChanges_MatchResultId",
                table: "MatchResultStateChanges",
                column: "MatchResultId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchResultStateChanges_NewMatchStatusId",
                table: "MatchResultStateChanges",
                column: "NewMatchStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchResultStateChanges_UpdateDate",
                table: "MatchResultStateChanges",
                column: "UpdateDate");

            migrationBuilder.CreateIndex(
                name: "IX_MatchResultStateChanges_UpdateUserId",
                table: "MatchResultStateChanges",
                column: "UpdateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfilesExtern_EntitySourceId",
                table: "ProfilesExtern",
                column: "EntitySourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfilesExtern_LastName",
                table: "ProfilesExtern",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_ProfilesExtern_ProfileRiskLevelId",
                table: "ProfilesExtern",
                column: "ProfileRiskLevelId");
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
                name: "MatchResultStateChanges");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ApplicationUsers");

            migrationBuilder.DropTable(
                name: "EntityMatchResults");

            migrationBuilder.DropTable(
                name: "EntityInternEntities");

            migrationBuilder.DropTable(
                name: "EntityMatchStatuses");

            migrationBuilder.DropTable(
                name: "ProfilesExtern");

            migrationBuilder.DropTable(
                name: "EntityInternRiskLevels");

            migrationBuilder.DropTable(
                name: "EntitySources");

            migrationBuilder.DropTable(
                name: "ProfileRiskLevels");
        }
    }
}
