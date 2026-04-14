using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class ProcessSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Process",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FontAwesomeIconCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Process", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessTranslation",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    ProcessId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Text = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessTranslation", x => new { x.LanguageId, x.ProcessId });
                    table.ForeignKey(
                        name: "FK_ProcessTranslation_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessTranslation_Process_ProcessId",
                        column: x => x.ProcessId,
                        principalTable: "Process",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Process",
                columns: new[] { "Id", "FontAwesomeIconCode" },
                values: new object[,]
                {
                    { 1, "" },
                    { 2, "" },
                    { 3, "" },
                    { 4, "" }
                });

            migrationBuilder.InsertData(
                table: "ProcessTranslation",
                columns: new[] { "LanguageId", "ProcessId", "Text", "Title" },
                values: new object[,]
                {
                    { 1, 1, "", "Processo 1" },
                    { 1, 2, "", "Processo 2" },
                    { 1, 3, "", "Processo 3" },
                    { 1, 4, "", "Processo 4" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Language_Name",
                table: "Language",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessTranslation_ProcessId",
                table: "ProcessTranslation",
                column: "ProcessId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessTranslation");

            migrationBuilder.DropTable(
                name: "Process");

            migrationBuilder.DropIndex(
                name: "IX_Language_Name",
                table: "Language");
        }
    }
}
