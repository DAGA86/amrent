using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedDataProtectionConsentStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataProtectionConsent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionConsent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataProtectionConsentReservation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HasConsented = table.Column<bool>(type: "bit", nullable: false),
                    DataProtectionConsentId = table.Column<int>(type: "int", nullable: false),
                    ReservationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionConsentReservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataProtectionConsentReservation_DataProtectionConsent_DataProtectionConsentId",
                        column: x => x.DataProtectionConsentId,
                        principalTable: "DataProtectionConsent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DataProtectionConsentReservation_Reservation_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataProtectionConsentTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    DataProtectionConsentId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionConsentTranslation", x => new { x.LanguageId, x.DataProtectionConsentId });
                    table.ForeignKey(
                        name: "FK_DataProtectionConsentTranslation_DataProtectionConsent_DataProtectionConsentId",
                        column: x => x.DataProtectionConsentId,
                        principalTable: "DataProtectionConsent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DataProtectionConsentTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataProtectionConsentUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HasConsented = table.Column<bool>(type: "bit", nullable: false),
                    DataProtectionConsentId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionConsentUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataProtectionConsentUser_DataProtectionConsent_DataProtectionConsentId",
                        column: x => x.DataProtectionConsentId,
                        principalTable: "DataProtectionConsent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DataProtectionConsentUser_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataProtectionConsentReservation_DataProtectionConsentId",
                table: "DataProtectionConsentReservation",
                column: "DataProtectionConsentId");

            migrationBuilder.CreateIndex(
                name: "IX_DataProtectionConsentReservation_ReservationId",
                table: "DataProtectionConsentReservation",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_DataProtectionConsentTranslation_DataProtectionConsentId",
                table: "DataProtectionConsentTranslation",
                column: "DataProtectionConsentId");

            migrationBuilder.CreateIndex(
                name: "IX_DataProtectionConsentUser_DataProtectionConsentId",
                table: "DataProtectionConsentUser",
                column: "DataProtectionConsentId");

            migrationBuilder.CreateIndex(
                name: "IX_DataProtectionConsentUser_UserId",
                table: "DataProtectionConsentUser",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataProtectionConsentReservation");

            migrationBuilder.DropTable(
                name: "DataProtectionConsentTranslation");

            migrationBuilder.DropTable(
                name: "DataProtectionConsentUser");

            migrationBuilder.DropTable(
                name: "DataProtectionConsent");
        }
    }
}
