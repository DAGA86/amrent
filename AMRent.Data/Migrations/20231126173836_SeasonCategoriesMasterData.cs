using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class SeasonCategoriesMasterData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SeasonCategory",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Verão" });

            migrationBuilder.InsertData(
                table: "SeasonCategory",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Inverno" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SeasonCategory",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SeasonCategory",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
