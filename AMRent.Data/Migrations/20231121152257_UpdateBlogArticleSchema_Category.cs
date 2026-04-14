using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class UpdateBlogArticleSchema_Category : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogArticle_BlogArticleCategory_BlogArticleCategoryId",
                table: "BlogArticle");

            migrationBuilder.AlterColumn<int>(
                name: "BlogArticleCategoryId",
                table: "BlogArticle",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogArticle_BlogArticleCategory_BlogArticleCategoryId",
                table: "BlogArticle",
                column: "BlogArticleCategoryId",
                principalTable: "BlogArticleCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogArticle_BlogArticleCategory_BlogArticleCategoryId",
                table: "BlogArticle");

            migrationBuilder.AlterColumn<int>(
                name: "BlogArticleCategoryId",
                table: "BlogArticle",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogArticle_BlogArticleCategory_BlogArticleCategoryId",
                table: "BlogArticle",
                column: "BlogArticleCategoryId",
                principalTable: "BlogArticleCategory",
                principalColumn: "Id");
        }
    }
}
