using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202402291 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Country_DriverIdentificationCountryId",
                table: "Reservation");

            migrationBuilder.RenameColumn(
                name: "DriverIdentificationNumber",
                table: "Reservation",
                newName: "DriverIdentityNumber");

            migrationBuilder.RenameColumn(
                name: "DriverIdentificationCountryId",
                table: "Reservation",
                newName: "DriverIdentityCountryId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservation_DriverIdentificationCountryId",
                table: "Reservation",
                newName: "IX_Reservation_DriverIdentityCountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Country_DriverIdentityCountryId",
                table: "Reservation",
                column: "DriverIdentityCountryId",
                principalTable: "Country",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Country_DriverIdentityCountryId",
                table: "Reservation");

            migrationBuilder.RenameColumn(
                name: "DriverIdentityNumber",
                table: "Reservation",
                newName: "DriverIdentificationNumber");

            migrationBuilder.RenameColumn(
                name: "DriverIdentityCountryId",
                table: "Reservation",
                newName: "DriverIdentificationCountryId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservation_DriverIdentityCountryId",
                table: "Reservation",
                newName: "IX_Reservation_DriverIdentificationCountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Country_DriverIdentificationCountryId",
                table: "Reservation",
                column: "DriverIdentificationCountryId",
                principalTable: "Country",
                principalColumn: "Id");
        }
    }
}
