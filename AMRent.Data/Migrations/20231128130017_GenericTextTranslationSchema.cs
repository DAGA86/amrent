using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class GenericTextTranslationSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenericTranslation");

            migrationBuilder.CreateTable(
                name: "GenericText",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericText", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GenericTextTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    GenericTextId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericTextTranslation", x => new { x.LanguageId, x.GenericTextId });
                    table.ForeignKey(
                        name: "FK_GenericTextTranslation_GenericText_GenericTextId",
                        column: x => x.GenericTextId,
                        principalTable: "GenericText",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenericTextTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GenericText_Key",
                table: "GenericText",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GenericTextTranslation_GenericTextId",
                table: "GenericTextTranslation",
                column: "GenericTextId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenericTextTranslation");

            migrationBuilder.DropTable(
                name: "GenericText");

            migrationBuilder.CreateTable(
                name: "GenericTranslation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GenericTranslation_LanguageId_Key",
                table: "GenericTranslation",
                columns: new[] { "LanguageId", "Key" },
                unique: true);
        }
    }
}
