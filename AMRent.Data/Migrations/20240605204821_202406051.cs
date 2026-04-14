using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202406051 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 183,
                column: "Key",
                value: "Footer.TermosECondicoesVisaMastercard");

            migrationBuilder.UpdateData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 183, 1 },
                column: "Value",
                value: "Termos e condições de pagamento");

            migrationBuilder.InsertData(
                table: "TranslatableSetting",
                columns: new[] { "Id", "Code" },
                values: new object[] { 3, 3 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TranslatableSetting",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 183,
                column: "Key",
                value: "Reserva.Pagamento.CartaoCredito.CodigoSeguranca");

            migrationBuilder.UpdateData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 183, 1 },
                column: "Value",
                value: "CVV");
        }
    }
}
