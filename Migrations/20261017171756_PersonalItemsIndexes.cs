using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarInventoryAPI.Migrations
{
    /// <inheritdoc />
    public partial class PersonalItemsIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "PersonalItems",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UexItem_categoryId",
                table: "UexItem",
                column: "categoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalItems_Username_LocationId",
                table: "PersonalItems",
                columns: new[] { "Username", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonalItems_Username_LocationId_UexIdentifier",
                table: "PersonalItems",
                columns: new[] { "Username", "LocationId", "UexIdentifier" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UexItem_categoryId",
                table: "UexItem");

            migrationBuilder.DropIndex(
                name: "IX_PersonalItems_Username_LocationId",
                table: "PersonalItems");

            migrationBuilder.DropIndex(
                name: "IX_PersonalItems_Username_LocationId_UexIdentifier",
                table: "PersonalItems");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "PersonalItems",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
