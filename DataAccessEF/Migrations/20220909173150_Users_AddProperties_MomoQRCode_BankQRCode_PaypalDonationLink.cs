using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessEF.Migrations
{
    public partial class Users_AddProperties_MomoQRCode_BankQRCode_PaypalDonationLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankQRCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MomoQRCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaypalDonationLink",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankQRCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MomoQRCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PaypalDonationLink",
                table: "Users");
        }
    }
}
