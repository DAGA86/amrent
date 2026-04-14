using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class OfficeLocationsSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OfficeLocation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    TimeTable = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    GoogleMapsURL = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeLocation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OfficeLocationTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    OfficeLocationId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeLocationTranslation", x => new { x.LanguageId, x.OfficeLocationId });
                    table.ForeignKey(
                        name: "FK_OfficeLocationTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfficeLocationTranslation_OfficeLocation_OfficeLocationId",
                        column: x => x.OfficeLocationId,
                        principalTable: "OfficeLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Language",
                columns: new[] { "Id", "Code", "CountryCode", "Name" },
                values: new object[] { 1, "pt", "pt", "Português" });

            migrationBuilder.InsertData(
                table: "Language",
                columns: new[] { "Id", "Code", "CountryCode", "Name" },
                values: new object[] { 2, "en", "uk", "English" });

            migrationBuilder.CreateIndex(
                name: "IX_OfficeLocationTranslation_OfficeLocationId",
                table: "OfficeLocationTranslation",
                column: "OfficeLocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfficeLocationTranslation");

            migrationBuilder.DropTable(
                name: "OfficeLocation");

            migrationBuilder.DeleteData(
                table: "Language",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Language",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
