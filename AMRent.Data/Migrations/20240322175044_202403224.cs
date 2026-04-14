using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202403224 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GenericText",
                columns: new[] { "Id", "Key" },
                values: new object[,]
                {
                    { 192, "Reserva.Confirmacao.Resumo.Pagamento.TransferenciaBancaria" },
                    { 193, "Reserva.Confirmacao.Resumo.Pagamento.Multibanco.Entidade" },
                    { 194, "Reserva.Confirmacao.Resumo.Pagamento.Multibanco.Referencia" },
                    { 195, "Reserva.Confirmacao.Resumo.Pagamento.Multibanco.Valor" }
                });

            migrationBuilder.InsertData(
                table: "GenericTextTranslation",
                columns: new[] { "GenericTextId", "LanguageId", "Value" },
                values: new object[,]
                {
                    { 192, 1, "Reserva.Confirmacao.Resumo.Pagamento.TransferenciaBancaria" },
                    { 193, 1, "Reserva.Confirmacao.Resumo.Pagamento.Multibanco.Entidade" },
                    { 194, 1, "Reserva.Confirmacao.Resumo.Pagamento.Multibanco.Referencia" },
                    { 195, 1, "Reserva.Confirmacao.Resumo.Pagamento.Multibanco.Valor" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 192, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 193, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 194, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 195, 1 });

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 192);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 193);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 194);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 195);
        }
    }
}
