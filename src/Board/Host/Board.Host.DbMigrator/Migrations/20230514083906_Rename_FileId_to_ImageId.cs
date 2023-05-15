using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Board.Host.DbMigrator.Migrations
{
    /// <inheritdoc />
    public partial class Rename_FileId_to_ImageId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Adverts_AdvertId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "AdvertisementId",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "FileId",
                table: "AdvertImages",
                newName: "ImageId");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdvertId",
                table: "Comments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Adverts_AdvertId",
                table: "Comments",
                column: "AdvertId",
                principalTable: "Adverts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Adverts_AdvertId",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "ImageId",
                table: "AdvertImages",
                newName: "FileId");

            migrationBuilder.AlterColumn<Guid>(
                name: "AdvertId",
                table: "Comments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "AdvertisementId",
                table: "Comments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Adverts_AdvertId",
                table: "Comments",
                column: "AdvertId",
                principalTable: "Adverts",
                principalColumn: "Id");
        }
    }
}
