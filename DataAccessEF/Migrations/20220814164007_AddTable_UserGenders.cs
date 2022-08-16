using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessEF.Migrations
{
    public partial class AddTable_UserGenders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "GenderId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserGenders",
                columns: table => new
                {
                    GenderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGenders", x => x.GenderId);
                });

            migrationBuilder.InsertData(
                table: "UserGenders",
                columns: new[] { "GenderId", "Gender" },
                values: new object[] { 1, "Unknown" });

            migrationBuilder.InsertData(
                table: "UserGenders",
                columns: new[] { "GenderId", "Gender" },
                values: new object[] { 2, "Male" });

            migrationBuilder.InsertData(
                table: "UserGenders",
                columns: new[] { "GenderId", "Gender" },
                values: new object[] { 3, "Female" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_GenderId",
                table: "Users",
                column: "GenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserGenders_GenderId",
                table: "Users",
                column: "GenderId",
                principalTable: "UserGenders",
                principalColumn: "GenderId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserGenders_GenderId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UserGenders");

            migrationBuilder.DropIndex(
                name: "IX_Users_GenderId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GenderId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
