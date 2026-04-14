using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202402086 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Developer", "Developer" });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 2, "Administração", "Administrador" },
                    { 3, "Marketing", "Marketing" },
                    { 4, "Renting", "Renting" }
                });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "XrfLK0rKzCYfTR3yNwyLRQ==");

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "EmailAddress", "FirstName", "IsLocked", "LastName", "Password", "Username" },
                values: new object[,]
                {
                    { 2, "alexandra.santos@amconfraria.com", null, false, null, "LVxdSub5BWmV1iSsEu4VhQ==", "alexandra.santos" },
                    { 3, "sara.rodrigues@amconfraria.com", null, false, null, "LVxdSub5BWmV1iSsEu4VhQ==", "sara.rodrigues" },
                    { 4, "elsa.francisco@sarafauto.pt", null, false, null, "LVxdSub5BWmV1iSsEu4VhQ==", "elsa.francisco" }
                });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "Claim", "RoleId" },
                values: new object[,]
                {
                    { "Delete", 2 },
                    { "Edit", 2 },
                    { "View", 2 },
                    { "Delete", 3 },
                    { "Edit", 3 },
                    { "View", 3 },
                    { "Edit", 4 },
                    { "View", 4 }
                });

            migrationBuilder.InsertData(
                table: "RoleUser",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 4 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Claim", "RoleId" },
                keyValues: new object[] { "Delete", 2 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Claim", "RoleId" },
                keyValues: new object[] { "Edit", 2 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Claim", "RoleId" },
                keyValues: new object[] { "View", 2 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Claim", "RoleId" },
                keyValues: new object[] { "Delete", 3 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Claim", "RoleId" },
                keyValues: new object[] { "Edit", 3 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Claim", "RoleId" },
                keyValues: new object[] { "View", 3 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Claim", "RoleId" },
                keyValues: new object[] { "Edit", 4 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Claim", "RoleId" },
                keyValues: new object[] { "View", 4 });

            migrationBuilder.DeleteData(
                table: "RoleUser",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "RoleUser",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "RoleUser",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Administração", "Administrador" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "");
        }
    }
}
