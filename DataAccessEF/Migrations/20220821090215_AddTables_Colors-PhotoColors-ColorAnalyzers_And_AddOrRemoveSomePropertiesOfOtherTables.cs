using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessEF.Migrations
{
    public partial class AddTables_ColorsPhotoColorsColorAnalyzers_And_AddOrRemoveSomePropertiesOfOtherTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhotoTags_Tags_TagId",
                table: "PhotoTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhotoTags",
                table: "PhotoTags");

            migrationBuilder.DropIndex(
                name: "IX_PhotoTags_TagId",
                table: "PhotoTags");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Tags",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TagName",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "PhotoTags");

            migrationBuilder.AlterColumn<string>(
                name: "RoleName",
                table: "Roles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TagName",
                table: "PhotoTags",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Colors",
                table: "Photos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FacesDetected",
                table: "Photos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Format",
                table: "Photos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Grayscale",
                table: "Photos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasSensitiveContent",
                table: "Photos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Phash",
                table: "Photos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SemiTransparent",
                table: "Photos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Collections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Views",
                table: "Collections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhotoTags",
                table: "PhotoTags",
                columns: new[] { "PhotoId", "TagName" });

            migrationBuilder.CreateTable(
                name: "ColorAnalyzers",
                columns: table => new
                {
                    ColorAnalyzerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColorAnalyzerName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorAnalyzers", x => x.ColorAnalyzerId);
                });

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "PhotoColors",
                columns: table => new
                {
                    PhotoId = table.Column<int>(type: "int", nullable: false),
                    ColorName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ColorAnalyzerId = table.Column<int>(type: "int", nullable: false),
                    PredominantPercent = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoColors", x => new { x.PhotoId, x.ColorName, x.ColorAnalyzerId });
                    table.ForeignKey(
                        name: "FK_PhotoColors_ColorAnalyzers_ColorAnalyzerId",
                        column: x => x.ColorAnalyzerId,
                        principalTable: "ColorAnalyzers",
                        principalColumn: "ColorAnalyzerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhotoColors_Colors_ColorName",
                        column: x => x.ColorName,
                        principalTable: "Colors",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhotoColors_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Photos",
                        principalColumn: "PhotoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ColorAnalyzers",
                columns: new[] { "ColorAnalyzerId", "ColorAnalyzerName" },
                values: new object[] { 1, "Google" });

            migrationBuilder.InsertData(
                table: "ColorAnalyzers",
                columns: new[] { "ColorAnalyzerId", "ColorAnalyzerName" },
                values: new object[] { 2, "Cloudinary" });

            migrationBuilder.CreateIndex(
                name: "IX_PhotoTags_TagName",
                table: "PhotoTags",
                column: "TagName");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoColors_ColorAnalyzerId",
                table: "PhotoColors",
                column: "ColorAnalyzerId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoColors_ColorName",
                table: "PhotoColors",
                column: "ColorName");

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoTags_Tags_TagName",
                table: "PhotoTags",
                column: "TagName",
                principalTable: "Tags",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhotoTags_Tags_TagName",
                table: "PhotoTags");

            migrationBuilder.DropTable(
                name: "PhotoColors");

            migrationBuilder.DropTable(
                name: "ColorAnalyzers");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhotoTags",
                table: "PhotoTags");

            migrationBuilder.DropIndex(
                name: "IX_PhotoTags_TagName",
                table: "PhotoTags");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TagName",
                table: "PhotoTags");

            migrationBuilder.DropColumn(
                name: "Colors",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "FacesDetected",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Format",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Grayscale",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "HasSensitiveContent",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Phash",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "SemiTransparent",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "Views",
                table: "Collections");

            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "Tags",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "TagName",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "RoleName",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "PhotoTags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhotoTags",
                table: "PhotoTags",
                columns: new[] { "PhotoId", "TagId" });

            migrationBuilder.CreateIndex(
                name: "IX_PhotoTags_TagId",
                table: "PhotoTags",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoTags_Tags_TagId",
                table: "PhotoTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "TagId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
