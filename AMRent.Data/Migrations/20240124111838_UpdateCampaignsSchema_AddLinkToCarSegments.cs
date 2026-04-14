using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class UpdateCampaignsSchema_AddLinkToCarSegments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CampaignCarSegment",
                columns: table => new
                {
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    CarSegmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignCarSegment", x => new { x.CarSegmentId, x.CampaignId });
                    table.ForeignKey(
                        name: "FK_CampaignCarSegment_Campaign_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignCarSegment_CarSegment_CarSegmentId",
                        column: x => x.CarSegmentId,
                        principalTable: "CarSegment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignCarSegment_CampaignId",
                table: "CampaignCarSegment",
                column: "CampaignId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaignCarSegment");
        }
    }
}
