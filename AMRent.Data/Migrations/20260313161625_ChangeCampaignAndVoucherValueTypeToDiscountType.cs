using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCampaignAndVoucherValueTypeToDiscountType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValueUnit",
                table: "Voucher",
                newName: "DiscountType");

            migrationBuilder.RenameColumn(
                name: "ValueUnit",
                table: "Campaign",
                newName: "DiscountType");

            migrationBuilder.AlterColumn<int>(
                name: "DiscountType",
                table: "Voucher",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DiscountType",
                table: "Campaign",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscountType",
                table: "Voucher",
                newName: "ValueUnit");

            migrationBuilder.RenameColumn(
                name: "DiscountType",
                table: "Campaign",
                newName: "ValueUnit");

            migrationBuilder.AlterColumn<int>(
                name: "ValueUnit",
                table: "Voucher",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ValueUnit",
                table: "Campaign",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
