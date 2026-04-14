using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class _202409091 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SourceQuotationId",
                table: "Reservation",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_SourceQuotationId",
                table: "Reservation",
                column: "SourceQuotationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Quotation_SourceQuotationId",
                table: "Reservation",
                column: "SourceQuotationId",
                principalTable: "Quotation",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Quotation_SourceQuotationId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_SourceQuotationId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "SourceQuotationId",
                table: "Reservation");
        }
    }
}
