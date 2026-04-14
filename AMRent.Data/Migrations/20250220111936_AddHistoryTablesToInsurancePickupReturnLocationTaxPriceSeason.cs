using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoryTablesToInsurancePickupReturnLocationTaxPriceSeason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InsuranceChange",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarSegmentId = table.Column<int>(type: "int", nullable: false),
                    InsuranceLevelId = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceChange_Insurance_CarSegmentId_InsuranceLevelId",
                        columns: x => new { x.CarSegmentId, x.InsuranceLevelId },
                        principalTable: "Insurance",
                        principalColumns: new[] { "CarSegmentId", "InsuranceLevelId" });
                    table.ForeignKey(
                        name: "FK_InsuranceChange_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PickupReturnLocationTaxChange",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickupReturnLocationTaxId = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickupReturnLocationTaxChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickupReturnLocationTaxChange_PickupReturnLocationTax_PickupReturnLocationTaxId",
                        column: x => x.PickupReturnLocationTaxId,
                        principalTable: "PickupReturnLocationTax",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PickupReturnLocationTaxChange_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PriceChange",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PriceId = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceChange_Price_PriceId",
                        column: x => x.PriceId,
                        principalTable: "Price",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PriceChange_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeasonChange",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeasonId = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeasonChange_Season_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Season",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeasonChange_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EmailContent",
                columns: new[] { "Id", "Type" },
                values: new object[] { 10, 10 });

            migrationBuilder.InsertData(
                table: "GenericText",
                columns: new[] { "Id", "Key" },
                values: new object[,]
                {
                    { 211, "Login.EsqueceuPassword" },
                    { 212, "PasswordEsquecida.Titulo" },
                    { 213, "PasswordEsquecida.Instrucoes" },
                    { 214, "PasswordEsquecida.Email" },
                    { 215, "PasswordEsquecida.Botao" }
                });

            migrationBuilder.InsertData(
                table: "EmailContentTranslation",
                columns: new[] { "EmailContentId", "LanguageId", "Subject", "Text" },
                values: new object[] { 10, 1, "", "" });

            migrationBuilder.InsertData(
                table: "GenericTextTranslation",
                columns: new[] { "GenericTextId", "LanguageId", "Value" },
                values: new object[,]
                {
                    { 211, 1, "Esqueceu a password?" },
                    { 212, 1, "Recuperação de password" },
                    { 213, 1, "Diga-nos o seu email e enviaremos um link para redefinir a sua password." },
                    { 214, 1, "Email" },
                    { 215, 1, "Submeter" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceChange_CarSegmentId_InsuranceLevelId",
                table: "InsuranceChange",
                columns: new[] { "CarSegmentId", "InsuranceLevelId" });

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceChange_UserId",
                table: "InsuranceChange",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PickupReturnLocationTaxChange_PickupReturnLocationTaxId",
                table: "PickupReturnLocationTaxChange",
                column: "PickupReturnLocationTaxId");

            migrationBuilder.CreateIndex(
                name: "IX_PickupReturnLocationTaxChange_UserId",
                table: "PickupReturnLocationTaxChange",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceChange_PriceId",
                table: "PriceChange",
                column: "PriceId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceChange_UserId",
                table: "PriceChange",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonChange_SeasonId",
                table: "SeasonChange",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonChange_UserId",
                table: "SeasonChange",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InsuranceChange");

            migrationBuilder.DropTable(
                name: "PickupReturnLocationTaxChange");

            migrationBuilder.DropTable(
                name: "PriceChange");

            migrationBuilder.DropTable(
                name: "SeasonChange");

            migrationBuilder.DeleteData(
                table: "EmailContentTranslation",
                keyColumns: new[] { "EmailContentId", "LanguageId" },
                keyValues: new object[] { 10, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 211, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 212, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 213, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 214, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 215, 1 });

            migrationBuilder.DeleteData(
                table: "EmailContent",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 211);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 212);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 213);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 214);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 215);
        }
    }
}
