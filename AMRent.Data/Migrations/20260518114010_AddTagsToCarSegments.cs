using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTagsToCarSegments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "CarSegmentTranslation",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "CarSegmentTranslation");
        }
    }
}
