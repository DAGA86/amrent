using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverIdentityTypeAndDriverVatNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DriverIdentityType",
                table: "Reservation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverVatNumber",
                table: "Reservation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "GenericText",
                columns: new[] { "Id", "Key" },
                values: new object[] { 231, "Reserva.Detalhe.DadosCondutor.TipoIdentificacao" });

            migrationBuilder.InsertData(
                table: "GenericTextTranslation",
                columns: new[] { "GenericTextId", "LanguageId", "Value" },
                values: new object[] { 231, 1, "Tipo de identificacao" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 231, 1 });

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 231);

            migrationBuilder.DropColumn(
                name: "DriverIdentityType",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "DriverVatNumber",
                table: "Reservation");
        }
    }
}
