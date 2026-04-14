using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedViaVerdeRejectedPaymentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ViaVerdeMovements",
                newName: "NameOrDeliverySlipNumber");

            migrationBuilder.AlterColumn<string>(
                name: "DriverLicenceNumber",
                table: "ViaVerdeMovements",
                type: "nvarchar(18)",
                maxLength: 18,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcquirerResultCode",
                table: "ViaVerdeMovements",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcquirerResultDescription",
                table: "ViaVerdeMovements",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreditCardLast4Digits",
                table: "ViaVerdeMovements",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SendToViaVerde",
                table: "ViaVerdeMovements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VatNumber",
                table: "ViaVerdeMovements",
                type: "nvarchar(18)",
                maxLength: 18,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcquirerResultCode",
                table: "ViaVerdeMovements");

            migrationBuilder.DropColumn(
                name: "AcquirerResultDescription",
                table: "ViaVerdeMovements");

            migrationBuilder.DropColumn(
                name: "CreditCardLast4Digits",
                table: "ViaVerdeMovements");

            migrationBuilder.DropColumn(
                name: "SendToViaVerde",
                table: "ViaVerdeMovements");

            migrationBuilder.DropColumn(
                name: "VatNumber",
                table: "ViaVerdeMovements");

            migrationBuilder.RenameColumn(
                name: "NameOrDeliverySlipNumber",
                table: "ViaVerdeMovements",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "DriverLicenceNumber",
                table: "ViaVerdeMovements",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(18)",
                oldMaxLength: 18,
                oldNullable: true);
        }
    }
}
