using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarInventoryAPI.Migrations
{
    /// <inheritdoc />
    public partial class newinit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrgInventoryUsers",
                columns: table => new
                {
                    Username = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgInventoryUsers", x => x.Username);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PersonalItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UexIdentifier = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    IsSharedWithOrganization = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalItems", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StarLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StarLocations", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UexCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Section = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsGameRelated = table.Column<int>(type: "int", nullable: false),
                    IsMining = table.Column<int>(type: "int", nullable: false),
                    DateAdded = table.Column<int>(type: "int", nullable: false),
                    DateModified = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UexCategories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UexItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    parentId = table.Column<int>(type: "int", nullable: false),
                    categoryId = table.Column<int>(type: "int", nullable: false),
                    vehicleId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Section = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Category = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VehicleName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Size = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Uuid = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoreUrl = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_exclusive_pledge = table.Column<int>(type: "int", nullable: false),
                    is_exclusive_subscriber = table.Column<int>(type: "int", nullable: false),
                    is_exclusive_concierge = table.Column<int>(type: "int", nullable: false),
                    is_commodity = table.Column<int>(type: "int", nullable: false),
                    is_harvestable = table.Column<int>(type: "int", nullable: false),
                    Notification = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GameVersion = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateAdded = table.Column<int>(type: "int", nullable: false),
                    DateModified = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UexItem", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UexPois",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StarSystemId = table.Column<int>(type: "int", nullable: false),
                    PlanetId = table.Column<int>(type: "int", nullable: false),
                    OrbitId = table.Column<int>(type: "int", nullable: false),
                    MoonId = table.Column<int>(type: "int", nullable: false),
                    SpaceStationId = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    OutpostId = table.Column<int>(type: "int", nullable: false),
                    FactionId = table.Column<int>(type: "int", nullable: false),
                    JurisdictionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nickname = table.Column<string>(type: "longtext", nullable: false)
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
                    MoonName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SpaceStationName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OutpostName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CityName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FactionName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JurisdictionName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UexPois", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UexSpaceStations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StarSystemId = table.Column<int>(type: "int", nullable: false),
                    PlanetId = table.Column<int>(type: "int", nullable: false),
                    OrbitId = table.Column<int>(type: "int", nullable: false),
                    MoonId = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    FactionId = table.Column<int>(type: "int", nullable: false),
                    JurisdictionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nickname = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsAvailable = table.Column<int>(type: "int", nullable: false),
                    IsAvailableLive = table.Column<int>(type: "int", nullable: false),
                    IsVisible = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<int>(type: "int", nullable: false),
                    IsMonitored = table.Column<int>(type: "int", nullable: false),
                    IsArmistice = table.Column<int>(type: "int", nullable: false),
                    IsLandable = table.Column<int>(type: "int", nullable: false),
                    IsDecommissioned = table.Column<int>(type: "int", nullable: false),
                    IsLagrange = table.Column<int>(type: "int", nullable: false),
                    IsJumpPoint = table.Column<int>(type: "int", nullable: false),
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
                    CityName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FactionName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JurisdictionName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UexSpaceStations", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrgInventoryUsers");

            migrationBuilder.DropTable(
                name: "PersonalItems");

            migrationBuilder.DropTable(
                name: "StarLocations");

            migrationBuilder.DropTable(
                name: "UexCategories");

            migrationBuilder.DropTable(
                name: "UexItem");

            migrationBuilder.DropTable(
                name: "UexPois");

            migrationBuilder.DropTable(
                name: "UexSpaceStations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
