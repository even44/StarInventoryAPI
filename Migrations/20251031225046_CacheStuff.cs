using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarInventoryAPI.Migrations
{
    /// <inheritdoc />
    public partial class CacheStuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonalItems_StarLocations_LocationId",
                table: "PersonalItems");

            migrationBuilder.DropIndex(
                name: "IX_PersonalItems_LocationId",
                table: "PersonalItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CacheItems",
                table: "CacheItems");

            migrationBuilder.RenameTable(
                name: "CacheItems",
                newName: "UexItem");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "UexItem",
                newName: "categoryId");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "UexItem",
                newName: "Slug");

            migrationBuilder.RenameColumn(
                name: "CategoryName",
                table: "UexItem",
                newName: "GameVersion");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "UexItem",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "UexItem",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "DateAdded",
                table: "UexItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DateModified",
                table: "UexItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Notification",
                table: "UexItem",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Section",
                table: "UexItem",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "UexItem",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "StoreUrl",
                table: "UexItem",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "VehicleName",
                table: "UexItem",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "is_commodity",
                table: "UexItem",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_exclusive_concierge",
                table: "UexItem",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_exclusive_pledge",
                table: "UexItem",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_exclusive_subscriber",
                table: "UexItem",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_harvestable",
                table: "UexItem",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "parentId",
                table: "UexItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "uuid",
                table: "UexItem",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "vehicleId",
                table: "UexItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UexItem",
                table: "UexItem",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UexCategory",
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
                    IsGameRelated = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsMining = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UexCategory", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UexCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UexItem",
                table: "UexItem");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "UexItem");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "UexItem");

            migrationBuilder.DropColumn(
                name: "DateAdded",
                table: "UexItem");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "UexItem");

            migrationBuilder.DropColumn(
                name: "Notification",
                table: "UexItem");

            migrationBuilder.DropColumn(
                name: "Section",
                table: "UexItem");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "UexItem");

            migrationBuilder.DropColumn(
                name: "StoreUrl",
                table: "UexItem");

            migrationBuilder.DropColumn(
                name: "VehicleName",
                table: "UexItem");

            migrationBuilder.DropColumn(
                name: "is_commodity",
                table: "UexItem");

            migrationBuilder.DropColumn(
                name: "is_exclusive_concierge",
                table: "UexItem");

            migrationBuilder.DropColumn(
                name: "is_exclusive_pledge",
                table: "UexItem");

            migrationBuilder.DropColumn(
                name: "is_exclusive_subscriber",
                table: "UexItem");

            migrationBuilder.DropColumn(
                name: "is_harvestable",
                table: "UexItem");

            migrationBuilder.DropColumn(
                name: "parentId",
                table: "UexItem");

            migrationBuilder.DropColumn(
                name: "uuid",
                table: "UexItem");

            migrationBuilder.DropColumn(
                name: "vehicleId",
                table: "UexItem");

            migrationBuilder.RenameTable(
                name: "UexItem",
                newName: "CacheItems");

            migrationBuilder.RenameColumn(
                name: "categoryId",
                table: "CacheItems",
                newName: "CategoryId");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "CacheItems",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "GameVersion",
                table: "CacheItems",
                newName: "CategoryName");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CacheItems",
                table: "CacheItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalItems_LocationId",
                table: "PersonalItems",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalItems_StarLocations_LocationId",
                table: "PersonalItems",
                column: "LocationId",
                principalTable: "StarLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
