using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202402102 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GenericText",
                columns: new[] { "Id", "Key" },
                values: new object[,]
                {
                    { 169, "Placeholder.Data" },
                    { 170, "Placeholder.Hora" },
                    { 171, "Pesquisa.Filtros.RecolhaDevolucao.AvisoDataHoraAjustada" },
                    { 172, "Segmento.Detalhe.Sidebar.AvisoDataHoraAjustada" }
                });

            migrationBuilder.InsertData(
                table: "GenericTextTranslation",
                columns: new[] { "GenericTextId", "LanguageId", "Value" },
                values: new object[,]
                {
                    { 169, 1, "DD-MM-AAAA" },
                    { 170, 1, "HH:MM" },
                    { 171, 1, "AvisoDataHoraAjustada" },
                    { 172, 1, "AvisoDataHoraAjustada" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 169, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 170, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 171, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 172, 1 });

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 169);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 170);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 171);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 172);
        }
    }
}
