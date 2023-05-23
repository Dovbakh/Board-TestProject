using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Board.Host.DbMigrator.Migrations
{
    /// <inheritdoc />
    public partial class Change_UserId_to_UserAuthorId_in_Comments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Comments",
                newName: "UserAuthorId");

            migrationBuilder.RenameColumn(
                name: "isRegistered",
                table: "AdvertViews",
                newName: "IsRegistered");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserAuthorId",
                table: "Comments",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "IsRegistered",
                table: "AdvertViews",
                newName: "isRegistered");
        }
    }
}
