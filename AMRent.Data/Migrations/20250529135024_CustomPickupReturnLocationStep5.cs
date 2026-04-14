using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class CustomPickupReturnLocationStep5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataProtectionConsentQuotation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HasConsented = table.Column<bool>(type: "bit", nullable: false),
                    DataProtectionConsentId = table.Column<int>(type: "int", nullable: false),
                    QuotationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionConsentQuotation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataProtectionConsentQuotation_DataProtectionConsent_DataProtectionConsentId",
                        column: x => x.DataProtectionConsentId,
                        principalTable: "DataProtectionConsent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DataProtectionConsentQuotation_Quotation_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "Quotation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataProtectionConsentQuotationChange",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataProtectionConsentQuotationId = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionConsentQuotationChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataProtectionConsentQuotationChange_DataProtectionConsentQuotation_DataProtectionConsentQuotationId",
                        column: x => x.DataProtectionConsentQuotationId,
                        principalTable: "DataProtectionConsentQuotation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DataProtectionConsentQuotationChange_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 52,
                column: "Key",
                value: "Segmento.Detalhe.Sidebar.BotaoReservar");

            migrationBuilder.InsertData(
                table: "GenericText",
                columns: new[] { "Id", "Key" },
                values: new object[,]
                {
                    { 244, "Segmento.Detalhe.Sidebar.BotaoPedirCotacao" },
                    { 245, "PedidoCotacao.Detalhe.Titulo" },
                    { 246, "PedidoCotacao.Detalhe.Botao" },
                    { 247, "PedidoCotacao.Confirmacao.Resumo.Titulo" },
                    { 248, "PedidoCotacao.Confirmacao.Resumo.Notas" },
                    { 249, "PedidoCotacao.Confirmacao.Resumo.Notas.Texto" }
                });

            migrationBuilder.InsertData(
                table: "GenericTextTranslation",
                columns: new[] { "GenericTextId", "LanguageId", "Value" },
                values: new object[,]
                {
                    { 244, 1, "Segmento.Detalhe.Sidebar.BotaoPedirCotacao" },
                    { 245, 1, "PedidoCotacao.Detalhe.Titulo" },
                    { 246, 1, "PedidoCotacao.Detalhe.Botao" },
                    { 247, 1, "PedidoCotacao.Confirmacao.Resumo.Titulo" },
                    { 248, 1, "PedidoCotacao.Confirmacao.Resumo.Notas" },
                    { 249, 1, "PedidoCotacao.Confirmacao.Resumo.Notas.Texto" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataProtectionConsentQuotation_DataProtectionConsentId",
                table: "DataProtectionConsentQuotation",
                column: "DataProtectionConsentId");

            migrationBuilder.CreateIndex(
                name: "IX_DataProtectionConsentQuotation_QuotationId",
                table: "DataProtectionConsentQuotation",
                column: "QuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_DataProtectionConsentQuotationChange_DataProtectionConsentQuotationId",
                table: "DataProtectionConsentQuotationChange",
                column: "DataProtectionConsentQuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_DataProtectionConsentQuotationChange_UserId",
                table: "DataProtectionConsentQuotationChange",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataProtectionConsentQuotationChange");

            migrationBuilder.DropTable(
                name: "DataProtectionConsentQuotation");

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 244, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 245, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 246, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 247, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 248, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 249, 1 });

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 244);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 245);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 246);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 247);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 248);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 249);

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 52,
                column: "Key",
                value: "Segmento.Detalhe.Sidebar.Botao");
        }
    }
}
