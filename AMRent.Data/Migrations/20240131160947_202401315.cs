using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202401315 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillTelephoneCountryCode",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "DriverTelephoneCountryCode",
                table: "Reservation");

            migrationBuilder.RenameColumn(
                name: "DriverIdNumber",
                table: "Reservation",
                newName: "DriverIdentificationNumber");

            migrationBuilder.AddColumn<int>(
                name: "BillCountryId",
                table: "Reservation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BillTelephonePrefixCountryId",
                table: "Reservation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DriverIdentificationCountryId",
                table: "Reservation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DriverLicenseCountryId",
                table: "Reservation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DriverTelephonePrefixCountryId",
                table: "Reservation",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_BillCountryId",
                table: "Reservation",
                column: "BillCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_BillTelephonePrefixCountryId",
                table: "Reservation",
                column: "BillTelephonePrefixCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_DriverIdentificationCountryId",
                table: "Reservation",
                column: "DriverIdentificationCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_DriverLicenseCountryId",
                table: "Reservation",
                column: "DriverLicenseCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_DriverTelephonePrefixCountryId",
                table: "Reservation",
                column: "DriverTelephonePrefixCountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Country_BillCountryId",
                table: "Reservation",
                column: "BillCountryId",
                principalTable: "Country",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Country_BillTelephonePrefixCountryId",
                table: "Reservation",
                column: "BillTelephonePrefixCountryId",
                principalTable: "Country",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Country_DriverIdentificationCountryId",
                table: "Reservation",
                column: "DriverIdentificationCountryId",
                principalTable: "Country",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Country_DriverLicenseCountryId",
                table: "Reservation",
                column: "DriverLicenseCountryId",
                principalTable: "Country",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Country_DriverTelephonePrefixCountryId",
                table: "Reservation",
                column: "DriverTelephonePrefixCountryId",
                principalTable: "Country",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Country_BillCountryId",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Country_BillTelephonePrefixCountryId",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Country_DriverIdentificationCountryId",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Country_DriverLicenseCountryId",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Country_DriverTelephonePrefixCountryId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_BillCountryId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_BillTelephonePrefixCountryId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_DriverIdentificationCountryId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_DriverLicenseCountryId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_DriverTelephonePrefixCountryId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "BillCountryId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "BillTelephonePrefixCountryId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "DriverIdentificationCountryId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "DriverLicenseCountryId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "DriverTelephonePrefixCountryId",
                table: "Reservation");

            migrationBuilder.RenameColumn(
                name: "DriverIdentificationNumber",
                table: "Reservation",
                newName: "DriverIdNumber");

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
    }
}
