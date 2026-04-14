using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class ReservationSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reservation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarSegmentId = table.Column<int>(type: "int", nullable: false),
                    CarSegmentValue = table.Column<decimal>(type: "decimal(7,2)", nullable: false),
                    InsuranceValue = table.Column<decimal>(type: "decimal(7,2)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(7,2)", nullable: false),
                    CampaignId = table.Column<int>(type: "int", nullable: true),
                    VoucherId = table.Column<int>(type: "int", nullable: true),
                    InsuranceLevelId = table.Column<int>(type: "int", nullable: false),
                    PickupLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PickupDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PickupTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ReturnLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    DriverName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DriverBirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DriverEmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DriverTelephone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DriverAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DriverPostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DriverPostalLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DriverIdNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DriverVatNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DriverLicenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DriverLicenceExpireDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BillName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillEmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillTelephone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillPostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillPostalLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillVatNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlightNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservation_Campaign_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaign",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reservation_CarSegment_CarSegmentId",
                        column: x => x.CarSegmentId,
                        principalTable: "CarSegment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservation_Voucher_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Voucher",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReservationExtra",
                columns: table => new
                {
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    ExtraId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(7,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationExtra", x => new { x.ExtraId, x.ReservationId });
                    table.ForeignKey(
                        name: "FK_ReservationExtra_Extra_ExtraId",
                        column: x => x.ExtraId,
                        principalTable: "Extra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationExtra_Reservation_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_CampaignId",
                table: "Reservation",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_CarSegmentId",
                table: "Reservation",
                column: "CarSegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_VoucherId",
                table: "Reservation",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationExtra_ReservationId",
                table: "ReservationExtra",
                column: "ReservationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservationExtra");

            migrationBuilder.DropTable(
                name: "Reservation");
        }
    }
}
