using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPickupReturnTemporaryTax : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PickupReturnTemporaryTax",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    Value = table.Column<decimal>(type: "decimal(7,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickupReturnTemporaryTax", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PickupReturnTemporaryTaxChange",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickupReturnTemporaryTaxId = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickupReturnTemporaryTaxChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickupReturnTemporaryTaxChange_PickupReturnTemporaryTax_PickupReturnTemporaryTaxId",
                        column: x => x.PickupReturnTemporaryTaxId,
                        principalTable: "PickupReturnTemporaryTax",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PickupReturnTemporaryTaxChange_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PickupReturnTemporaryTaxTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    PickupReturnTemporaryTaxId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickupReturnTemporaryTaxTranslation", x => new { x.LanguageId, x.PickupReturnTemporaryTaxId });
                    table.ForeignKey(
                        name: "FK_PickupReturnTemporaryTaxTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PickupReturnTemporaryTaxTranslation_PickupReturnTemporaryTax_PickupReturnTemporaryTaxId",
                        column: x => x.PickupReturnTemporaryTaxId,
                        principalTable: "PickupReturnTemporaryTax",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuotationItemPickupReturnTemporaryTax",
                columns: table => new
                {
                    QuotationItemId = table.Column<int>(type: "int", nullable: false),
                    PickupReturnTemporaryTaxId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitValue = table.Column<decimal>(type: "decimal(7,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationItemPickupReturnTemporaryTax", x => new { x.QuotationItemId, x.PickupReturnTemporaryTaxId });
                    table.ForeignKey(
                        name: "FK_QuotationItemPickupReturnTemporaryTax_PickupReturnTemporaryTax_PickupReturnTemporaryTaxId",
                        column: x => x.PickupReturnTemporaryTaxId,
                        principalTable: "PickupReturnTemporaryTax",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationItemPickupReturnTemporaryTax_QuotationItem_QuotationItemId",
                        column: x => x.QuotationItemId,
                        principalTable: "QuotationItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReservationPickupReturnTemporaryTax",
                columns: table => new
                {
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    PickupReturnTemporaryTaxId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitValue = table.Column<decimal>(type: "decimal(7,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationPickupReturnTemporaryTax", x => new { x.ReservationId, x.PickupReturnTemporaryTaxId });
                    table.ForeignKey(
                        name: "FK_ReservationPickupReturnTemporaryTax_PickupReturnTemporaryTax_PickupReturnTemporaryTaxId",
                        column: x => x.PickupReturnTemporaryTaxId,
                        principalTable: "PickupReturnTemporaryTax",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationPickupReturnTemporaryTax_Reservation_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PickupReturnTemporaryTaxChange_PickupReturnTemporaryTaxId",
                table: "PickupReturnTemporaryTaxChange",
                column: "PickupReturnTemporaryTaxId");

            migrationBuilder.CreateIndex(
                name: "IX_PickupReturnTemporaryTaxChange_UserId",
                table: "PickupReturnTemporaryTaxChange",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PickupReturnTemporaryTaxTranslation_PickupReturnTemporaryTaxId",
                table: "PickupReturnTemporaryTaxTranslation",
                column: "PickupReturnTemporaryTaxId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItemPickupReturnTemporaryTax_PickupReturnTemporaryTaxId",
                table: "QuotationItemPickupReturnTemporaryTax",
                column: "PickupReturnTemporaryTaxId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationPickupReturnTemporaryTax_PickupReturnTemporaryTaxId",
                table: "ReservationPickupReturnTemporaryTax",
                column: "PickupReturnTemporaryTaxId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PickupReturnTemporaryTaxChange");

            migrationBuilder.DropTable(
                name: "PickupReturnTemporaryTaxTranslation");

            migrationBuilder.DropTable(
                name: "QuotationItemPickupReturnTemporaryTax");

            migrationBuilder.DropTable(
                name: "ReservationPickupReturnTemporaryTax");

            migrationBuilder.DropTable(
                name: "PickupReturnTemporaryTax");
        }
    }
}
