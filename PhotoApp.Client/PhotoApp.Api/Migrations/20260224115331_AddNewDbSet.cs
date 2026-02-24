using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddNewDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_ProjectWebDesign_Projects_ProjectId",
                table: "Project_ProjectWebDesign");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_ProjectWebDesign_Projects_ProjectId1",
                table: "Project_ProjectWebDesign");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_ProjectWebDesign_WebDesignes_ProjectWebDesignId",
                table: "Project_ProjectWebDesign");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_ProjectWebDesign_WebDesignes_ProjectWebDesignId1",
                table: "Project_ProjectWebDesign");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Project_ProjectWebDesign",
                table: "Project_ProjectWebDesign");

            migrationBuilder.RenameTable(
                name: "Project_ProjectWebDesign",
                newName: "Project_ProjectWebDesigns");

            migrationBuilder.RenameIndex(
                name: "IX_Project_ProjectWebDesign_ProjectWebDesignId1",
                table: "Project_ProjectWebDesigns",
                newName: "IX_Project_ProjectWebDesigns_ProjectWebDesignId1");

            migrationBuilder.RenameIndex(
                name: "IX_Project_ProjectWebDesign_ProjectWebDesignId",
                table: "Project_ProjectWebDesigns",
                newName: "IX_Project_ProjectWebDesigns_ProjectWebDesignId");

            migrationBuilder.RenameIndex(
                name: "IX_Project_ProjectWebDesign_ProjectId1",
                table: "Project_ProjectWebDesigns",
                newName: "IX_Project_ProjectWebDesigns_ProjectId1");

            migrationBuilder.RenameIndex(
                name: "IX_Project_ProjectWebDesign_ProjectId_Device",
                table: "Project_ProjectWebDesigns",
                newName: "IX_Project_ProjectWebDesigns_ProjectId_Device");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Project_ProjectWebDesigns",
                table: "Project_ProjectWebDesigns",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_ProjectWebDesigns_Projects_ProjectId",
                table: "Project_ProjectWebDesigns",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_ProjectWebDesigns_Projects_ProjectId1",
                table: "Project_ProjectWebDesigns",
                column: "ProjectId1",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_ProjectWebDesigns_WebDesignes_ProjectWebDesignId",
                table: "Project_ProjectWebDesigns",
                column: "ProjectWebDesignId",
                principalTable: "WebDesignes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_ProjectWebDesigns_WebDesignes_ProjectWebDesignId1",
                table: "Project_ProjectWebDesigns",
                column: "ProjectWebDesignId1",
                principalTable: "WebDesignes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_ProjectWebDesigns_Projects_ProjectId",
                table: "Project_ProjectWebDesigns");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_ProjectWebDesigns_Projects_ProjectId1",
                table: "Project_ProjectWebDesigns");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_ProjectWebDesigns_WebDesignes_ProjectWebDesignId",
                table: "Project_ProjectWebDesigns");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_ProjectWebDesigns_WebDesignes_ProjectWebDesignId1",
                table: "Project_ProjectWebDesigns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Project_ProjectWebDesigns",
                table: "Project_ProjectWebDesigns");

            migrationBuilder.RenameTable(
                name: "Project_ProjectWebDesigns",
                newName: "Project_ProjectWebDesign");

            migrationBuilder.RenameIndex(
                name: "IX_Project_ProjectWebDesigns_ProjectWebDesignId1",
                table: "Project_ProjectWebDesign",
                newName: "IX_Project_ProjectWebDesign_ProjectWebDesignId1");

            migrationBuilder.RenameIndex(
                name: "IX_Project_ProjectWebDesigns_ProjectWebDesignId",
                table: "Project_ProjectWebDesign",
                newName: "IX_Project_ProjectWebDesign_ProjectWebDesignId");

            migrationBuilder.RenameIndex(
                name: "IX_Project_ProjectWebDesigns_ProjectId1",
                table: "Project_ProjectWebDesign",
                newName: "IX_Project_ProjectWebDesign_ProjectId1");

            migrationBuilder.RenameIndex(
                name: "IX_Project_ProjectWebDesigns_ProjectId_Device",
                table: "Project_ProjectWebDesign",
                newName: "IX_Project_ProjectWebDesign_ProjectId_Device");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Project_ProjectWebDesign",
                table: "Project_ProjectWebDesign",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_ProjectWebDesign_Projects_ProjectId",
                table: "Project_ProjectWebDesign",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_ProjectWebDesign_Projects_ProjectId1",
                table: "Project_ProjectWebDesign",
                column: "ProjectId1",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_ProjectWebDesign_WebDesignes_ProjectWebDesignId",
                table: "Project_ProjectWebDesign",
                column: "ProjectWebDesignId",
                principalTable: "WebDesignes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_ProjectWebDesign_WebDesignes_ProjectWebDesignId1",
                table: "Project_ProjectWebDesign",
                column: "ProjectWebDesignId1",
                principalTable: "WebDesignes",
                principalColumn: "Id");
        }
    }
}
