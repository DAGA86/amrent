using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtraPriceByInsuranceLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExtraPriceByInsuranceLevel",
                columns: table => new
                {
                    ExtraId = table.Column<int>(type: "int", nullable: false),
                    InsuranceLevelId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(7,2)", nullable: false),
                    MaximumValue = table.Column<decimal>(type: "decimal(7,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraPriceByInsuranceLevel", x => new { x.InsuranceLevelId, x.ExtraId });
                    table.ForeignKey(
                        name: "FK_ExtraPriceByInsuranceLevel_Extra_ExtraId",
                        column: x => x.ExtraId,
                        principalTable: "Extra",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExtraPriceByInsuranceLevel_InsuranceLevel_InsuranceLevelId",
                        column: x => x.InsuranceLevelId,
                        principalTable: "InsuranceLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExtraPriceByInsuranceLevel_ExtraId",
                table: "ExtraPriceByInsuranceLevel",
                column: "ExtraId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExtraPriceByInsuranceLevel");
        }
    }
}
