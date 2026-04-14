using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202401311 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PickupValue",
                table: "Reservation",
                type: "decimal(7,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReturnValue",
                table: "Reservation",
                type: "decimal(7,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxValue",
                table: "PickupReturnLocation",
                type: "decimal(7,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(7,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TaxType",
                table: "PickupReturnLocation",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 90,
                column: "Key",
                value: "Segmento.Detalhe.Sidebar.ValorVeiculo");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 91,
                column: "Key",
                value: "Segmento.Detalhe.Sidebar.ValorExtras");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 92,
                column: "Key",
                value: "Segmento.Detalhe.Sidebar.ValorSeguro");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 93,
                column: "Key",
                value: "Segmento.Detalhe.Sidebar.ValorTotal");

            migrationBuilder.InsertData(
                table: "GenericText",
                columns: new[] { "Id", "Key" },
                values: new object[,]
                {
                    { 132, "Segmento.Detalhe.Sidebar.ValorRecolha" },
                    { 133, "Segmento.Detalhe.Sidebar.ValorDevolucao" }
                });

            migrationBuilder.InsertData(
                table: "GenericTextTranslation",
                columns: new[] { "GenericTextId", "LanguageId", "Value" },
                values: new object[] { 132, 1, "Recolha" });

            migrationBuilder.InsertData(
                table: "GenericTextTranslation",
                columns: new[] { "GenericTextId", "LanguageId", "Value" },
                values: new object[] { 133, 1, "Devolução" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 132, 1 });

            migrationBuilder.DeleteData(
                table: "GenericTextTranslation",
                keyColumns: new[] { "GenericTextId", "LanguageId" },
                keyValues: new object[] { 133, 1 });

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 132);

            migrationBuilder.DeleteData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 133);

            migrationBuilder.DropColumn(
                name: "PickupValue",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "ReturnValue",
                table: "Reservation");

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxValue",
                table: "PickupReturnLocation",
                type: "decimal(7,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(7,2)");

            migrationBuilder.AlterColumn<int>(
                name: "TaxType",
                table: "PickupReturnLocation",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 90,
                column: "Key",
                value: "Segmento.Detalhe.Sidebar.Veiculo");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 91,
                column: "Key",
                value: "Segmento.Detalhe.Sidebar.Extras");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 92,
                column: "Key",
                value: "Segmento.Detalhe.Sidebar.Seguro");

            migrationBuilder.UpdateData(
                table: "GenericText",
                keyColumn: "Id",
                keyValue: 93,
                column: "Key",
                value: "Segmento.Detalhe.Sidebar.Total");
        }
    }
}
