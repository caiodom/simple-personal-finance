using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;

namespace SimplePersonalFinance.Test.Core.Domain.Entities;

public class TransactionTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var description = "Test Transaction";
        var amount = 100m;
        var date = DateTime.Now;

        // Act
        var transaction = new Transaction(
            accountId,
            CategoryEnum.ENTERTAINMENT,
            TransactionTypeEnum.EXPENSE,
            description,
            amount,
            date);

        // Assert
        Assert.Equal(accountId, transaction.AccountId);
        Assert.Equal((int)CategoryEnum.ENTERTAINMENT, transaction.CategoryId);
        Assert.Equal((int)TransactionTypeEnum.EXPENSE, transaction.TransactionTypeId);
        Assert.Equal(description, transaction.Description);
        Assert.Equal(amount, transaction.Amount);
        Assert.Equal(date, transaction.Date);
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateAllProperties()
    {
        // Arrange
        var transaction = new Transaction(
            Guid.NewGuid(),
            CategoryEnum.FOOD,
            TransactionTypeEnum.EXPENSE,
            "Groceries",
            50m,
            DateTime.Now);

        // Act
        transaction.UpdateDetails(
            75m,
            "Updated Groceries",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME);

        // Assert
        Assert.Equal(75m, transaction.Amount);
        Assert.Equal("Updated Groceries", transaction.Description);
        Assert.Equal((int)CategoryEnum.OTHERS, transaction.CategoryId);
        Assert.Equal((int)TransactionTypeEnum.INCOME, transaction.TransactionTypeId);
    }
}
