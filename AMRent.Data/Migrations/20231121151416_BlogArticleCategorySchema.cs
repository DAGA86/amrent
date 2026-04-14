using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class BlogArticleCategorySchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleTranslation");

            migrationBuilder.DropTable(
                name: "Article");

            migrationBuilder.DeleteData(
                table: "ProcessTranslation",
                keyColumns: new[] { "LanguageId", "ProcessId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "Process",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.CreateTable(
                name: "BlogArticleCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogArticleCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogArticle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogArticleCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogArticle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogArticle_BlogArticleCategory_BlogArticleCategoryId",
                        column: x => x.BlogArticleCategoryId,
                        principalTable: "BlogArticleCategory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BlogArticleCategoryTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    BlogArticleCategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogArticleCategoryTranslation", x => new { x.LanguageId, x.BlogArticleCategoryId });
                    table.ForeignKey(
                        name: "FK_BlogArticleCategoryTranslation_BlogArticleCategory_BlogArticleCategoryId",
                        column: x => x.BlogArticleCategoryId,
                        principalTable: "BlogArticleCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogArticleCategoryTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlogArticleTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    YoutubeURL = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    BlogArticleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogArticleTranslation", x => new { x.LanguageId, x.ArticleId });
                    table.ForeignKey(
                        name: "FK_BlogArticleTranslation_BlogArticle_BlogArticleId",
                        column: x => x.BlogArticleId,
                        principalTable: "BlogArticle",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BlogArticleTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogArticle_BlogArticleCategoryId",
                table: "BlogArticle",
                column: "BlogArticleCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogArticleCategoryTranslation_BlogArticleCategoryId",
                table: "BlogArticleCategoryTranslation",
                column: "BlogArticleCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogArticleTranslation_BlogArticleId",
                table: "BlogArticleTranslation",
                column: "BlogArticleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogArticleCategoryTranslation");

            migrationBuilder.DropTable(
                name: "BlogArticleTranslation");

            migrationBuilder.DropTable(
                name: "BlogArticle");

            migrationBuilder.DropTable(
                name: "BlogArticleCategory");

            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticleTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    YoutubeURL = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleTranslation", x => new { x.LanguageId, x.ArticleId });
                    table.ForeignKey(
                        name: "FK_ArticleTranslation_Article_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Article",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Process",
                columns: new[] { "Id", "FontAwesomeIconCode" },
                values: new object[] { 4, "" });

            migrationBuilder.InsertData(
                table: "ProcessTranslation",
                columns: new[] { "LanguageId", "ProcessId", "Text", "Title" },
                values: new object[] { 1, 4, "", "Processo 4" });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleTranslation_ArticleId",
                table: "ArticleTranslation",
                column: "ArticleId");
        }
    }
}
