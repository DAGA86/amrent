using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class _202410021 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LanguageId",
                table: "Reservation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LanguageId",
                table: "Quotation",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_LanguageId",
                table: "Reservation",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_LanguageId",
                table: "Quotation",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotation_Language_LanguageId",
                table: "Quotation",
                column: "LanguageId",
                principalTable: "Language",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Language_LanguageId",
                table: "Reservation",
                column: "LanguageId",
                principalTable: "Language",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotation_Language_LanguageId",
                table: "Quotation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Language_LanguageId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_LanguageId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Quotation_LanguageId",
                table: "Quotation");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "Quotation");
        }
    }
}
