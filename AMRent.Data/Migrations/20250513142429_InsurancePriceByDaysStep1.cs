using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class InsurancePriceByDaysStep1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("TRUNCATE TABLE InsuranceChange;");
            migrationBuilder.DropForeignKey(
                name: "FK_InsuranceChange_Insurance_CarSegmentId_InsuranceLevelId",
                table: "InsuranceChange");

            migrationBuilder.DropIndex(
                name: "IX_InsuranceChange_CarSegmentId_InsuranceLevelId",
                table: "InsuranceChange");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Insurance",
                table: "Insurance");

            migrationBuilder.DropIndex(
                name: "IX_Insurance_InsuranceLevelId",
                table: "Insurance");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "Insurance");

            migrationBuilder.AddColumn<int>(
                name: "InsuranceId",
                table: "InsuranceChange",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Insurance",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Insurance",
                table: "Insurance",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "InsurancePrice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Days = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(7,2)", nullable: false),
                    InsuranceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsurancePrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsurancePrice_Insurance_InsuranceId",
                        column: x => x.InsuranceId,
                        principalTable: "Insurance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceChange_InsuranceId",
                table: "InsuranceChange",
                column: "InsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_Insurance_CarSegmentId",
                table: "Insurance",
                column: "CarSegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Insurance_InsuranceLevelId_CarSegmentId",
                table: "Insurance",
                columns: new[] { "InsuranceLevelId", "CarSegmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePrice_InsuranceId",
                table: "InsurancePrice",
                column: "InsuranceId");

            migrationBuilder.AddForeignKey(
                name: "FK_InsuranceChange_Insurance_InsuranceId",
                table: "InsuranceChange",
                column: "InsuranceId",
                principalTable: "Insurance",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InsuranceChange_Insurance_InsuranceId",
                table: "InsuranceChange");

            migrationBuilder.DropTable(
                name: "InsurancePrice");

            migrationBuilder.DropIndex(
                name: "IX_InsuranceChange_InsuranceId",
                table: "InsuranceChange");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Insurance",
                table: "Insurance");

            migrationBuilder.DropIndex(
                name: "IX_Insurance_CarSegmentId",
                table: "Insurance");

            migrationBuilder.DropIndex(
                name: "IX_Insurance_InsuranceLevelId_CarSegmentId",
                table: "Insurance");

            migrationBuilder.DropColumn(
                name: "InsuranceId",
                table: "InsuranceChange");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Insurance");

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "Insurance",
                type: "decimal(7,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Insurance",
                table: "Insurance",
                columns: new[] { "CarSegmentId", "InsuranceLevelId" });

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceChange_CarSegmentId_InsuranceLevelId",
                table: "InsuranceChange",
                columns: new[] { "CarSegmentId", "InsuranceLevelId" });

            migrationBuilder.CreateIndex(
                name: "IX_Insurance_InsuranceLevelId",
                table: "Insurance",
                column: "InsuranceLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_InsuranceChange_Insurance_CarSegmentId_InsuranceLevelId",
                table: "InsuranceChange",
                columns: new[] { "CarSegmentId", "InsuranceLevelId" },
                principalTable: "Insurance",
                principalColumns: new[] { "CarSegmentId", "InsuranceLevelId" });
        }
    }
}
