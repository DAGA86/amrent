using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202402022 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 132,
                column: "Key",
                value: "Segmento.Detalhe.Sidebar.ValorVeiculo.Dias");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 133,
                column: "Key",
                value: "Segmento.Detalhe.Extras.Dia");

            migrationBuilder.InsertData(
                table: "GenericText",
                columns: new[] { "Id", "Key" },
                values: new object[,]
                {
                    { 134, "Pesquisa.Filtros.Campanhas" },
                    { 135, "Segmento.Detalhe.Titulo.OuSimilar" },
                    { 136, "Reserva.Detalhe.Seguro.Franquia" },
                    { 137, "Reserva.Detalhe.Segmento.OuSimilar" }
                });

            migrationBuilder.UpdateData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 90, 1 },
                column: "Value",
                value: "Perído de aluguer");

            migrationBuilder.UpdateData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 132, 1 },
                column: "Value",
                value: "dias");

            migrationBuilder.UpdateData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 133, 1 },
                column: "Value",
                value: "dia");

            migrationBuilder.InsertData(
                table: "GenericTextTranslation",
                columns: new[] { "GenericTextId", "LanguageId", "Value" },
                values: new object[,]
                {
                    { 134, 1, "Campanhas" },
                    { 135, 1, "ou similar" },
                    { 136, 1, "Franquia" },
                    { 137, 1, "ou similar" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 134, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 135, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 136, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 137, 1 });

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 134);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 135);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 136);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 137);

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 132,
                column: "Key",
                value: "Segmento.Detalhe.Sidebar.ValorRecolha");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 133,
                column: "Key",
                value: "Segmento.Detalhe.Sidebar.ValorDevolucao");

            migrationBuilder.UpdateData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 90, 1 },
                column: "Value",
                value: "Veículo");

            migrationBuilder.UpdateData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 132, 1 },
                column: "Value",
                value: "Recolha");

            migrationBuilder.UpdateData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 133, 1 },
                column: "Value",
                value: "Devolução");
        }
    }
}
