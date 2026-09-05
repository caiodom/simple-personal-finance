using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SimplePersonalFinance.Infrastructure.Data.Context;

#nullable disable

namespace SimplePersonalFinance.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260905222500_AddDataIntegrityIndexes")]
public partial class AddDataIntegrityIndexes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Transactions_AccountId",
            table: "Transactions");

        migrationBuilder.CreateIndex(
            name: "UX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "UX_Budgets_UserId_CategoryId_Active",
            table: "Budgets",
            columns: new[] { "UserId", "CategoryId" },
            unique: true,
            filter: "\"IsActive\" = TRUE");

        migrationBuilder.CreateIndex(
            name: "IX_Transactions_AccountId_IsActive_Date",
            table: "Transactions",
            columns: new[] { "AccountId", "IsActive", "Date" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "UX_Users_Email",
            table: "Users");

        migrationBuilder.DropIndex(
            name: "UX_Budgets_UserId_CategoryId_Active",
            table: "Budgets");

        migrationBuilder.DropIndex(
            name: "IX_Transactions_AccountId_IsActive_Date",
            table: "Transactions");

        migrationBuilder.CreateIndex(
            name: "IX_Transactions_AccountId",
            table: "Transactions",
            column: "AccountId");
    }
}
