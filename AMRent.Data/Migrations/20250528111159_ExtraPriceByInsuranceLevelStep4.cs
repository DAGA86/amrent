using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtraPriceByInsuranceLevelStep4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaximumValue",
                table: "Extra");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "Extra");

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "ExtraPriceByInsuranceLevel",
                type: "decimal(7,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(7,2)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "ExtraPriceByInsuranceLevel",
                type: "decimal(7,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(7,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumValue",
                table: "Extra",
                type: "decimal(7,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "Extra",
                type: "decimal(7,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
