using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeQuotationItemExtraAndReservationExtraValueToUnitValueStep1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "ReservationExtra",
                newName: "UnitValue");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "QuotationItemExtra",
                newName: "UnitValue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnitValue",
                table: "ReservationExtra",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "UnitValue",
                table: "QuotationItemExtra",
                newName: "Value");
        }
    }
}
