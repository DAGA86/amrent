using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class GenericTextTranslationSchema_UpdateSeedFullSite8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverAddress",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "DriverPostalCode",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "DriverPostalLocation",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "DriverVatNumber",
                table: "Reservation");

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Alpha2Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelephoneCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CountryTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryTranslation", x => new { x.LanguageId, x.CountryId });
                    table.ForeignKey(
                        name: "FK_CountryTranslation_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CountryTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 109,
                column: "Key",
                value: "Reserva.Detalhe.DadosCondutor.Titulo");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 110,
                column: "Key",
                value: "Reserva.Detalhe.DadosCondutor.Nome");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 111,
                column: "Key",
                value: "Reserva.Detalhe.DadosCondutor.Email");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 112,
                column: "Key",
                value: "Reserva.Detalhe.DadosCondutor.Telefone");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 113,
                column: "Key",
                value: "Reserva.Detalhe.DadosFaturacao.Morada");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 114,
                column: "Key",
                value: "Reserva.Detalhe.DadosFaturacao.CodigoPostal");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 115,
                column: "Key",
                value: "Reserva.Detalhe.DadosFaturacao.LocalidadePostal");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 116,
                column: "Key",
                value: "Reserva.Detalhe.DadosOutros.NumeroVoo");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 117,
                column: "Key",
                value: "Reserva.Detalhe.DadosCondutor.DataNascimento");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 118,
                column: "Key",
                value: "Reserva.Detalhe.DadosOutros.Comentarios");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 119,
                column: "Key",
                value: "Reserva.Detalhe.DadosCondutor.NumeroIdentificacao");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 120,
                column: "Key",
                value: "Reserva.Detalhe.DadosFaturacao.NumeroContribuinte");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 121,
                column: "Key",
                value: "Reserva.Detalhe.DadosCondutor.NumeroCartaConducao");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 122,
                column: "Key",
                value: "Reserva.Detalhe.DadosCondutor.ValidadeCartaConducao");

            migrationBuilder.InsertData(
                table: "GenericText",
                columns: new[] { "Id", "Key" },
                values: new object[,]
                {
                    { 123, "Reserva.Detalhe.DadosCondutor.PaisTelefone" },
                    { 124, "Reserva.Detalhe.DadosCondutor.PaisIdentificacao" },
                    { 125, "Reserva.Detalhe.DadosCondutor.PaisCartaConducao" },
                    { 126, "Reserva.Detalhe.DadosFaturacao.Titulo" },
                    { 127, "Reserva.Detalhe.DadosFaturacao.Nome" },
                    { 128, "Reserva.Detalhe.DadosFaturacao.Email" },
                    { 129, "Reserva.Detalhe.DadosFaturacao.PaisTelefone" },
                    { 130, "Reserva.Detalhe.DadosFaturacao.Telefone" },
                    { 131, "Reserva.Detalhe.DadosFaturacao.Pais" }
                });

            migrationBuilder.InsertData(
                table: "GenericTextTranslation",
                columns: new[] { "GenericTextId", "LanguageId", "Value" },
                values: new object[,]
                {
                    { 123, 1, "Indicativo" },
                    { 124, 1, "País ID" },
                    { 125, 1, "País" },
                    { 126, 1, "Dados de faturação" },
                    { 127, 1, "Nome" },
                    { 128, 1, "Email" },
                    { 129, 1, "Indicativo" },
                    { 130, 1, "Telefone" },
                    { 131, 1, "País" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CountryTranslation_CountryId",
                table: "CountryTranslation",
                column: "CountryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CountryTranslation");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 123, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 124, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 125, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 126, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 127, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 128, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 129, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 130, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 131, 1 });

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 125);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 126);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 127);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 128);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 129);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 130);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 131);

            migrationBuilder.AddColumn<string>(
                name: "DriverAddress",
                table: "Reservation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverPostalCode",
                table: "Reservation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverPostalLocation",
                table: "Reservation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverVatNumber",
                table: "Reservation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 109,
                column: "Key",
                value: "Reserva.Detalhe.Dados.Titulo");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 110,
                column: "Key",
                value: "Reserva.Detalhe.Dados.Nome");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 111,
                column: "Key",
                value: "Reserva.Detalhe.Dados.Email");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 112,
                column: "Key",
                value: "Reserva.Detalhe.Dados.Telefone");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 113,
                column: "Key",
                value: "Reserva.Detalhe.Dados.Morada");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 114,
                column: "Key",
                value: "Reserva.Detalhe.Dados.CodigoPostal");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 115,
                column: "Key",
                value: "Reserva.Detalhe.Dados.LocalidadePostal");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 116,
                column: "Key",
                value: "Reserva.Detalhe.Dados.NumeroVoo");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 117,
                column: "Key",
                value: "Reserva.Detalhe.Dados.DataNascimento");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 118,
                column: "Key",
                value: "Reserva.Detalhe.Dados.Comentarios");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 119,
                column: "Key",
                value: "Reserva.Detalhe.Dados.NumeroIdentificacao");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 120,
                column: "Key",
                value: "Reserva.Detalhe.Dados.NumeroContribuinte");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 121,
                column: "Key",
                value: "Reserva.Detalhe.Dados.NumeroCartaConducao");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 122,
                column: "Key",
                value: "Reserva.Detalhe.Dados.ValidadeCartaConducao");
        }
    }
}
