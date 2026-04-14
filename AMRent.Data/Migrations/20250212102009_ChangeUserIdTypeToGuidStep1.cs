using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserIdTypeToGuidStep1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "User",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "RoleUser",
                newName: "User2Id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ReservationChange",
                newName: "User2Id");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Reservation",
                newName: "Customer2Id");

            migrationBuilder.RenameColumn(
                name: "AssignedUserId",
                table: "Reservation",
                newName: "AssignedUser2Id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "QuotationChange",
                newName: "User2Id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Quotation",
                newName: "User2Id");

            migrationBuilder.InsertData(
                table: "GenericText",
                columns: new[] { "Id", "Key" },
                values: new object[,]
                {
                    { 209, "Header.MinhaConta" },
                    { 210, "Header.Logout" }
                });

            migrationBuilder.InsertData(
                table: "GenericTextTranslation",
                columns: new[] { "GenericTextId", "LanguageId", "Value" },
                values: new object[,]
                {
                    { 209, 1, "My AMC" },
                    { 210, 1, "Sair" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 209, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 210, 1 });

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 209);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 210);

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "User",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "User2Id",
                table: "RoleUser",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "User2Id",
                table: "ReservationChange",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Customer2Id",
                table: "Reservation",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "AssignedUser2Id",
                table: "Reservation",
                newName: "AssignedUserId");

            migrationBuilder.RenameColumn(
                name: "User2Id",
                table: "QuotationChange",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "User2Id",
                table: "Quotation",
                newName: "UserId");

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
    }
}
