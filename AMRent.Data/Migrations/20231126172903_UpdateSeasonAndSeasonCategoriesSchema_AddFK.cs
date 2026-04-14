using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class UpdateSeasonAndSeasonCategoriesSchema_AddFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Season_SeasonCategory_SeasonCategoryId",
                table: "Season");

            migrationBuilder.AlterColumn<int>(
                name: "SeasonCategoryId",
                table: "Season",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Season_SeasonCategory_SeasonCategoryId",
                table: "Season",
                column: "SeasonCategoryId",
                principalTable: "SeasonCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Season_SeasonCategory_SeasonCategoryId",
                table: "Season");

            migrationBuilder.AlterColumn<int>(
                name: "SeasonCategoryId",
                table: "Season",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Season_SeasonCategory_SeasonCategoryId",
                table: "Season",
                column: "SeasonCategoryId",
                principalTable: "SeasonCategory",
                principalColumn: "Id");
        }
    }
}
