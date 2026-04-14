using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class RevertPasswordChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("42a7b5d3-98e0-4e19-bd89-1c5f3e0d2a67"),
                column: "Password",
                value: "LVxdSub5BWmV1iSsEu4VhQ==");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("5a9e2c3d-7b0f-4e81-bd46-3f2d1a5e98c7"),
                column: "Password",
                value: "LVxdSub5BWmV1iSsEu4VhQ==");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("9bfa3d47-2c5e-4f38-9872-7b6d4e1c5a99"),
                column: "Password",
                value: "LVxdSub5BWmV1iSsEu4VhQ==");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("d51c7e89-3a2b-417c-8f0d-5e1b9c6d4f22"),
                column: "Password",
                value: "LVxdSub5BWmV1iSsEu4VhQ==");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("e6f8a9b1-0d3c-465e-90b8-2f9e2d4a8c11"),
                column: "Password",
                value: "LVxdSub5BWmV1iSsEu4VhQ==");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("f8b6d3a1-9c42-4e70-83d2-5e9a1b7c4f55"),
                column: "Password",
                value: "LVxdSub5BWmV1iSsEu4VhQ==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("42a7b5d3-98e0-4e19-bd89-1c5f3e0d2a67"),
                column: "Password",
                value: "RPRZu9vDxd3exdyZ/z+Swg==");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("5a9e2c3d-7b0f-4e81-bd46-3f2d1a5e98c7"),
                column: "Password",
                value: "RPRZu9vDxd3exdyZ/z+Swg==");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("9bfa3d47-2c5e-4f38-9872-7b6d4e1c5a99"),
                column: "Password",
                value: "RPRZu9vDxd3exdyZ/z+Swg==");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("d51c7e89-3a2b-417c-8f0d-5e1b9c6d4f22"),
                column: "Password",
                value: "RPRZu9vDxd3exdyZ/z+Swg==");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("e6f8a9b1-0d3c-465e-90b8-2f9e2d4a8c11"),
                column: "Password",
                value: "RPRZu9vDxd3exdyZ/z+Swg==");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("f8b6d3a1-9c42-4e70-83d2-5e9a1b7c4f55"),
                column: "Password",
                value: "RPRZu9vDxd3exdyZ/z+Swg==");
        }
    }
}
