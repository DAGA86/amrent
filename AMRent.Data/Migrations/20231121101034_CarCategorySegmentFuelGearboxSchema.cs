using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class CarCategorySegmentFuelGearboxSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TranslatableSettingTranslation",
                keyColumns: new[] { "LanguageId", "TranslatableSettingId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "TranslatableSettingTranslation",
                keyColumns: new[] { "LanguageId", "TranslatableSettingId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "TranslatableSettingTranslation",
                keyColumns: new[] { "LanguageId", "TranslatableSettingId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "TranslatableSettingTranslation",
                keyColumns: new[] { "LanguageId", "TranslatableSettingId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "TranslatableSettingTranslation",
                keyColumns: new[] { "LanguageId", "TranslatableSettingId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "TranslatableSettingTranslation",
                keyColumns: new[] { "LanguageId", "TranslatableSettingId" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "TranslatableSettingTranslation",
                keyColumns: new[] { "LanguageId", "TranslatableSettingId" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "TranslatableSettingTranslation",
                keyColumns: new[] { "LanguageId", "TranslatableSettingId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "TranslatableSettingTranslation",
                keyColumns: new[] { "LanguageId", "TranslatableSettingId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.CreateTable(
                name: "CarCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarFuel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarFuel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarGearbox",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarGearbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarCategoryTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    CarCategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarCategoryTranslation", x => new { x.LanguageId, x.CarCategoryId });
                    table.ForeignKey(
                        name: "FK_CarCategoryTranslation_CarCategory_CarCategoryId",
                        column: x => x.CarCategoryId,
                        principalTable: "CarCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarCategoryTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarFuelTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    CarFuelId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarFuelTranslation", x => new { x.LanguageId, x.CarFuelId });
                    table.ForeignKey(
                        name: "FK_CarFuelTranslation_CarFuel_CarFuelId",
                        column: x => x.CarFuelId,
                        principalTable: "CarFuel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarFuelTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarGearboxTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    CarGearboxId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarGearboxTranslation", x => new { x.LanguageId, x.CarGearboxId });
                    table.ForeignKey(
                        name: "FK_CarGearboxTranslation_CarGearbox_CarGearboxId",
                        column: x => x.CarGearboxId,
                        principalTable: "CarGearbox",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarGearboxTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarSegment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Doors = table.Column<int>(type: "int", nullable: false),
                    CarGearboxId = table.Column<int>(type: "int", nullable: false),
                    CarFuelId = table.Column<int>(type: "int", nullable: false),
                    CarCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarSegment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarSegment_CarCategory_CarCategoryId",
                        column: x => x.CarCategoryId,
                        principalTable: "CarCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarSegment_CarFuel_CarFuelId",
                        column: x => x.CarFuelId,
                        principalTable: "CarFuel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarSegment_CarGearbox_CarGearboxId",
                        column: x => x.CarGearboxId,
                        principalTable: "CarGearbox",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarSegmentTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    CarSegmentId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarSegmentTranslation", x => new { x.LanguageId, x.CarSegmentId });
                    table.ForeignKey(
                        name: "FK_CarSegmentTranslation_CarSegment_CarSegmentId",
                        column: x => x.CarSegmentId,
                        principalTable: "CarSegment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarSegmentTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarCategoryTranslation_CarCategoryId",
                table: "CarCategoryTranslation",
                column: "CarCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CarFuelTranslation_CarFuelId",
                table: "CarFuelTranslation",
                column: "CarFuelId");

            migrationBuilder.CreateIndex(
                name: "IX_CarGearboxTranslation_CarGearboxId",
                table: "CarGearboxTranslation",
                column: "CarGearboxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarSegment_CarCategoryId",
                table: "CarSegment",
                column: "CarCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CarSegment_CarFuelId",
                table: "CarSegment",
                column: "CarFuelId");

            migrationBuilder.CreateIndex(
                name: "IX_CarSegment_CarGearboxId",
                table: "CarSegment",
                column: "CarGearboxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarSegmentTranslation_CarSegmentId",
                table: "CarSegmentTranslation",
                column: "CarSegmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarCategoryTranslation");

            migrationBuilder.DropTable(
                name: "CarFuelTranslation");

            migrationBuilder.DropTable(
                name: "CarGearboxTranslation");

            migrationBuilder.DropTable(
                name: "CarSegmentTranslation");

            migrationBuilder.DropTable(
                name: "CarSegment");

            migrationBuilder.DropTable(
                name: "CarCategory");

            migrationBuilder.DropTable(
                name: "CarFuel");

            migrationBuilder.DropTable(
                name: "CarGearbox");

            migrationBuilder.InsertData(
                table: "TranslatableSettingTranslation",
                columns: new[] { "LanguageId", "TranslatableSettingId", "Text" },
                values: new object[,]
                {
                    { 1, 1, "" },
                    { 1, 2, "" },
                    { 1, 3, "" },
                    { 2, 1, "" },
                    { 2, 2, "" },
                    { 2, 3, "" },
                    { 3, 1, "" },
                    { 3, 2, "" },
                    { 3, 3, "" }
                });
        }
    }
}
