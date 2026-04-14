using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202401312 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DriverLicenceNumber",
                table: "Reservation",
                newName: "DriverLicenseNumber");

            migrationBuilder.RenameColumn(
                name: "DriverLicenceExpireDate",
                table: "Reservation",
                newName: "DriverLicenseExpireDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DriverLicenseNumber",
                table: "Reservation",
                newName: "DriverLicenceNumber");

            migrationBuilder.RenameColumn(
                name: "DriverLicenseExpireDate",
                table: "Reservation",
                newName: "DriverLicenceExpireDate");
        }
    }
}
