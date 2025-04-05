using Microsoft.EntityFrameworkCore;
using SimplePersonalFinance.Core.Entities;

namespace SimplePersonalFinance.Infrastructure.Data.Extensions;
public static class ModelBuilderExtensions
{
    public static void SeedDefaults(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountType>().HasData(
            new AccountType(id: 1, name: "Checking"),
            new AccountType(id: 2, name: "Savings"),
            new AccountType(id: 3, name: "Wallet")
        );

        modelBuilder.Entity<TransactionType>().HasData(

            new TransactionType(id: 1, name: "Expense", isCredit: false),
            new TransactionType(id: 2, name: "Income", isCredit: true)
        );

        modelBuilder.Entity<Category>().HasData(
            new Category(id: 1, name: "Entertainment"),
            new Category(id: 2, name: "Food"),
            new Category(id: 3, name: "Transport"),
            new Category(id: 4, name: "Health"),
            new Category(id: 5, name: "Salary"),
            new Category(id: 6, name: "Others")
            );
    }
}
