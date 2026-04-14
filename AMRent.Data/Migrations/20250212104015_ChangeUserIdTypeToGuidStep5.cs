using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserIdTypeToGuidStep5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RoleUser_RoleId",
                table: "RoleUser",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationChange_UserId",
                table: "ReservationChange",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_AssignedUserId",
                table: "Reservation",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_CustomerId",
                table: "Reservation",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationChange_UserId",
                table: "QuotationChange",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_UserId",
                table: "Quotation",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotation_User_UserId",
                table: "Quotation",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationChange_User_UserId",
                table: "QuotationChange",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_User_AssignedUserId",
                table: "Reservation",
                column: "AssignedUserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_User_CustomerId",
                table: "Reservation",
                column: "CustomerId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationChange_User_UserId",
                table: "ReservationChange",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleUser_Role_RoleId",
                table: "RoleUser",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleUser_User_UserId",
                table: "RoleUser",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotation_User_UserId",
                table: "Quotation");

            migrationBuilder.DropForeignKey(
                name: "FK_QuotationChange_User_UserId",
                table: "QuotationChange");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_User_AssignedUserId",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_User_CustomerId",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationChange_User_UserId",
                table: "ReservationChange");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleUser_Role_RoleId",
                table: "RoleUser");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleUser_User_UserId",
                table: "RoleUser");

            migrationBuilder.DropIndex(
                name: "IX_RoleUser_RoleId",
                table: "RoleUser");

            migrationBuilder.DropIndex(
                name: "IX_ReservationChange_UserId",
                table: "ReservationChange");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_AssignedUserId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_CustomerId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_QuotationChange_UserId",
                table: "QuotationChange");

            migrationBuilder.DropIndex(
                name: "IX_Quotation_UserId",
                table: "Quotation");
        }
    }
}
