using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarInventoryAPI.Migrations
{
    /// <inheritdoc />
    public partial class cities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UexCities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StarSystemId = table.Column<int>(type: "int", nullable: false),
                    PlanetId = table.Column<int>(type: "int", nullable: false),
                    OrbitId = table.Column<int>(type: "int", nullable: false),
                    MoonId = table.Column<int>(type: "int", nullable: false),
                    FactionId = table.Column<int>(type: "int", nullable: false),
                    JurisdictionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsAvailable = table.Column<int>(type: "int", nullable: false),
                    IsAvailableLive = table.Column<int>(type: "int", nullable: false),
                    IsVisible = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<int>(type: "int", nullable: false),
                    IsMonitored = table.Column<int>(type: "int", nullable: false),
                    IsArmistice = table.Column<int>(type: "int", nullable: false),
                    IsLandable = table.Column<int>(type: "int", nullable: false),
                    IsDecommissioned = table.Column<int>(type: "int", nullable: false),
                    HasQuantumMarker = table.Column<int>(type: "int", nullable: false),
                    HasTradeTerminal = table.Column<int>(type: "int", nullable: false),
                    HasHabitation = table.Column<int>(type: "int", nullable: false),
                    HasRefinery = table.Column<int>(type: "int", nullable: false),
                    HasCargoCenter = table.Column<int>(type: "int", nullable: false),
                    HasClinic = table.Column<int>(type: "int", nullable: false),
                    HasFood = table.Column<int>(type: "int", nullable: false),
                    HasShops = table.Column<int>(type: "int", nullable: false),
                    HasRefuel = table.Column<int>(type: "int", nullable: false),
                    HasRepair = table.Column<int>(type: "int", nullable: false),
                    HasGravity = table.Column<int>(type: "int", nullable: false),
                    HasLoadingDock = table.Column<int>(type: "int", nullable: false),
                    HasDockingPort = table.Column<int>(type: "int", nullable: false),
                    HasFreightElevator = table.Column<int>(type: "int", nullable: false),
                    PadTypes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateAdded = table.Column<int>(type: "int", nullable: false),
                    DateModified = table.Column<int>(type: "int", nullable: false),
                    StarSystemName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PlanetName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrbitName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FactionName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JurisdictionName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UexCities", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UexCities");
        }
    }
}
