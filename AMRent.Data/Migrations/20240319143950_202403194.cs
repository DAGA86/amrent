using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202403194 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EmailContent",
                columns: new[] { "Id", "Type" },
                values: new object[] { 4, 4 });

            migrationBuilder.InsertData(
                table: "EmailContent",
                columns: new[] { "Id", "Type" },
                values: new object[] { 5, 5 });

            migrationBuilder.InsertData(
                table: "EmailContent",
                columns: new[] { "Id", "Type" },
                values: new object[] { 6, 3 });

            migrationBuilder.InsertData(
                table: "EmailContentTranslation",
                columns: new[] { "EmailContentId", "LanguageId", "Subject", "Text" },
                values: new object[] { 4, 1, "", "" });

            migrationBuilder.InsertData(
                table: "EmailContentTranslation",
                columns: new[] { "EmailContentId", "LanguageId", "Subject", "Text" },
                values: new object[] { 5, 1, "", "" });

            migrationBuilder.InsertData(
                table: "EmailContentTranslation",
                columns: new[] { "EmailContentId", "LanguageId", "Subject", "Text" },
                values: new object[] { 6, 1, "", "" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmailContentTranslation",
                keyColumns: new[] { "EmailContentId", "LanguageId" },
                keyValues: new object[] { 4, 1 });

            migrationBuilder.DeleteData(
                table: "EmailContentTranslation",
                keyColumns: new[] { "EmailContentId", "LanguageId" },
                keyValues: new object[] { 5, 1 });

            migrationBuilder.DeleteData(
                table: "EmailContentTranslation",
                keyColumns: new[] { "EmailContentId", "LanguageId" },
                keyValues: new object[] { 6, 1 });

            migrationBuilder.DeleteData(
                table: "EmailContent",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EmailContent",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "EmailContent",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
