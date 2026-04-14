using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSendQuotationReservationSummaryPdfToEmailTypeStep2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE EmailContent SET SendQuotationReservationSummaryPdf = 1 WHERE Id IN (SELECT EmailContentId FROM EmailContentTranslation WHERE [Text] LIKE '%\[SUMMARY\]%')");
            migrationBuilder.Sql(@"UPDATE EmailContentTranslation SET [Text] = REPLACE([Text], CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) + '[SUMMARY]', '')");
            migrationBuilder.Sql(@"UPDATE EmailContentTranslation SET [Text] = REPLACE([Text], '[SUMMARY]' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10), '')");
            migrationBuilder.Sql(@"UPDATE EmailContentTranslation SET [Text] = REPLACE([Text], CHAR(13) + CHAR(10) + '[SUMMARY]' + CHAR(13) + CHAR(10), '')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
