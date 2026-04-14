using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class UpdateReservationSchema_AddDriverTelephoneCountryCodeAndBillTelephoneCountryCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillTelephoneCountryCode",
                table: "Reservation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverTelephoneCountryCode",
                table: "Reservation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillTelephoneCountryCode",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "DriverTelephoneCountryCode",
                table: "Reservation");
        }
    }
}
