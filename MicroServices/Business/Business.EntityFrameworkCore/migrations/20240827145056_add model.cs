using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Business.migrations
{
    /// <inheritdoc />
    public partial class addmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MissionCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionCategory", x => x.Id);
                });
            
            migrationBuilder.CreateTable(
                name: "MissionTag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionTag", x => x.Id);
                });

            

            migrationBuilder.CreateTable(
                name: "MyFileInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyFileInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MissionPriority = table.Column<int>(type: "int", nullable: false),
                    ParentMissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MissionCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MissionStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MissionEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MissionFinishTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MissionBeforeEnd = table.Column<int>(type: "int", nullable: false),
                    MissionState = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mission_MissionCategory_MissionCategoryId",
                        column: x => x.MissionCategoryId,
                        principalTable: "MissionCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MissionCategoryI18N",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MissionCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MissionCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lang = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionCategoryI18N", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MissionCategoryI18N_MissionCategory_MissionCategoryId",
                        column: x => x.MissionCategoryId,
                        principalTable: "MissionCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MissionTagI18N",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MissionTagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MissionTagName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lang = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionTagI18N", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MissionTagI18N_MissionTag_MissionTagId",
                        column: x => x.MissionTagId,
                        principalTable: "MissionTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MissionI18N",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MissionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MissionDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lang = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionI18N", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MissionI18N_Mission_MissionId",
                        column: x => x.MissionId,
                        principalTable: "Mission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MissionPostTag",
                columns: table => new
                {
                    MissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MissionTagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissionPostTag", x => new { x.MissionId, x.MissionTagId });
                    table.ForeignKey(
                        name: "FK_MissionPostTag_MissionTag_MissionTagId",
                        column: x => x.MissionTagId,
                        principalTable: "MissionTag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MissionPostTag_Mission_MissionId",
                        column: x => x.MissionId,
                        principalTable: "Mission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mission_MissionCategoryId",
                table: "Mission",
                column: "MissionCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MissionCategoryI18N_MissionCategoryId",
                table: "MissionCategoryI18N",
                column: "MissionCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MissionI18N_MissionId",
                table: "MissionI18N",
                column: "MissionId");

            migrationBuilder.CreateIndex(
                name: "IX_MissionPostTag_MissionTagId",
                table: "MissionPostTag",
                column: "MissionTagId");

            migrationBuilder.CreateIndex(
                name: "IX_MissionTagI18N_MissionTagId",
                table: "MissionTagI18N",
                column: "MissionTagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MissionCategoryI18N");

            migrationBuilder.DropTable(
                name: "MissionCategoryView");

            migrationBuilder.DropTable(
                name: "MissionI18N");

            migrationBuilder.DropTable(
                name: "MissionPostTag");

            migrationBuilder.DropTable(
                name: "MissionTagI18N");

            migrationBuilder.DropTable(
                name: "MissionView");

            migrationBuilder.DropTable(
                name: "MyFileInfo");

            migrationBuilder.DropTable(
                name: "Mission");

            migrationBuilder.DropTable(
                name: "MissionTag");

            migrationBuilder.DropTable(
                name: "MissionCategory");
        }
    }
}
