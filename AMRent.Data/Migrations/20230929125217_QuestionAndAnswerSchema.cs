using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class QuestionAndAnswerSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuestionAndAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAndAnswer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionAndAnswerTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    QuestionAndAnswerId = table.Column<int>(type: "int", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAndAnswerTranslation", x => new { x.LanguageId, x.QuestionAndAnswerId });
                    table.ForeignKey(
                        name: "FK_QuestionAndAnswerTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionAndAnswerTranslation_QuestionAndAnswer_QuestionAndAnswerId",
                        column: x => x.QuestionAndAnswerId,
                        principalTable: "QuestionAndAnswer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAndAnswerTranslation_QuestionAndAnswerId",
                table: "QuestionAndAnswerTranslation",
                column: "QuestionAndAnswerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionAndAnswerTranslation");

            migrationBuilder.DropTable(
                name: "QuestionAndAnswer");
        }
    }
}
