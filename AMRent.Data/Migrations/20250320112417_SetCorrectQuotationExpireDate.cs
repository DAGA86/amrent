using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class SetCorrectQuotationExpireDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"UPDATE [Quotation] SET [ExpireDateTime] = DATEADD(DAY, 15, [CreateDate])");
            migrationBuilder.Sql($"UPDATE [Quotation] SET [ExpireDateTime] = [PickupDateTime] WHERE [ExpireDateTime] > [PickupDateTime]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"UPDATE [Quotation] SET [ExpireDateTime] = NULL");
        }
    }
}
