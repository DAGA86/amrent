using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class _202501241 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EmailContent",
                columns: new[] { "Id", "Type" },
                values: new object[] { 9, 9 });

            migrationBuilder.InsertData(
                table: "EmailContentTranslation",
                columns: new[] { "EmailContentId", "LanguageId", "Subject", "Text" },
                values: new object[] { 9, 1, "", "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmailContentTranslation",
                keyColumns: new[] { "EmailContentId", "LanguageId" },
                keyValues: new object[] { 9, 1 });

            migrationBuilder.DeleteData(
                table: "EmailContent",
                keyColumn: "Id",
                keyValue: 9);
        }
    }
}
