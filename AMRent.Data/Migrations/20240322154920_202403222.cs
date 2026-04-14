using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202403222 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalValue",
                table: "Reservation",
                newName: "TotalCost");

            migrationBuilder.RenameColumn(
                name: "ReturnValue",
                table: "Reservation",
                newName: "ReturnCost");

            migrationBuilder.RenameColumn(
                name: "PickupValue",
                table: "Reservation",
                newName: "PickupCost");

            migrationBuilder.RenameColumn(
                name: "InsuranceValue",
                table: "Reservation",
                newName: "InsuranceCost");

            migrationBuilder.RenameColumn(
                name: "CarSegmentValue",
                table: "Reservation",
                newName: "CarSegmentCost");

            migrationBuilder.AddColumn<string>(
                name: "CreditCardAuthorizationKey",
                table: "Reservation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InsuranceExcess",
                table: "Reservation",
                type: "decimal(7,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_InsuranceLevelId",
                table: "Reservation",
                column: "InsuranceLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_InsuranceLevel_InsuranceLevelId",
                table: "Reservation",
                column: "InsuranceLevelId",
                principalTable: "InsuranceLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_InsuranceLevel_InsuranceLevelId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_InsuranceLevelId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "CreditCardAuthorizationKey",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "InsuranceExcess",
                table: "Reservation");

            migrationBuilder.RenameColumn(
                name: "TotalCost",
                table: "Reservation",
                newName: "TotalValue");

            migrationBuilder.RenameColumn(
                name: "ReturnCost",
                table: "Reservation",
                newName: "ReturnValue");

            migrationBuilder.RenameColumn(
                name: "PickupCost",
                table: "Reservation",
                newName: "PickupValue");

            migrationBuilder.RenameColumn(
                name: "InsuranceCost",
                table: "Reservation",
                newName: "InsuranceValue");

            migrationBuilder.RenameColumn(
                name: "CarSegmentCost",
                table: "Reservation",
                newName: "CarSegmentValue");
        }
    }
}
