using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStartAndEndDateTimesToHomeBanners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFromUtc",
                table: "HomeBanner",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidUntilUtc",
                table: "HomeBanner",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidFromUtc",
                table: "HomeBanner");

            migrationBuilder.DropColumn(
                name: "ValidUntilUtc",
                table: "HomeBanner");
        }
    }
}
