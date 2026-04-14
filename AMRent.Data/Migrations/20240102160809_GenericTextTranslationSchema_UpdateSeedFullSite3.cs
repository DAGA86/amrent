using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class GenericTextTranslationSchema_UpdateSeedFullSite3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GenericText",
                columns: new[] { "Id", "Key" },
                values: new object[,]
                {
                    { 68, "Contactos.Escritorios.Telefone" },
                    { 69, "Contactos.Escritorios.Email" },
                    { 70, "Contactos.Escritorios.Localizacao" },
                    { 71, "Contactos.Escritorios.Horario" },
                    { 72, "Contactos.Escritorios.Horario.SegundaASexta" },
                    { 73, "Contactos.Escritorios.Horario.Sabado" },
                    { 74, "Contactos.Escritorios.Localizacao.Abrir" }
                });

            migrationBuilder.InsertData(
                table: "GenericTextTranslation",
                columns: new[] { "GenericTextId", "LanguageId", "Value" },
                values: new object[,]
                {
                    { 68, 1, "Telefone" },
                    { 69, 1, "Email" },
                    { 70, 1, "Localização" },
                    { 71, 1, "Horário" },
                    { 72, 1, "Seg-Sex" },
                    { 73, 1, "Sábado" },
                    { 74, 1, "Abrir no Google Maps" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 68, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 69, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 70, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 71, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 72, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 73, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 74, 1 });

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 74);
        }
    }
}
