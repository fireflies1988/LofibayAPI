using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessEF.Migrations
{
    public partial class LikedPhotos_RemoveOnDeleteNoActionConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LikedPhotos_Photos_PhotoId",
                table: "LikedPhotos");

            migrationBuilder.AddForeignKey(
                name: "FK_LikedPhotos_Photos_PhotoId",
                table: "LikedPhotos",
                column: "PhotoId",
                principalTable: "Photos",
                principalColumn: "PhotoId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LikedPhotos_Photos_PhotoId",
                table: "LikedPhotos");

            migrationBuilder.AddForeignKey(
                name: "FK_LikedPhotos_Photos_PhotoId",
                table: "LikedPhotos",
                column: "PhotoId",
                principalTable: "Photos",
                principalColumn: "PhotoId");
        }
    }
}
