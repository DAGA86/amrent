using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class BlogArticleSchema_FixTranslationsForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogArticleTranslation_BlogArticle_BlogArticleId",
                table: "BlogArticleTranslation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogArticleTranslation",
                table: "BlogArticleTranslation");

            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "BlogArticleTranslation");

            migrationBuilder.AlterColumn<int>(
                name: "BlogArticleId",
                table: "BlogArticleTranslation",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogArticleTranslation",
                table: "BlogArticleTranslation",
                columns: new[] { "LanguageId", "BlogArticleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BlogArticleTranslation_BlogArticle_BlogArticleId",
                table: "BlogArticleTranslation",
                column: "BlogArticleId",
                principalTable: "BlogArticle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogArticleTranslation_BlogArticle_BlogArticleId",
                table: "BlogArticleTranslation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogArticleTranslation",
                table: "BlogArticleTranslation");

            migrationBuilder.AlterColumn<int>(
                name: "BlogArticleId",
                table: "BlogArticleTranslation",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ArticleId",
                table: "BlogArticleTranslation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogArticleTranslation",
                table: "BlogArticleTranslation",
                columns: new[] { "LanguageId", "ArticleId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BlogArticleTranslation_BlogArticle_BlogArticleId",
                table: "BlogArticleTranslation",
                column: "BlogArticleId",
                principalTable: "BlogArticle",
                principalColumn: "Id");
        }
    }
}
