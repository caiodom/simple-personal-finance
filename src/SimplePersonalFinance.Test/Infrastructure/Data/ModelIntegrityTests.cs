using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.ValueObjects;
using SimplePersonalFinance.Core.Interfaces.Services;
using SimplePersonalFinance.Infrastructure.Data.Context;

namespace SimplePersonalFinance.Test.Infrastructure.Data;

public class ModelIntegrityTests
{
    [Fact]
    public void Model_ShouldDefineExpectedIntegrityIndexes()
    {
        using var context = CreateContext();

        var emailEntity = context.Model.FindEntityType(typeof(Email));
        Assert.NotNull(emailEntity);
        var emailIndex = Assert.Single(
            emailEntity.GetIndexes(),
            index => index.GetDatabaseName() == "UX_Users_Email");
        Assert.True(emailIndex.IsUnique);
        Assert.Equal(new[] { "Value" }, emailIndex.Properties.Select(property => property.Name));

        var budgetEntity = context.Model.FindEntityType(typeof(Budget));
        Assert.NotNull(budgetEntity);
        var budgetIndex = Assert.Single(
            budgetEntity.GetIndexes(),
            index => index.GetDatabaseName() == "UX_Budgets_UserId_CategoryId_Active");
        Assert.True(budgetIndex.IsUnique);
        Assert.Equal("\"IsActive\" = TRUE", budgetIndex.GetFilter());
        Assert.Equal(
            new[] { nameof(Budget.UserId), nameof(Budget.CategoryId) },
            budgetIndex.Properties.Select(property => property.Name));

        var transactionEntity = context.Model.FindEntityType(typeof(Transaction));
        Assert.NotNull(transactionEntity);
        var transactionIndex = Assert.Single(
            transactionEntity.GetIndexes(),
            index => index.GetDatabaseName() == "IX_Transactions_AccountId_IsActive_Date");
        Assert.Equal(
            new[] { nameof(Transaction.AccountId), nameof(Transaction.IsActive), nameof(Transaction.Date) },
            transactionIndex.Properties.Select(property => property.Name));
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=model_tests;Username=test;Password=test")
            .Options;

        return new AppDbContext(options, Mock.Of<IDomainEventDispatcher>());
    }
}
