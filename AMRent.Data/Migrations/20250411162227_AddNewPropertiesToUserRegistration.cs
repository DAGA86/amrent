using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewPropertiesToUserRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "User",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdentityCountryId",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityNumber",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LicenseCountryId",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseDate",
                table: "User",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseExpireDate",
                table: "User",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalLocation",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telephone",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TelephonePrefixCountryId",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VatNumber",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("3f1d2e57-4a77-44d9-9c0d-5b2e3c2a1e99"),
                columns: new[] { "Address", "BirthDate", "CountryId", "IdentityCountryId", "IdentityNumber", "LicenseCountryId", "LicenseDate", "LicenseExpireDate", "LicenseNumber", "Name", "PostalCode", "PostalLocation", "Telephone", "TelephonePrefixCountryId", "VatNumber" },
                values: new object[] { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("42a7b5d3-98e0-4e19-bd89-1c5f3e0d2a67"),
                columns: new[] { "Address", "BirthDate", "CountryId", "IdentityCountryId", "IdentityNumber", "LicenseCountryId", "LicenseDate", "LicenseExpireDate", "LicenseNumber", "Name", "PostalCode", "PostalLocation", "Telephone", "TelephonePrefixCountryId", "VatNumber" },
                values: new object[] { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("5a9e2c3d-7b0f-4e81-bd46-3f2d1a5e98c7"),
                columns: new[] { "Address", "BirthDate", "CountryId", "IdentityCountryId", "IdentityNumber", "LicenseCountryId", "LicenseDate", "LicenseExpireDate", "LicenseNumber", "Name", "PostalCode", "PostalLocation", "Telephone", "TelephonePrefixCountryId", "VatNumber" },
                values: new object[] { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("9bfa3d47-2c5e-4f38-9872-7b6d4e1c5a99"),
                columns: new[] { "Address", "BirthDate", "CountryId", "IdentityCountryId", "IdentityNumber", "LicenseCountryId", "LicenseDate", "LicenseExpireDate", "LicenseNumber", "Name", "PostalCode", "PostalLocation", "Telephone", "TelephonePrefixCountryId", "VatNumber" },
                values: new object[] { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("d51c7e89-3a2b-417c-8f0d-5e1b9c6d4f22"),
                columns: new[] { "Address", "BirthDate", "CountryId", "IdentityCountryId", "IdentityNumber", "LicenseCountryId", "LicenseDate", "LicenseExpireDate", "LicenseNumber", "Name", "PostalCode", "PostalLocation", "Telephone", "TelephonePrefixCountryId", "VatNumber" },
                values: new object[] { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("e6f8a9b1-0d3c-465e-90b8-2f9e2d4a8c11"),
                columns: new[] { "Address", "BirthDate", "CountryId", "IdentityCountryId", "IdentityNumber", "LicenseCountryId", "LicenseDate", "LicenseExpireDate", "LicenseNumber", "Name", "PostalCode", "PostalLocation", "Telephone", "TelephonePrefixCountryId", "VatNumber" },
                values: new object[] { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("f8b6d3a1-9c42-4e70-83d2-5e9a1b7c4f55"),
                columns: new[] { "Address", "BirthDate", "CountryId", "IdentityCountryId", "IdentityNumber", "LicenseCountryId", "LicenseDate", "LicenseExpireDate", "LicenseNumber", "Name", "PostalCode", "PostalLocation", "Telephone", "TelephonePrefixCountryId", "VatNumber" },
                values: new object[] { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "User");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IdentityCountryId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IdentityNumber",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LicenseCountryId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LicenseDate",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LicenseExpireDate",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PostalLocation",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Telephone",
                table: "User");

            migrationBuilder.DropColumn(
                name: "TelephonePrefixCountryId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VatNumber",
                table: "User");
        }
    }
}
