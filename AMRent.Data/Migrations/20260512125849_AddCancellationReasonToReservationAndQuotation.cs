using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCancellationReasonToReservationAndQuotation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RejectionReason",
                table: "Quotation",
                newName: "CancellationReasonDescription");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReasonDescription",
                table: "Reservation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CancellationReasonId",
                table: "Reservation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Reservation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CancellationReasonId",
                table: "Quotation",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReservationQuotationCancellationReason",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationQuotationCancellationReason", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ReservationQuotationCancellationReason",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { 1, "Datas alteradas" },
                    { 2, "Voo cancelado" },
                    { 3, "Preço elevado" },
                    { 4, "Pagamento recusado" },
                    { 5, "Sem cartão de crédito" },
                    { 6, "Reserva duplicada" },
                    { 7, "Planos alterados" },
                    { 8, "Não necessita de viatura" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_CancellationReasonId",
                table: "Reservation",
                column: "CancellationReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_CancellationReasonId",
                table: "Quotation",
                column: "CancellationReasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotation_ReservationQuotationCancellationReason_CancellationReasonId",
                table: "Quotation",
                column: "CancellationReasonId",
                principalTable: "ReservationQuotationCancellationReason",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_ReservationQuotationCancellationReason_CancellationReasonId",
                table: "Reservation",
                column: "CancellationReasonId",
                principalTable: "ReservationQuotationCancellationReason",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotation_ReservationQuotationCancellationReason_CancellationReasonId",
                table: "Quotation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_ReservationQuotationCancellationReason_CancellationReasonId",
                table: "Reservation");

            migrationBuilder.DropTable(
                name: "ReservationQuotationCancellationReason");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_CancellationReasonId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Quotation_CancellationReasonId",
                table: "Quotation");

            migrationBuilder.DropColumn(
                name: "CancellationReasonDescription",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "CancellationReasonId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "CancellationReasonId",
                table: "Quotation");

            migrationBuilder.RenameColumn(
                name: "CancellationReasonDescription",
                table: "Quotation",
                newName: "RejectionReason");
        }
    }
}
