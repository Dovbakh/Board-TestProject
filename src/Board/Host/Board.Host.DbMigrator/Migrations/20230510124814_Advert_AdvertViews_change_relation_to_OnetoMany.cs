using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Board.Host.DbMigrator.Migrations
{
    /// <inheritdoc />
    public partial class Advert_AdvertViews_change_relation_to_OnetoMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AdvertViews_AdvertId",
                table: "AdvertViews");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertViews_AdvertId",
                table: "AdvertViews",
                column: "AdvertId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AdvertViews_AdvertId",
                table: "AdvertViews");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertViews_AdvertId",
                table: "AdvertViews",
                column: "AdvertId",
                unique: true);
        }
    }
}
