using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202403201 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GenericText",
                columns: new[] { "Id", "Key" },
                values: new object[,]
                {
                    { 174, "Login.Titulo" },
                    { 175, "Login.Username" },
                    { 176, "Login.Password" },
                    { 177, "Login.Botao" },
                    { 178, "Login.Registar.Texto" },
                    { 179, "Login.Registar.Link" }
                });

            migrationBuilder.InsertData(
                table: "GenericTextTranslation",
                columns: new[] { "GenericTextId", "LanguageId", "Value" },
                values: new object[,]
                {
                    { 174, 1, "Entrar" },
                    { 175, 1, "Nome de Utilizador" },
                    { 176, 1, "Palavra passe" },
                    { 177, 1, "Entrar" },
                    { 178, 1, "Ainda não tem conta?" },
                    { 179, 1, "Registar" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 174, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 175, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 176, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 177, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 178, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 179, 1 });

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 174);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 175);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 176);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 177);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 178);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 179);
        }
    }
}
