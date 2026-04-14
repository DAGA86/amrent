using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202402201 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsOpenedAllDay",
                table: "PickupReturnLocationDayOfWeekSchedule",
                newName: "IsClosed");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "LunchBreakEndTime",
                table: "PickupReturnLocationDayOfWeekSchedule",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "LunchBreakStartTime",
                table: "PickupReturnLocationDayOfWeekSchedule",
                type: "time",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LunchBreakEndTime",
                table: "PickupReturnLocationDayOfWeekSchedule");

            migrationBuilder.DropColumn(
                name: "LunchBreakStartTime",
                table: "PickupReturnLocationDayOfWeekSchedule");

            migrationBuilder.RenameColumn(
                name: "IsClosed",
                table: "PickupReturnLocationDayOfWeekSchedule",
                newName: "IsOpenedAllDay");
        }
    }
}
