using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202404086 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 180,
                column: "Key",
                value: "Reserva.Pagamento.TransferenciaBancaria.Texto");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 181,
                column: "Key",
                value: "Reserva.Pagamento.CartaoCredito.Texto");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 182,
                column: "Key",
                value: "Reserva.Pagamento.ReferenciaMultibanco.Texto");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 184,
                column: "Key",
                value: "Reserva.Pagamento.MbWay.Texto");

            migrationBuilder.UpdateData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 180, 1 },
                column: "Value",
                value: "Texto para pagamento por transferência");

            migrationBuilder.UpdateData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 181, 1 },
                column: "Value",
                value: "Texto para pagamento por cartão de crédito");

            migrationBuilder.UpdateData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 182, 1 },
                column: "Value",
                value: "Texto para pagamento por referência multibanco");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 180,
                column: "Key",
                value: "Reserva.Pagamento.CartaoCredito.Nome");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 181,
                column: "Key",
                value: "Reserva.Pagamento.CartaoCredito.Numero");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 182,
                column: "Key",
                value: "Reserva.Pagamento.CartaoCredito.Validate");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 184,
                column: "Key",
                value: "Reserva.Pagamento.MbWay.Numero");

            migrationBuilder.UpdateData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 180, 1 },
                column: "Value",
                value: "Nome");

            migrationBuilder.UpdateData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 181, 1 },
                column: "Value",
                value: "Número");

            migrationBuilder.UpdateData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 182, 1 },
                column: "Value",
                value: "Validate");
        }
    }
}
