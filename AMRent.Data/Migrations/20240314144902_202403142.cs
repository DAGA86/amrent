using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202403142 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SundayTimetable",
                table: "OfficeLocation",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EmailContent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailContent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailContentTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    EmailContentId = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailContentTranslation", x => new { x.LanguageId, x.EmailContentId });
                    table.ForeignKey(
                        name: "FK_EmailContentTranslation_EmailContent_EmailContentId",
                        column: x => x.EmailContentId,
                        principalTable: "EmailContent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmailContentTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "GenericText",
                columns: new[] { "Id", "Key" },
                values: new object[] { 173, "Contactos.Escritorios.Horario.Domingo" });

            migrationBuilder.InsertData(
                table: "GenericTextTranslation",
                columns: new[] { "GenericTextId", "LanguageId", "Value" },
                values: new object[] { 173, 1, "Domingo" });

            migrationBuilder.CreateIndex(
                name: "IX_EmailContentTranslation_EmailContentId",
                table: "EmailContentTranslation",
                column: "EmailContentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailContentTranslation");

            migrationBuilder.DropTable(
                name: "EmailContent");

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 173, 1 });

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 173);

            migrationBuilder.DropColumn(
                name: "SundayTimetable",
                table: "OfficeLocation");
        }
    }
}
