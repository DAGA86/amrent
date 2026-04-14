using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtraPriceByInsuranceLevelStep3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"INSERT INTO [ExtraPriceByInsuranceLevel]([ExtraId],[InsuranceLevelId],[Value],[MaximumValue])SELECT [Id],1,[Value],[MaximumValue] FROM [Extra]");
            migrationBuilder.Sql(@"INSERT INTO [ExtraPriceByInsuranceLevel]([ExtraId],[InsuranceLevelId],[Value],[MaximumValue])SELECT [Id],2,[Value],[MaximumValue] FROM [Extra]");
            migrationBuilder.Sql(@"INSERT INTO [ExtraPriceByInsuranceLevel]([ExtraId],[InsuranceLevelId],[Value],[MaximumValue])SELECT [Id],3,[Value],[MaximumValue] FROM [Extra]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE [Extra] SET [Value] = (SELECT [Value] FROM [ExtraPriceByInsuranceLevel] WHERE ExtraId = Id AND InsuranceLevelId = 1)");
        }
    }
}
