using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class UpdateSeasonAndSeasonCategoriesSchema_DatesUTC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Season",
                newName: "StartDateUtc");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Season",
                newName: "EndDateUtc");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDateUtc",
                table: "Season",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "EndDateUtc",
                table: "Season",
                newName: "EndDate");
        }
    }
}
