using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class PickupReturnLocationSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PickupReturnLocation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaxValue = table.Column<decimal>(type: "decimal(7,2)", nullable: true),
                    TaxType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickupReturnLocation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PickupReturnLocationTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    PickupReturnLocationId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickupReturnLocationTranslation", x => new { x.LanguageId, x.PickupReturnLocationId });
                    table.ForeignKey(
                        name: "FK_PickupReturnLocationTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PickupReturnLocationTranslation_PickupReturnLocation_PickupReturnLocationId",
                        column: x => x.PickupReturnLocationId,
                        principalTable: "PickupReturnLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PickupReturnLocationTranslation_PickupReturnLocationId",
                table: "PickupReturnLocationTranslation",
                column: "PickupReturnLocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PickupReturnLocationTranslation");

            migrationBuilder.DropTable(
                name: "PickupReturnLocation");
        }
    }
}
