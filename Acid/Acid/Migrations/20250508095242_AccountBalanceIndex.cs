using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acid.Migrations
{
    public partial class AccountBalanceIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Account_Balance",
                table: "MyAccounts",
                column: "Balance");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Account_Balance",
                table: "MyAccounts");
        }
    }
}
