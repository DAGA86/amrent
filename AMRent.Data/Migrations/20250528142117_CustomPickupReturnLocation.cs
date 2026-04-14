using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class CustomPickupReturnLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomPickupLocationName",
                table: "Reservation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomReturnLocationName",
                table: "Reservation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomPickupLocationName",
                table: "Quotation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomReturnLocationName",
                table: "Quotation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "PickupReturnLocation",
                columns: new[] { "Id", "IsAlwaysAvailableForPickupAndReturn", "IsSelectedByDefault", "IsWorkingOffice", "MinimumAnticipationMinutes" },
                values: new object[] { -1, true, false, false, 2880 });

            migrationBuilder.InsertData(
                table: "PickupReturnLocationTranslation",
                columns: new[] { "LanguageId", "PickupReturnLocationId", "Name" },
                values: new object[,]
                {
                    { 1, -1, "Outro" },
                    { 2, -1, "Other" },
                    { 3, -1, "Autre" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PickupReturnLocationTranslation",
                keyColumns: new[] { "LanguageId", "PickupReturnLocationId" },
                keyValues: new object[] { 1, -1 });

            migrationBuilder.DeleteData(
                table: "PickupReturnLocationTranslation",
                keyColumns: new[] { "LanguageId", "PickupReturnLocationId" },
                keyValues: new object[] { 2, -1 });

            migrationBuilder.DeleteData(
                table: "PickupReturnLocationTranslation",
                keyColumns: new[] { "LanguageId", "PickupReturnLocationId" },
                keyValues: new object[] { 3, -1 });

            migrationBuilder.DeleteData(
                table: "PickupReturnLocation",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.DropColumn(
                name: "CustomPickupLocationName",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "CustomReturnLocationName",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "CustomPickupLocationName",
                table: "Quotation");

            migrationBuilder.DropColumn(
                name: "CustomReturnLocationName",
                table: "Quotation");
        }
    }
}
