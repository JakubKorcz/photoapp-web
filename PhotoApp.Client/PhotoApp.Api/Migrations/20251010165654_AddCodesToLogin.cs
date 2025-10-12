using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoApp.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCodesToLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CodeExpiration",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "LoginCode",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeExpiration",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LoginCode",
                table: "Users");
        }
    }
}
