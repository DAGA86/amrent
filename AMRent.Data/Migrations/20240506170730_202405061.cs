using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    public partial class _202405061 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermission",
                table: "RolePermission");

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Claim", "RoleId" },
                keyColumnTypes: new[] { "nvarchar(450)", "int" },
                keyValues: new object[] { "Administration", 2 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Claim", "RoleId" },
                keyColumnTypes: new[] { "nvarchar(450)", "int" },
                keyValues: new object[] { "Delete", 3 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Claim", "RoleId" },
                keyColumnTypes: new[] { "nvarchar(450)", "int" },
                keyValues: new object[] { "Edit", 3 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Claim", "RoleId" },
                keyColumnTypes: new[] { "nvarchar(450)", "int" },
                keyValues: new object[] { "View", 3 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Claim", "RoleId" },
                keyColumnTypes: new[] { "nvarchar(450)", "int" },
                keyValues: new object[] { "Delete", 4 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Claim", "RoleId" },
                keyColumnTypes: new[] { "nvarchar(450)", "int" },
                keyValues: new object[] { "Edit", 4 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Claim", "RoleId" },
                keyColumnTypes: new[] { "nvarchar(450)", "int" },
                keyValues: new object[] { "View", 4 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Claim", "RoleId" },
                keyColumnTypes: new[] { "nvarchar(450)", "int" },
                keyValues: new object[] { "Edit", 5 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Claim", "RoleId" },
                keyColumnTypes: new[] { "nvarchar(450)", "int" },
                keyValues: new object[] { "View", 5 });

            migrationBuilder.DropColumn(
                name: "Claim",
                table: "RolePermission");

            migrationBuilder.AddColumn<int>(
                name: "Permission",
                table: "RolePermission",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermission",
                table: "RolePermission",
                columns: new[] { "RoleId", "Permission" });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "Permission", "RoleId" },
                values: new object[] { 1, 2 });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "Permission", "RoleId" },
                values: new object[] { 2, 2 });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "Permission", "RoleId" },
                values: new object[] { 3, 2 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermission",
                table: "RolePermission");

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Permission", "RoleId" },
                keyColumnTypes: new[] { "int", "int" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Permission", "RoleId" },
                keyColumnTypes: new[] { "int", "int" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "Permission", "RoleId" },
                keyColumnTypes: new[] { "int", "int" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DropColumn(
                name: "Permission",
                table: "RolePermission");

            migrationBuilder.AddColumn<string>(
                name: "Claim",
                table: "RolePermission",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermission",
                table: "RolePermission",
                columns: new[] { "RoleId", "Claim" });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "Claim", "RoleId" },
                values: new object[,]
                {
                    { "Administration", 2 },
                    { "Delete", 3 },
                    { "Edit", 3 },
                    { "View", 3 },
                    { "Delete", 4 },
                    { "Edit", 4 },
                    { "View", 4 },
                    { "Edit", 5 },
                    { "View", 5 }
                });
        }
    }
}
