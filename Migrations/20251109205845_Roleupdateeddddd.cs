using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarInventoryAPI.Migrations
{
    /// <inheritdoc />
    public partial class Roleupdateeddddd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClaimString",
                table: "Roles",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Roles",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClaimString",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Roles");
        }
    }
}
