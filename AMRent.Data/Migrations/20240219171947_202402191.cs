using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202402191 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PickupReturnLocationDayOfWeekSchedule",
                columns: table => new
                {
                    PickupReturnLocationId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    OpeningTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    ClosingTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    AllDay = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickupReturnLocationDayOfWeekSchedule", x => new { x.DayOfWeek, x.PickupReturnLocationId });
                    table.ForeignKey(
                        name: "FK_PickupReturnLocationDayOfWeekSchedule_PickupReturnLocation_PickupReturnLocationId",
                        column: x => x.PickupReturnLocationId,
                        principalTable: "PickupReturnLocation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PickupReturnLocationDayOfWeekSchedule_PickupReturnLocationId",
                table: "PickupReturnLocationDayOfWeekSchedule",
                column: "PickupReturnLocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PickupReturnLocationDayOfWeekSchedule");
        }
    }
}
