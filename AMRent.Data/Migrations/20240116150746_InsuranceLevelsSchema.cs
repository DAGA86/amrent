using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class InsuranceLevelsSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Level",
                table: "Insurance",
                newName: "InsuranceLevelId");

            migrationBuilder.AddColumn<string>(
                name: "Excluded",
                table: "Insurance",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Included",
                table: "Insurance",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InsuranceLevel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceLevel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceLevelTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    InsuranceLevelId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Included = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Excluded = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceLevelTranslation", x => new { x.LanguageId, x.InsuranceLevelId });
                    table.ForeignKey(
                        name: "FK_InsuranceLevelTranslation_InsuranceLevel_InsuranceLevelId",
                        column: x => x.InsuranceLevelId,
                        principalTable: "InsuranceLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InsuranceLevelTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "InsuranceLevel",
                column: "Id",
                value: 1);

            migrationBuilder.InsertData(
                table: "InsuranceLevel",
                column: "Id",
                value: 2);

            migrationBuilder.InsertData(
                table: "InsuranceLevel",
                column: "Id",
                value: 3);

            migrationBuilder.InsertData(
                table: "InsuranceLevelTranslation",
                columns: new[] { "InsuranceLevelId", "LanguageId", "Excluded", "Included", "Name" },
                values: new object[] { 1, 1, "", "", "Nível 1" });

            migrationBuilder.InsertData(
                table: "InsuranceLevelTranslation",
                columns: new[] { "InsuranceLevelId", "LanguageId", "Excluded", "Included", "Name" },
                values: new object[] { 2, 1, "", "", "Nível 2" });

            migrationBuilder.InsertData(
                table: "InsuranceLevelTranslation",
                columns: new[] { "InsuranceLevelId", "LanguageId", "Excluded", "Included", "Name" },
                values: new object[] { 3, 1, "", "", "Nível 3" });

            migrationBuilder.CreateIndex(
                name: "IX_Insurance_InsuranceLevelId",
                table: "Insurance",
                column: "InsuranceLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceLevelTranslation_InsuranceLevelId",
                table: "InsuranceLevelTranslation",
                column: "InsuranceLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Insurance_InsuranceLevel_InsuranceLevelId",
                table: "Insurance",
                column: "InsuranceLevelId",
                principalTable: "InsuranceLevel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Insurance_InsuranceLevel_InsuranceLevelId",
                table: "Insurance");

            migrationBuilder.DropTable(
                name: "InsuranceLevelTranslation");

            migrationBuilder.DropTable(
                name: "InsuranceLevel");

            migrationBuilder.DropIndex(
                name: "IX_Insurance_InsuranceLevelId",
                table: "Insurance");

            migrationBuilder.DropColumn(
                name: "Excluded",
                table: "Insurance");

            migrationBuilder.DropColumn(
                name: "Included",
                table: "Insurance");

            migrationBuilder.RenameColumn(
                name: "InsuranceLevelId",
                table: "Insurance",
                newName: "Level");
        }
    }
}
