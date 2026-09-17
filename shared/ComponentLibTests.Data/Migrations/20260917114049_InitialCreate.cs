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
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUsers", x => x.Id);
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
                name: "MatchResultStateChanges");

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
