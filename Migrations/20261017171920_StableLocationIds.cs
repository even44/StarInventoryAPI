using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarInventoryAPI.Migrations
{
    /// <inheritdoc />
    public partial class StableLocationIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "StarLocations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceType",
                table: "StarLocations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StarLocations_SourceType_SourceId",
                table: "StarLocations",
                columns: new[] { "SourceType", "SourceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StarLocations_SourceType_SourceId",
                table: "StarLocations");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "StarLocations");

            migrationBuilder.DropColumn(
                name: "SourceType",
                table: "StarLocations");
        }
    }
}
