using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLoadingSpaceMeasuresToCommercialSegments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoadingSpaceHeightInMilimeters",
                table: "CarSegment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoadingSpaceLengthInMilimeters",
                table: "CarSegment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoadingSpaceWidthInMilimeters",
                table: "CarSegment",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "GenericText",
                columns: new[] { "Id", "Key" },
                values: new object[,]
                {
                    { 217, "Segmento.Detalhe.Caracteristicas.ComprimentoCarga" },
                    { 218, "Segmento.Detalhe.Caracteristicas.LarguraCarga" },
                    { 219, "Segmento.Detalhe.Caracteristicas.AlturaCarga" }
                });

            migrationBuilder.InsertData(
                table: "GenericTextTranslation",
                columns: new[] { "GenericTextId", "LanguageId", "Value" },
                values: new object[,]
                {
                    { 217, 1, "ComprimentoCarga" },
                    { 218, 1, "LarguraCarga" },
                    { 219, 1, "AlturaCarga" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 217, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 218, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 219, 1 });

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 217);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 218);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 219);

            migrationBuilder.DropColumn(
                name: "LoadingSpaceHeightInMilimeters",
                table: "CarSegment");

            migrationBuilder.DropColumn(
                name: "LoadingSpaceLengthInMilimeters",
                table: "CarSegment");

            migrationBuilder.DropColumn(
                name: "LoadingSpaceWidthInMilimeters",
                table: "CarSegment");
        }
    }
}
