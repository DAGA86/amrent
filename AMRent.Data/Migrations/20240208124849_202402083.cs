using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202402083 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubTitle",
                table: "HomeBannerTranslation");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "HomeBannerTranslation");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "HomeBannerTranslation");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "HomeBanner",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "HomeBanner");

            migrationBuilder.AddColumn<string>(
                name: "SubTitle",
                table: "HomeBannerTranslation",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "HomeBannerTranslation",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "HomeBannerTranslation",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }
    }
}
