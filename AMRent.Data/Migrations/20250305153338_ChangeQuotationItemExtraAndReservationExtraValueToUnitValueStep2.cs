using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeQuotationItemExtraAndReservationExtraValueToUnitValueStep2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [QuotationItemExtra] WHERE [Quantity] = 0");
            migrationBuilder.Sql("UPDATE [QuotationItemExtra] SET [UnitValue] = ROUND(([UnitValue] / [Quantity]), 2)");
            migrationBuilder.Sql("UPDATE [ReservationExtra] SET [UnitValue] = ROUND(([UnitValue] / [Quantity]), 2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [QuotationItemExtra] SET [UnitValue] = ROUND(([UnitValue] * [Quantity]), 2)");
            migrationBuilder.Sql("UPDATE [ReservationExtra] SET [UnitValue] = ROUND(([UnitValue] * [Quantity])");
        }
    }
}
