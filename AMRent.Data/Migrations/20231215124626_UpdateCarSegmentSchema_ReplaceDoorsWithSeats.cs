using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class UpdateCarSegmentSchema_ReplaceDoorsWithSeats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "FactTranslation");

            migrationBuilder.RenameColumn(
                name: "Doors",
                table: "CarSegment",
                newName: "Seats");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Seats",
                table: "CarSegment",
                newName: "Doors");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "FactTranslation",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "FactTranslation",
                keyColumns: new[] { "FactId", "LanguageId" },
                keyValues: new object[] { 1, 1 },
                column: "Text",
                value: "");

            migrationBuilder.UpdateData(
                table: "FactTranslation",
                keyColumns: new[] { "FactId", "LanguageId" },
                keyValues: new object[] { 2, 1 },
                column: "Text",
                value: "");

            migrationBuilder.UpdateData(
                table: "FactTranslation",
                keyColumns: new[] { "FactId", "LanguageId" },
                keyValues: new object[] { 3, 1 },
                column: "Text",
                value: "");

            migrationBuilder.UpdateData(
                table: "FactTranslation",
                keyColumns: new[] { "FactId", "LanguageId" },
                keyValues: new object[] { 4, 1 },
                column: "Text",
                value: "");
        }
    }
}
