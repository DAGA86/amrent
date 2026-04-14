using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202402192 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AllDay",
                table: "PickupReturnLocationDayOfWeekSchedule",
                newName: "IsOpenedAllDay");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsOpenedAllDay",
                table: "PickupReturnLocationDayOfWeekSchedule",
                newName: "AllDay");
        }
    }
}
