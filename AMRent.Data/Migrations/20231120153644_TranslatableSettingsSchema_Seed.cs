using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class TranslatableSettingsSchema_Seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Language",
                columns: new[] { "Id", "Code", "CountryCode", "Name" },
                values: new object[] { 3, "fr", "fr", "Français" });

            migrationBuilder.InsertData(
                table: "TranslatableSettingTranslation",
                columns: new[] { "LanguageId", "TranslatableSettingId", "Text" },
                values: new object[,]
                {
                    { 1, 1, "" },
                    { 1, 2, "" },
                    { 1, 3, "" },
                    { 2, 1, "" },
                    { 2, 2, "" },
                    { 2, 3, "" }
                });

            migrationBuilder.InsertData(
                table: "TranslatableSettingTranslation",
                columns: new[] { "LanguageId", "TranslatableSettingId", "Text" },
                values: new object[] { 3, 1, "" });

            migrationBuilder.InsertData(
                table: "TranslatableSettingTranslation",
                columns: new[] { "LanguageId", "TranslatableSettingId", "Text" },
                values: new object[] { 3, 2, "" });

            migrationBuilder.InsertData(
                table: "TranslatableSettingTranslation",
                columns: new[] { "LanguageId", "TranslatableSettingId", "Text" },
                values: new object[] { 3, 3, "" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TranslatableSettingTranslation",
                keyColumns: new[] { "LanguageId", "TranslatableSettingId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "TranslatableSettingTranslation",
                keyColumns: new[] { "LanguageId", "TranslatableSettingId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "TranslatableSettingTranslation",
                keyColumns: new[] { "LanguageId", "TranslatableSettingId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "TranslatableSettingTranslation",
                keyColumns: new[] { "LanguageId", "TranslatableSettingId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "TranslatableSettingTranslation",
                keyColumns: new[] { "LanguageId", "TranslatableSettingId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "TranslatableSettingTranslation",
                keyColumns: new[] { "LanguageId", "TranslatableSettingId" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "TranslatableSettingTranslation",
                keyColumns: new[] { "LanguageId", "TranslatableSettingId" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "TranslatableSettingTranslation",
                keyColumns: new[] { "LanguageId", "TranslatableSettingId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "TranslatableSettingTranslation",
                keyColumns: new[] { "LanguageId", "TranslatableSettingId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "Language",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
