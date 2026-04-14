using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202402292 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDefault",
                table: "PickupReturnLocation",
                newName: "IsWorkingOffice");

            migrationBuilder.AddColumn<bool>(
                name: "IsSelectedByDefault",
                table: "PickupReturnLocation",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSelectedByDefault",
                table: "PickupReturnLocation");

            migrationBuilder.RenameColumn(
                name: "IsWorkingOffice",
                table: "PickupReturnLocation",
                newName: "IsDefault");
        }
    }
}
