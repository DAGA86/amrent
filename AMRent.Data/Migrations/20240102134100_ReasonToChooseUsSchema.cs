using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class ReasonToChooseUsSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReasonToChooseUs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FontAwesomeIconCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReasonToChooseUs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReasonToChooseUsTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    ReasonToChooseUsId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReasonToChooseUsTranslation", x => new { x.LanguageId, x.ReasonToChooseUsId });
                    table.ForeignKey(
                        name: "FK_ReasonToChooseUsTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReasonToChooseUsTranslation_ReasonToChooseUs_ReasonToChooseUsId",
                        column: x => x.ReasonToChooseUsId,
                        principalTable: "ReasonToChooseUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ReasonToChooseUs",
                columns: new[] { "Id", "FontAwesomeIconCode" },
                values: new object[] { 1, "" });

            migrationBuilder.InsertData(
                table: "ReasonToChooseUs",
                columns: new[] { "Id", "FontAwesomeIconCode" },
                values: new object[] { 2, "" });

            migrationBuilder.InsertData(
                table: "ReasonToChooseUs",
                columns: new[] { "Id", "FontAwesomeIconCode" },
                values: new object[] { 3, "" });

            migrationBuilder.InsertData(
                table: "ReasonToChooseUsTranslation",
                columns: new[] { "LanguageId", "ReasonToChooseUsId", "Text", "Title" },
                values: new object[] { 1, 1, "", "Razão 1" });

            migrationBuilder.InsertData(
                table: "ReasonToChooseUsTranslation",
                columns: new[] { "LanguageId", "ReasonToChooseUsId", "Text", "Title" },
                values: new object[] { 1, 2, "", "Razão 2" });

            migrationBuilder.InsertData(
                table: "ReasonToChooseUsTranslation",
                columns: new[] { "LanguageId", "ReasonToChooseUsId", "Text", "Title" },
                values: new object[] { 1, 3, "", "Razão 3" });

            migrationBuilder.CreateIndex(
                name: "IX_ReasonToChooseUsTranslation_ReasonToChooseUsId",
                table: "ReasonToChooseUsTranslation",
                column: "ReasonToChooseUsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReasonToChooseUsTranslation");

            migrationBuilder.DropTable(
                name: "ReasonToChooseUs");
        }
    }
}
