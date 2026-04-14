using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202402081 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DriverLicenseDate",
                table: "Reservation",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReservationExtraDriver",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LicenseCountryId = table.Column<int>(type: "int", nullable: true),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LicenseExpireDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationExtraDriver", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationExtraDriver_Country_LicenseCountryId",
                        column: x => x.LicenseCountryId,
                        principalTable: "Country",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReservationExtraDriver_Reservation_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "GenericText",
                columns: new[] { "Id", "Key" },
                values: new object[,]
                {
                    { 141, "Reserva.Detalhe.DadosCondutor.DataEmissaoCartaConducao" },
                    { 142, "Reserva.Detalhe.DadosCondutoresExtra.Titulo" },
                    { 143, "Reserva.Detalhe.DadosCondutoresExtra.Nome" },
                    { 144, "Reserva.Detalhe.DadosCondutoresExtra.PaisCartaConducao" },
                    { 145, "Reserva.Detalhe.DadosCondutoresExtra.NumeroCartaConducao" },
                    { 146, "Reserva.Detalhe.DadosCondutoresExtra.DataEmissaoCartaConducao" },
                    { 147, "Reserva.Detalhe.DadosCondutoresExtra.ValidadeCartaConducao" }
                });

            migrationBuilder.InsertData(
                table: "GenericTextTranslation",
                columns: new[] { "GenericTextId", "LanguageId", "Value" },
                values: new object[,]
                {
                    { 141, 1, "Data de emissão" },
                    { 142, 1, "Dados do condutor Extra" },
                    { 143, 1, "Nome" },
                    { 144, 1, "Carta de condução" },
                    { 145, 1, "Número" },
                    { 146, 1, "Data de emissão" },
                    { 147, 1, "Validade" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationExtraDriver_LicenseCountryId",
                table: "ReservationExtraDriver",
                column: "LicenseCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationExtraDriver_ReservationId",
                table: "ReservationExtraDriver",
                column: "ReservationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservationExtraDriver");

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 141, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 142, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 143, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 144, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 145, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 146, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 147, 1 });

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 141);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 142);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 143);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 144);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 145);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 146);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 147);

            migrationBuilder.DropColumn(
                name: "DriverLicenseDate",
                table: "Reservation");
        }
    }
}
