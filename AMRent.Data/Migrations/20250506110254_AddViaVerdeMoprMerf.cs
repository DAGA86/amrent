using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddViaVerdeMoprMerf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessedFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ViaVerdeMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    ManufacturerCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    EquipmentNumber = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    ContractLicencePlate = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    TariffClass = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    ExitTollCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    ExitTollName = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    ExitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntryTollCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    EntryTollName = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TariffInCents = table.Column<int>(type: "int", nullable: false),
                    TransactionCode = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    VatCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    ServiceCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    ResultCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Town = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    PostalLocation = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    IdentityNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CollectionAttemptedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViaVerdeMovements", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessedFiles");

            migrationBuilder.DropTable(
                name: "ViaVerdeMovements");
        }
    }
}
