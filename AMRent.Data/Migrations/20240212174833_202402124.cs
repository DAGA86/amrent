using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202402124 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Reservation");

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "Reservation",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Reservation");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Reservation",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
