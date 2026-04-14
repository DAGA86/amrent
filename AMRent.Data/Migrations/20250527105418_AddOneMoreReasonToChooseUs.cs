using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOneMoreReasonToChooseUs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ReasonToChooseUs",
                columns: new[] { "Id", "FontAwesomeIconCode" },
                values: new object[] { 4, "" });

            migrationBuilder.InsertData(
                table: "ReasonToChooseUsTranslation",
                columns: new[] { "LanguageId", "ReasonToChooseUsId", "Text", "Title" },
                values: new object[] { 1, 4, "", "Razão 4" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ReasonToChooseUsTranslation",
                keyColumns: new[] { "LanguageId", "ReasonToChooseUsId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "ReasonToChooseUs",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
