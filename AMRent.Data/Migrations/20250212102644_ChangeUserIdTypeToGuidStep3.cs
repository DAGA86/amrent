using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMRent.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserIdTypeToGuidStep3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"UPDATE [Quotation] SET [UserId] = (SELECT [Id] FROM [User] u WHERE u.[UserId] = [User2Id])");
            migrationBuilder.Sql($"UPDATE [QuotationChange] SET [UserId] = (SELECT [Id] FROM [User] u WHERE u.[UserId] = [User2Id])");
            migrationBuilder.Sql($"UPDATE [Reservation] SET [AssignedUserId] = (SELECT [Id] FROM [User] u WHERE u.[UserId] = [AssignedUser2Id])");
            migrationBuilder.Sql($"UPDATE [Reservation] SET [CustomerId] = (SELECT [Id] FROM [User] u WHERE u.[UserId] = [Customer2Id])");
            migrationBuilder.Sql($"UPDATE [ReservationChange] SET [UserId] = (SELECT [Id] FROM [User] u WHERE u.[UserId] = [User2Id])");
            migrationBuilder.Sql($"UPDATE [RoleUser] SET [UserId] = (SELECT [Id] FROM [User] u WHERE u.[UserId] = [User2Id])");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
