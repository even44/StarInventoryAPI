using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarInventoryAPI.Migrations
{
    /// <inheritdoc />
    public partial class StarLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonalItems_StarLocation_LocationId",
                table: "PersonalItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StarLocation",
                table: "StarLocation");

            migrationBuilder.RenameTable(
                name: "StarLocation",
                newName: "StarLocations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StarLocations",
                table: "StarLocations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalItems_StarLocations_LocationId",
                table: "PersonalItems",
                column: "LocationId",
                principalTable: "StarLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonalItems_StarLocations_LocationId",
                table: "PersonalItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StarLocations",
                table: "StarLocations");

            migrationBuilder.RenameTable(
                name: "StarLocations",
                newName: "StarLocation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StarLocation",
                table: "StarLocation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalItems_StarLocation_LocationId",
                table: "PersonalItems",
                column: "LocationId",
                principalTable: "StarLocation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
