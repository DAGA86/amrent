using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202407292 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationExtra_Quotation_QuotationId",
                table: "ReservationExtra");

            migrationBuilder.DropIndex(
                name: "IX_ReservationExtra_QuotationId",
                table: "ReservationExtra");

            migrationBuilder.DropColumn(
                name: "QuotationId",
                table: "ReservationExtra");

            migrationBuilder.CreateTable(
                name: "QuotationItemExtra",
                columns: table => new
                {
                    QuotationItemId = table.Column<int>(type: "int", nullable: false),
                    ExtraId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(7,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationItemExtra", x => new { x.ExtraId, x.QuotationItemId });
                    table.ForeignKey(
                        name: "FK_QuotationItemExtra_Extra_ExtraId",
                        column: x => x.ExtraId,
                        principalTable: "Extra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationItemExtra_QuotationItem_QuotationItemId",
                        column: x => x.QuotationItemId,
                        principalTable: "QuotationItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItem_QuotationId",
                table: "QuotationItem",
                column: "QuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItemExtra_QuotationItemId",
                table: "QuotationItemExtra",
                column: "QuotationItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationItem_Quotation_QuotationId",
                table: "QuotationItem",
                column: "QuotationId",
                principalTable: "Quotation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationItem_Quotation_QuotationId",
                table: "QuotationItem");

            migrationBuilder.DropTable(
                name: "QuotationItemExtra");

            migrationBuilder.DropIndex(
                name: "IX_QuotationItem_QuotationId",
                table: "QuotationItem");

            migrationBuilder.AddColumn<int>(
                name: "QuotationId",
                table: "ReservationExtra",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReservationExtra_QuotationId",
                table: "ReservationExtra",
                column: "QuotationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationExtra_Quotation_QuotationId",
                table: "ReservationExtra",
                column: "QuotationId",
                principalTable: "Quotation",
                principalColumn: "Id");
        }
    }
}
