using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessEF.Migrations
{
    public partial class Users_Photos_ChangeOnDeleteConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhotoCollections_Photos_PhotoId",
                table: "PhotoCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Users_UserId",
                table: "Photos");

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoCollections_Photos_PhotoId",
                table: "PhotoCollections",
                column: "PhotoId",
                principalTable: "Photos",
                principalColumn: "PhotoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Users_UserId",
                table: "Photos",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhotoCollections_Photos_PhotoId",
                table: "PhotoCollections");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Users_UserId",
                table: "Photos");

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoCollections_Photos_PhotoId",
                table: "PhotoCollections",
                column: "PhotoId",
                principalTable: "Photos",
                principalColumn: "PhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Users_UserId",
                table: "Photos",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
