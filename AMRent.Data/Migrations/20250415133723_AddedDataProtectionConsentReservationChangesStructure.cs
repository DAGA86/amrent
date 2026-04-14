using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedDataProtectionConsentReservationChangesStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataProtectionConsentReservationChange",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataProtectionConsentReservationId = table.Column<int>(type: "int", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangeDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionConsentReservationChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataProtectionConsentReservationChange_DataProtectionConsentReservation_DataProtectionConsentReservationId",
                        column: x => x.DataProtectionConsentReservationId,
                        principalTable: "DataProtectionConsentReservation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DataProtectionConsentReservationChange_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataProtectionConsentReservationChange_DataProtectionConsentReservationId",
                table: "DataProtectionConsentReservationChange",
                column: "DataProtectionConsentReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_DataProtectionConsentReservationChange_UserId",
                table: "DataProtectionConsentReservationChange",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataProtectionConsentReservationChange");
        }
    }
}
