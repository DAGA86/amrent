using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class TranslatableSettingsSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TranslatableSetting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslatableSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TranslatableSettingTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    TranslatableSettingId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslatableSettingTranslation", x => new { x.LanguageId, x.TranslatableSettingId });
                    table.ForeignKey(
                        name: "FK_TranslatableSettingTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TranslatableSettingTranslation_TranslatableSetting_TranslatableSettingId",
                        column: x => x.TranslatableSettingId,
                        principalTable: "TranslatableSetting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TranslatableSetting",
                columns: new[] { "Id", "Code" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "TranslatableSetting",
                columns: new[] { "Id", "Code" },
                values: new object[] { 2, 2 });

            migrationBuilder.InsertData(
                table: "TranslatableSetting",
                columns: new[] { "Id", "Code" },
                values: new object[] { 3, 3 });

            migrationBuilder.CreateIndex(
                name: "IX_TranslatableSettingTranslation_TranslatableSettingId",
                table: "TranslatableSettingTranslation",
                column: "TranslatableSettingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TranslatableSettingTranslation");

            migrationBuilder.DropTable(
                name: "TranslatableSetting");
        }
    }
}
