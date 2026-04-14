using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class _20250506110254_AddViaVerdeMoprMerfStep6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "ViaVerdeMovements");

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "ViaVerdeMovements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ViaVerdeMovements_CountryId",
                table: "ViaVerdeMovements",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ViaVerdeMovements_Country_CountryId",
                table: "ViaVerdeMovements",
                column: "CountryId",
                principalTable: "Country",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ViaVerdeMovements_Country_CountryId",
                table: "ViaVerdeMovements");

            migrationBuilder.DropIndex(
                name: "IX_ViaVerdeMovements_CountryId",
                table: "ViaVerdeMovements");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "ViaVerdeMovements");

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "ViaVerdeMovements",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);
        }
    }
}
