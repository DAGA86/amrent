using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202405062 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Permission", "RoleId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Permission", "RoleId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 6, "Reservas", "Reservas" });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "Permission", "RoleId" },
                values: new object[,]
                {
                    { 0, 3 },
                    { 1, 3 },
                    { 0, 4 },
                    { 1, 4 },
                    { 0, 5 },
                    { 1, 5 }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "EmailAddress", "FirstName", "IsLocked", "LastName", "Password", "Username" },
                values: new object[,]
                {
                    { 5, "joana.leitao@sarafauto.pt", "Joana", false, "Leitão", "LVxdSub5BWmV1iSsEu4VhQ==", "joana.leitao" },
                    { 6, "dina.gomes@sarafauto.pt", "Dina", false, "Gomes", "LVxdSub5BWmV1iSsEu4VhQ==", "dina.gomes" },
                    { 7, "sabina.gameiro@amconfraria.com", "Sabina", false, "Gameiro", "LVxdSub5BWmV1iSsEu4VhQ==", "sabina.gameiro" }
                });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "Permission", "RoleId" },
                values: new object[] { 1, 6 });

            migrationBuilder.InsertData(
                table: "RoleUser",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { 6, 5 },
                    { 6, 6 },
                    { 6, 7 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Permission", "RoleId" },
                keyValues: new object[] { 0, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Permission", "RoleId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Permission", "RoleId" },
                keyValues: new object[] { 0, 4 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Permission", "RoleId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Permission", "RoleId" },
                keyValues: new object[] { 0, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Permission", "RoleId" },
                keyValues: new object[] { 1, 5 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Permission", "RoleId" },
                keyValues: new object[] { 1, 6 });

            migrationBuilder.DeleteData(
                table: "RoleUser",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 6, 5 });

            migrationBuilder.DeleteData(
                table: "RoleUser",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 6, 6 });

            migrationBuilder.DeleteData(
                table: "RoleUser",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 6, 7 });

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "Permission", "RoleId" },
                values: new object[] { 2, 2 });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "Permission", "RoleId" },
                values: new object[] { 3, 2 });
        }
    }
}
