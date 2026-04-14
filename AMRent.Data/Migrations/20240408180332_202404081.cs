using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202404081 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EmailContent",
                keyColumn: "Id",
                keyValue: 6,
                column: "Type",
                value: 6);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EmailContent",
                keyColumn: "Id",
                keyValue: 6,
                column: "Type",
                value: 3);
        }
    }
}
