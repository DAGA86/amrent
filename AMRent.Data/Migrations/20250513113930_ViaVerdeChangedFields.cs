using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class ViaVerdeChangedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverLicenceNumber",
                table: "ViaVerdeMovements");

            migrationBuilder.RenameColumn(
                name: "VatNumber",
                table: "ViaVerdeMovements",
                newName: "IdentificationNumber");

            migrationBuilder.InsertData(
                table: "GenericText",
                columns: new[] { "Id", "Key" },
                values: new object[] { 235, "Reserva.Detalhe.Consentimentos.Titulo" });

            migrationBuilder.InsertData(
                table: "GenericTextTranslation",
                columns: new[] { "GenericTextId", "LanguageId", "Value" },
                values: new object[] { 235, 1, "Reserva.Detalhe.Consentimentos.Titulo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 235, 1 });

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 235);

            migrationBuilder.RenameColumn(
                name: "IdentificationNumber",
                table: "ViaVerdeMovements",
                newName: "VatNumber");

            migrationBuilder.AddColumn<string>(
                name: "DriverLicenceNumber",
                table: "ViaVerdeMovements",
                type: "nvarchar(18)",
                maxLength: 18,
                nullable: true);
        }
    }
}
