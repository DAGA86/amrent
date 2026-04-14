using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202403223 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GenericText",
                columns: new[] { "Id", "Key" },
                values: new object[,]
                {
                    { 185, "EmailRegistoReserva.Pagamento.CartaoCredito" },
                    { 186, "EmailRegistoReserva.Pagamento.MBWay" },
                    { 187, "EmailRegistoReserva.Pagamento.TransferenciaBancaria" },
                    { 188, "EmailRegistoReserva.Pagamento.Multibanco" },
                    { 189, "EmailRegistoReserva.Pagamento.Multibanco.Entidade" },
                    { 190, "EmailRegistoReserva.Pagamento.Multibanco.Referencia" },
                    { 191, "EmailRegistoReserva.Pagamento.Multibanco.Valor" }
                });

            migrationBuilder.InsertData(
                table: "GenericTextTranslation",
                columns: new[] { "GenericTextId", "LanguageId", "Value" },
                values: new object[,]
                {
                    { 185, 1, "EmailRegistoReserva.Pagamento.CartaoCredito" },
                    { 186, 1, "EmailRegistoReserva.Pagamento.MBWay" },
                    { 187, 1, "EmailRegistoReserva.Pagamento.TransferenciaBancaria" },
                    { 188, 1, "EmailRegistoReserva.Pagamento.Multibanco" },
                    { 189, 1, "EmailRegistoReserva.Pagamento.Multibanco.Entidade" },
                    { 190, 1, "EmailRegistoReserva.Pagamento.Multibanco.Referencia" },
                    { 191, 1, "EmailRegistoReserva.Pagamento.Multibanco.Valor" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 185, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 186, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 187, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 188, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 189, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 190, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 191, 1 });

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 185);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 186);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 187);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 188);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 189);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 190);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 191);
        }
    }
}
