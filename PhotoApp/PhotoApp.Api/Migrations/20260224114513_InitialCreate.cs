using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Folders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsHeadFolder = table.Column<bool>(type: "boolean", nullable: false),
                    ParentFolderId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Folders_Folders_ParentFolderId",
                        column: x => x.ParentFolderId,
                        principalTable: "Folders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    LoginCode = table.Column<int>(type: "integer", nullable: true),
                    CodeExpiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectName = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MainFolderId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Folders_MainFolderId",
                        column: x => x.MainFolderId,
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Projects_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Medias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Extension = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    IsLiked = table.Column<bool>(type: "boolean", nullable: false),
                    ParentFolderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medias_Folders_ParentFolderId",
                        column: x => x.ParentFolderId,
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Medias_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebDesignes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CoverPhotoId = table.Column<Guid>(type: "uuid", nullable: true),
                    FontFamily = table.Column<string>(type: "text", nullable: true),
                    FontSize = table.Column<int>(type: "integer", nullable: true),
                    FontWeight = table.Column<int>(type: "integer", nullable: true),
                    FontLayout = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebDesignes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebDesignes_Medias_CoverPhotoId",
                        column: x => x.CoverPhotoId,
                        principalTable: "Medias",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Project_ProjectWebDesign",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectWebDesignId = table.Column<Guid>(type: "uuid", nullable: false),
                    Device = table.Column<int>(type: "integer", nullable: false),
                    ProjectId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    ProjectWebDesignId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project_ProjectWebDesign", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Project_ProjectWebDesign_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Project_ProjectWebDesign_Projects_ProjectId1",
                        column: x => x.ProjectId1,
                        principalTable: "Projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Project_ProjectWebDesign_WebDesignes_ProjectWebDesignId",
                        column: x => x.ProjectWebDesignId,
                        principalTable: "WebDesignes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Project_ProjectWebDesign_WebDesignes_ProjectWebDesignId1",
                        column: x => x.ProjectWebDesignId1,
                        principalTable: "WebDesignes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Folders_ParentFolderId",
                table: "Folders",
                column: "ParentFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Medias_ParentFolderId",
                table: "Medias",
                column: "ParentFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Medias_ProjectId",
                table: "Medias",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectWebDesign_ProjectId_Device",
                table: "Project_ProjectWebDesign",
                columns: new[] { "ProjectId", "Device" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectWebDesign_ProjectId1",
                table: "Project_ProjectWebDesign",
                column: "ProjectId1");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectWebDesign_ProjectWebDesignId",
                table: "Project_ProjectWebDesign",
                column: "ProjectWebDesignId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectWebDesign_ProjectWebDesignId1",
                table: "Project_ProjectWebDesign",
                column: "ProjectWebDesignId1");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_MainFolderId",
                table: "Projects",
                column: "MainFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserId",
                table: "Projects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WebDesignes_CoverPhotoId",
                table: "WebDesignes",
                column: "CoverPhotoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Project_ProjectWebDesign");

            migrationBuilder.DropTable(
                name: "WebDesignes");

            migrationBuilder.DropTable(
                name: "Medias");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Folders");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
