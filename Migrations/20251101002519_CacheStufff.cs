using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarInventoryAPI.Migrations
{
    /// <inheritdoc />
    public partial class CacheStufff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UexCategory",
                table: "UexCategory");

            migrationBuilder.RenameTable(
                name: "UexCategory",
                newName: "UexCategories");

            migrationBuilder.AlterColumn<int>(
                name: "IsMining",
                table: "UexCategories",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<int>(
                name: "IsGameRelated",
                table: "UexCategories",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<int>(
                name: "DateModified",
                table: "UexCategories",
                type: "int",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<int>(
                name: "DateAdded",
                table: "UexCategories",
                type: "int",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UexCategories",
                table: "UexCategories",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UexCategories",
                table: "UexCategories");

            migrationBuilder.RenameTable(
                name: "UexCategories",
                newName: "UexCategory");

            migrationBuilder.AlterColumn<bool>(
                name: "IsMining",
                table: "UexCategory",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsGameRelated",
                table: "UexCategory",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateModified",
                table: "UexCategory",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateAdded",
                table: "UexCategory",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UexCategory",
                table: "UexCategory",
                column: "Id");
        }
    }
}
