using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class _202410024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EmailContent",
                columns: new[] { "Id", "Type" },
                values: new object[,]
                {
                    { 7, 7 },
                    { 8, 8 }
                });

            migrationBuilder.InsertData(
                table: "EmailContentTranslation",
                columns: new[] { "EmailContentId", "LanguageId", "Subject", "Text" },
                values: new object[,]
                {
                    { 7, 1, "", "" },
                    { 8, 1, "", "" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmailContentTranslation",
                keyColumns: new[] { "EmailContentId", "LanguageId" },
                keyValues: new object[] { 7, 1 });

            migrationBuilder.DeleteData(
                table: "EmailContentTranslation",
                keyColumns: new[] { "EmailContentId", "LanguageId" },
                keyValues: new object[] { 8, 1 });

            migrationBuilder.DeleteData(
                table: "EmailContent",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "EmailContent",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
