using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202401314 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ReservationExtra",
                table: "ReservationExtra");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ReservationExtra");

            migrationBuilder.AddColumn<int>(
                name: "ExtraId",
                table: "ReservationExtra",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReservationExtra",
                table: "ReservationExtra",
                columns: new[] { "ExtraId", "ReservationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationExtra_Extra_ExtraId",
                table: "ReservationExtra",
                column: "ExtraId",
                principalTable: "Extra",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationExtra_Extra_ExtraId",
                table: "ReservationExtra");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReservationExtra",
                table: "ReservationExtra");

            migrationBuilder.DropColumn(
                name: "ExtraId",
                table: "ReservationExtra");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ReservationExtra",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReservationExtra",
                table: "ReservationExtra",
                columns: new[] { "Name", "ReservationId" });
        }
    }
}
