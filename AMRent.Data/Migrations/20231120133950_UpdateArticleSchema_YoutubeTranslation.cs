using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class UpdateArticleSchema_YoutubeTranslation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YoutubeURL",
                table: "Article");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "ArticleTranslation",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "YoutubeURL",
                table: "ArticleTranslation",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "ArticleTranslation");

            migrationBuilder.DropColumn(
                name: "YoutubeURL",
                table: "ArticleTranslation");

            migrationBuilder.AddColumn<string>(
                name: "YoutubeURL",
                table: "Article",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");
        }
    }
}
