using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class UpdateOfficeLocationSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeTable",
                table: "OfficeLocation");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "OfficeLocation",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MondayToFridayTimetable",
                table: "OfficeLocation",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SaturdayTimetable",
                table: "OfficeLocation",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "OfficeLocation");

            migrationBuilder.DropColumn(
                name: "MondayToFridayTimetable",
                table: "OfficeLocation");

            migrationBuilder.DropColumn(
                name: "SaturdayTimetable",
                table: "OfficeLocation");

            migrationBuilder.AddColumn<string>(
                name: "TimeTable",
                table: "OfficeLocation",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");
        }
    }
}
