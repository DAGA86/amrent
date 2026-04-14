using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserIdTypeToGuidStep2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "User",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWID()");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "RoleUser",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ReservationChange",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedUserId",
                table: "Reservation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Reservation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "QuotationChange",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Quotation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "RoleUser",
                keyColumns: new[] { "RoleId", "User2Id" },
                keyValues: new object[] { 2, 1 },
                column: "UserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RoleUser",
                keyColumns: new[] { "RoleId", "User2Id" },
                keyValues: new object[] { 3, 2 },
                column: "UserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RoleUser",
                keyColumns: new[] { "RoleId", "User2Id" },
                keyValues: new object[] { 4, 3 },
                column: "UserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RoleUser",
                keyColumns: new[] { "RoleId", "User2Id" },
                keyValues: new object[] { 5, 4 },
                column: "UserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RoleUser",
                keyColumns: new[] { "RoleId", "User2Id" },
                keyValues: new object[] { 6, 5 },
                column: "UserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RoleUser",
                keyColumns: new[] { "RoleId", "User2Id" },
                keyValues: new object[] { 6, 6 },
                column: "UserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "RoleUser",
                keyColumns: new[] { "RoleId", "User2Id" },
                keyValues: new object[] { 6, 7 },
                column: "UserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Id",
                value: new Guid("3f1d2e57-4a77-44d9-9c0d-5b2e3c2a1e99"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 2,
                column: "Id",
                value: new Guid("e6f8a9b1-0d3c-465e-90b8-2f9e2d4a8c11"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Id",
                value: new Guid("9bfa3d47-2c5e-4f38-9872-7b6d4e1c5a99"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 4,
                column: "Id",
                value: new Guid("42a7b5d3-98e0-4e19-bd89-1c5f3e0d2a67"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 5,
                column: "Id",
                value: new Guid("d51c7e89-3a2b-417c-8f0d-5e1b9c6d4f22"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 6,
                column: "Id",
                value: new Guid("5a9e2c3d-7b0f-4e81-bd46-3f2d1a5e98c7"));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 7,
                column: "Id",
                value: new Guid("f8b6d3a1-9c42-4e70-83d2-5e9a1b7c4f55"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RoleUser");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ReservationChange");

            migrationBuilder.DropColumn(
                name: "AssignedUserId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "QuotationChange");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Quotation");
        }
    }
}
