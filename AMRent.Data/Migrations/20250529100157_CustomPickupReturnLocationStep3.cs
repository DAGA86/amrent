using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class CustomPickupReturnLocationStep3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PickupReturnLocation",
                keyColumn: "Id",
                keyValue: -1,
                column: "IsAlwaysAvailableForPickupAndReturn",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PickupReturnLocation",
                keyColumn: "Id",
                keyValue: -1,
                column: "IsAlwaysAvailableForPickupAndReturn",
                value: true);
        }
    }
}
