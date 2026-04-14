using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class _202410022 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotation_Language_LanguageId",
                table: "Quotation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Language_LanguageId",
                table: "Reservation");

            migrationBuilder.AlterColumn<int>(
                name: "LanguageId",
                table: "Reservation",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LanguageId",
                table: "Quotation",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotation_Language_LanguageId",
                table: "Quotation",
                column: "LanguageId",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Language_LanguageId",
                table: "Reservation",
                column: "LanguageId",
                principalTable: "Language",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.AlterColumn<int>(
                name: "LanguageId",
                table: "Reservation",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "LanguageId",
                table: "Quotation",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

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
    }
}
