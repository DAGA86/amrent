using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202402121 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickupLocation",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "PickupTime",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "ReturnLocation",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "ReturnTime",
                table: "Reservation");

            migrationBuilder.RenameColumn(
                name: "ReturnDate",
                table: "Reservation",
                newName: "ReturnDateTime");

            migrationBuilder.RenameColumn(
                name: "PickupDate",
                table: "Reservation",
                newName: "PickupDateTime");

            migrationBuilder.AddColumn<int>(
                name: "PickupLocationId",
                table: "Reservation",
                type: "int",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "ReturnLocationId",
                table: "Reservation",
                type: "int",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_PickupLocationId",
                table: "Reservation",
                column: "PickupLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_ReturnLocationId",
                table: "Reservation",
                column: "ReturnLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_PickupReturnLocation_PickupLocationId",
                table: "Reservation",
                column: "PickupLocationId",
                principalTable: "PickupReturnLocation",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_PickupReturnLocation_ReturnLocationId",
                table: "Reservation",
                column: "ReturnLocationId",
                principalTable: "PickupReturnLocation",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_PickupReturnLocation_PickupLocationId",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_PickupReturnLocation_ReturnLocationId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_PickupLocationId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_ReturnLocationId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "PickupLocationId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "ReturnLocationId",
                table: "Reservation");

            migrationBuilder.RenameColumn(
                name: "ReturnDateTime",
                table: "Reservation",
                newName: "ReturnDate");

            migrationBuilder.RenameColumn(
                name: "PickupDateTime",
                table: "Reservation",
                newName: "PickupDate");

            migrationBuilder.AddColumn<string>(
                name: "PickupLocation",
                table: "Reservation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "PickupTime",
                table: "Reservation",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "ReturnLocation",
                table: "Reservation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ReturnTime",
                table: "Reservation",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
