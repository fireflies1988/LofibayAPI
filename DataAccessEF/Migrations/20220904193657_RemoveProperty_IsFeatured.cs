using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessEF.Migrations
{
    public partial class RemoveProperty_IsFeatured : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PhotoStates",
                keyColumn: "PhotoStateId",
                keyValue: 4);

            migrationBuilder.RenameColumn(
                name: "IsFeatured",
                table: "Photos",
                newName: "IsReported");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsReported",
                table: "Photos",
                newName: "IsFeatured");

            migrationBuilder.InsertData(
                table: "PhotoStates",
                columns: new[] { "PhotoStateId", "State" },
                values: new object[] { 4, "Reported" });
        }
    }
}
