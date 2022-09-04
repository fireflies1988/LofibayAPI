using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessEF.Migrations
{
    public partial class AddTable_PhotoStates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PhotoStateId",
                table: "Photos",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "PhotoStates",
                columns: table => new
                {
                    PhotoStateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoStates", x => x.PhotoStateId);
                });

            migrationBuilder.InsertData(
                table: "PhotoStates",
                columns: new[] { "PhotoStateId", "State" },
                values: new object[,]
                {
                    { 1, "NotReviewed" },
                    { 2, "Featured" },
                    { 3, "Rejected" },
                    { 4, "Reported" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Photos_PhotoStateId",
                table: "Photos",
                column: "PhotoStateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_PhotoStates_PhotoStateId",
                table: "Photos",
                column: "PhotoStateId",
                principalTable: "PhotoStates",
                principalColumn: "PhotoStateId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_PhotoStates_PhotoStateId",
                table: "Photos");

            migrationBuilder.DropTable(
                name: "PhotoStates");

            migrationBuilder.DropIndex(
                name: "IX_Photos_PhotoStateId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "PhotoStateId",
                table: "Photos");
        }
    }
}
