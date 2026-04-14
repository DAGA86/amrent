using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202407291 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuotationId",
                table: "ReservationExtra",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Quotation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 10),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    ExpireDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PickupLocationId = table.Column<int>(type: "int", nullable: false),
                    PickupDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnLocationId = table.Column<int>(type: "int", nullable: false),
                    ReturnDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerEmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerTelephonePrefixCountryId = table.Column<int>(type: "int", nullable: true),
                    CustomerTelephone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quotation_Country_CustomerTelephonePrefixCountryId",
                        column: x => x.CustomerTelephonePrefixCountryId,
                        principalTable: "Country",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Quotation_PickupReturnLocation_PickupLocationId",
                        column: x => x.PickupLocationId,
                        principalTable: "PickupReturnLocation",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Quotation_PickupReturnLocation_ReturnLocationId",
                        column: x => x.ReturnLocationId,
                        principalTable: "PickupReturnLocation",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Quotation_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QuotationItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuotationId = table.Column<int>(type: "int", nullable: false),
                    CarSegmentId = table.Column<int>(type: "int", nullable: false),
                    CampaignId = table.Column<int>(type: "int", nullable: true),
                    VoucherId = table.Column<int>(type: "int", nullable: true),
                    InsuranceLevelId = table.Column<int>(type: "int", nullable: false),
                    CarSegmentCost = table.Column<decimal>(type: "decimal(7,2)", nullable: false),
                    PickupCost = table.Column<decimal>(type: "decimal(7,2)", nullable: false),
                    ReturnCost = table.Column<decimal>(type: "decimal(7,2)", nullable: false),
                    InsuranceCost = table.Column<decimal>(type: "decimal(7,2)", nullable: false),
                    InsuranceExcess = table.Column<decimal>(type: "decimal(7,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(7,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuotationItem_Campaign_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaign",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuotationItem_CarSegment_CarSegmentId",
                        column: x => x.CarSegmentId,
                        principalTable: "CarSegment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationItem_InsuranceLevel_InsuranceLevelId",
                        column: x => x.InsuranceLevelId,
                        principalTable: "InsuranceLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationItem_Voucher_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Voucher",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationExtra_QuotationId",
                table: "ReservationExtra",
                column: "QuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_CustomerTelephonePrefixCountryId",
                table: "Quotation",
                column: "CustomerTelephonePrefixCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_PickupLocationId",
                table: "Quotation",
                column: "PickupLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_ReturnLocationId",
                table: "Quotation",
                column: "ReturnLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_UserId",
                table: "Quotation",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItem_CampaignId",
                table: "QuotationItem",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItem_CarSegmentId",
                table: "QuotationItem",
                column: "CarSegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItem_InsuranceLevelId",
                table: "QuotationItem",
                column: "InsuranceLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItem_VoucherId",
                table: "QuotationItem",
                column: "VoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationExtra_Quotation_QuotationId",
                table: "ReservationExtra",
                column: "QuotationId",
                principalTable: "Quotation",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationExtra_Quotation_QuotationId",
                table: "ReservationExtra");

            migrationBuilder.DropTable(
                name: "Quotation");

            migrationBuilder.DropTable(
                name: "QuotationItem");

            migrationBuilder.DropIndex(
                name: "IX_ReservationExtra_QuotationId",
                table: "ReservationExtra");

            migrationBuilder.DropColumn(
                name: "QuotationId",
                table: "ReservationExtra");
        }
    }
}
