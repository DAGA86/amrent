using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class GenericTextTranslationSchema_UpdateSeedFullSite4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 24,
                column: "Key",
                value: "Home.Pesquisa.Recolha");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 25,
                column: "Key",
                value: "Home.Pesquisa.Devolucao");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 26,
                column: "Key",
                value: "Home.Pesquisa.Botao");

            migrationBuilder.InsertData(
                table: "GenericText",
                columns: new[] { "Id", "Key" },
                values: new object[,]
                {
                    { 75, "Home.Processo.Botao" },
                    { 76, "Home.MaisAlugados.Botao" },
                    { 77, "Home.Pesquisa.Localizacao.Placeholder" },
                    { 78, "Pesquisa.Filtros.Recolha" },
                    { 79, "Pesquisa.Filtros.Devolucao" },
                    { 80, "Pesquisa.Filtros.Localizacao.Placeholder" }
                });

            migrationBuilder.InsertData(
                table: "GenericTextTranslation",
                columns: new[] { "GenericTextId", "LanguageId", "Value" },
                values: new object[,]
                {
                    { 75, 1, "Pedir Proposta" },
                    { 76, 1, "Ver todos" },
                    { 77, 1, "Cidade ou aeroporto" },
                    { 78, 1, "Recolha" },
                    { 79, 1, "Devolução" },
                    { 80, 1, "Cidade ou aeroporto" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 75, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 76, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 77, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 78, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 79, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 80, 1 });

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 24,
                column: "Key",
                value: "Home.Procura.Recolha");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 25,
                column: "Key",
                value: "Home.Procura.Devolucao");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 26,
                column: "Key",
                value: "Home.Procura.Botao");
        }
    }
}
