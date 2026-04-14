using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvancePartialPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdvancePartialPaymentPaymentType",
                table: "Reservation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AdvancePartialPaymentValue",
                table: "Reservation",
                type: "decimal(7,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasAdvancePartialPayment",
                table: "Reservation",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdvancePartialPaymentPaymentType",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "AdvancePartialPaymentValue",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "HasAdvancePartialPayment",
                table: "Reservation");
        }
    }
}
