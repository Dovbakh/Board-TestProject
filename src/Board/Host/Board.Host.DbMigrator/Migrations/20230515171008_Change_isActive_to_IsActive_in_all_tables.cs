using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Board.Host.DbMigrator.Migrations
{
    /// <inheritdoc />
    public partial class Change_isActive_to_IsActive_in_all_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Comments",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Categories",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "Adverts",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "AdvertImages",
                newName: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Comments",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Categories",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Adverts",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "AdvertImages",
                newName: "isActive");
        }
    }
}
