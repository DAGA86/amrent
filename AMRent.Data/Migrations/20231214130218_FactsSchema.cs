using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class FactsSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fact",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<int>(type: "int", nullable: false),
                    FontAwesomeIconCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fact", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FactTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    FactId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactTranslation", x => new { x.LanguageId, x.FactId });
                    table.ForeignKey(
                        name: "FK_FactTranslation_Fact_FactId",
                        column: x => x.FactId,
                        principalTable: "Fact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FactTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Fact",
                columns: new[] { "Id", "FontAwesomeIconCode", "Number" },
                values: new object[,]
                {
                    { 1, "", 100 },
                    { 2, "", 100 },
                    { 3, "", 100 },
                    { 4, "", 100 }
                });

            migrationBuilder.InsertData(
                table: "FactTranslation",
                columns: new[] { "FactId", "LanguageId", "Text", "Title" },
                values: new object[,]
                {
                    { 1, 1, "", "Facto 1" },
                    { 2, 1, "", "Facto 2" },
                    { 3, 1, "", "Facto 3" },
                    { 4, 1, "", "Facto 4" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FactTranslation_FactId",
                table: "FactTranslation",
                column: "FactId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FactTranslation");

            migrationBuilder.DropTable(
                name: "Fact");
        }
    }
}
