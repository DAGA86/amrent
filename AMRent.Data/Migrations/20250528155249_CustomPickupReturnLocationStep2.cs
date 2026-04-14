using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class CustomPickupReturnLocationStep2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PickupReturnLocationDayOfWeekSchedule",
                columns: new[] { "DayOfWeek", "PickupReturnLocationId", "ClosingTime", "IsClosed", "LunchBreakEndTime", "LunchBreakStartTime", "OpeningTime" },
                values: new object[,]
                {
                    { 1, -1, new TimeSpan(0, 23, 59, 0, 0), false, null, null, new TimeSpan(0, 0, 0, 0, 0) },
                    { 2, -1, new TimeSpan(0, 23, 59, 0, 0), false, null, null, new TimeSpan(0, 0, 0, 0, 0) },
                    { 3, -1, new TimeSpan(0, 23, 59, 0, 0), false, null, null, new TimeSpan(0, 0, 0, 0, 0) },
                    { 4, -1, new TimeSpan(0, 23, 59, 0, 0), false, null, null, new TimeSpan(0, 0, 0, 0, 0) },
                    { 5, -1, new TimeSpan(0, 23, 59, 0, 0), false, null, null, new TimeSpan(0, 0, 0, 0, 0) },
                    { 6, -1, new TimeSpan(0, 23, 59, 0, 0), false, null, null, new TimeSpan(0, 0, 0, 0, 0) },
                    { 7, -1, new TimeSpan(0, 23, 59, 0, 0), false, null, null, new TimeSpan(0, 0, 0, 0, 0) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PickupReturnLocationDayOfWeekSchedule",
                keyColumns: new[] { "DayOfWeek", "PickupReturnLocationId" },
                keyValues: new object[] { 1, -1 });

            migrationBuilder.DeleteData(
                table: "PickupReturnLocationDayOfWeekSchedule",
                keyColumns: new[] { "DayOfWeek", "PickupReturnLocationId" },
                keyValues: new object[] { 2, -1 });

            migrationBuilder.DeleteData(
                table: "PickupReturnLocationDayOfWeekSchedule",
                keyColumns: new[] { "DayOfWeek", "PickupReturnLocationId" },
                keyValues: new object[] { 3, -1 });

            migrationBuilder.DeleteData(
                table: "PickupReturnLocationDayOfWeekSchedule",
                keyColumns: new[] { "DayOfWeek", "PickupReturnLocationId" },
                keyValues: new object[] { 4, -1 });

            migrationBuilder.DeleteData(
                table: "PickupReturnLocationDayOfWeekSchedule",
                keyColumns: new[] { "DayOfWeek", "PickupReturnLocationId" },
                keyValues: new object[] { 5, -1 });

            migrationBuilder.DeleteData(
                table: "PickupReturnLocationDayOfWeekSchedule",
                keyColumns: new[] { "DayOfWeek", "PickupReturnLocationId" },
                keyValues: new object[] { 6, -1 });

            migrationBuilder.DeleteData(
                table: "PickupReturnLocationDayOfWeekSchedule",
                keyColumns: new[] { "DayOfWeek", "PickupReturnLocationId" },
                keyValues: new object[] { 7, -1 });
        }
    }
}
