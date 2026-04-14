using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202402072 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Price_SeasonId",
                table: "Price");

            migrationBuilder.DropColumn(
                name: "TaxType",
                table: "PickupReturnLocation");

            migrationBuilder.DropColumn(
                name: "TaxValue",
                table: "PickupReturnLocation");

            migrationBuilder.CreateTable(
                name: "PickupReturnLocationTax",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickupReturnLocationId = table.Column<int>(type: "int", nullable: false),
                    Days = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(7,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickupReturnLocationTax", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickupReturnLocationTax_PickupReturnLocation_PickupReturnLocationId",
                        column: x => x.PickupReturnLocationId,
                        principalTable: "PickupReturnLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Price_SeasonId_CarSegmentId_Days",
                table: "Price",
                columns: new[] { "SeasonId", "CarSegmentId", "Days" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PickupReturnLocationTax_PickupReturnLocationId_Days",
                table: "PickupReturnLocationTax",
                columns: new[] { "PickupReturnLocationId", "Days" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PickupReturnLocationTax");

            migrationBuilder.DropIndex(
                name: "IX_Price_SeasonId_CarSegmentId_Days",
                table: "Price");

            migrationBuilder.AddColumn<int>(
                name: "TaxType",
                table: "PickupReturnLocation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxValue",
                table: "PickupReturnLocation",
                type: "decimal(7,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Price_SeasonId",
                table: "Price",
                column: "SeasonId");
        }
    }
}
