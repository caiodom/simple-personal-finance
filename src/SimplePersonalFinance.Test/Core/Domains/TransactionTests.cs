using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Exceptions;

namespace SimplePersonalFinance.Test.Core.Domain.Entities;

public class TransactionTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        var accountId = Guid.NewGuid();
        var description = "Test Transaction";
        var amount = 100m;
        var date = DateTime.Now;

        var transaction = new Transaction(
            accountId,
            CategoryEnum.ENTERTAINMENT,
            TransactionTypeEnum.EXPENSE,
            description,
            amount,
            date);

        Assert.Equal(accountId, transaction.AccountId);
        Assert.Equal((int)CategoryEnum.ENTERTAINMENT, transaction.CategoryId);
        Assert.Equal((int)TransactionTypeEnum.EXPENSE, transaction.TransactionTypeId);
        Assert.Equal(description, transaction.Description);
        Assert.Equal(amount, transaction.Amount);
        Assert.Equal(date, transaction.Date);
    }

    [Fact]
    public void Constructor_WithEmptyAccountId_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            CreateTransaction(accountId: Guid.Empty));

        Assert.Equal("Transaction account id cannot be empty", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyDescription_ShouldThrowDomainException(string invalidDescription)
    {
        var exception = Assert.Throws<DomainException>(() =>
            CreateTransaction(description: invalidDescription));

        Assert.Equal("Transaction description cannot be empty", exception.Message);
    }

    [Fact]
    public void Constructor_WithNegativeAmount_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            CreateTransaction(amount: -1m));

        Assert.Equal("Transaction amount cannot be negative", exception.Message);
    }

    [Fact]
    public void Constructor_WithInvalidCategory_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            CreateTransaction(category: (CategoryEnum)999));

        Assert.Equal("Transaction category is invalid", exception.Message);
    }

    [Fact]
    public void Constructor_WithInvalidTransactionType_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            CreateTransaction(transactionType: (TransactionTypeEnum)999));

        Assert.Equal("Transaction type is invalid", exception.Message);
    }

    [Fact]
    public void Constructor_WithDefaultDate_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            CreateTransaction(date: DateTime.MinValue));

        Assert.Equal("Transaction date must be provided", exception.Message);
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateAllProperties()
    {
        var transaction = CreateTransaction(
            category: CategoryEnum.FOOD,
            transactionType: TransactionTypeEnum.EXPENSE,
            description: "Groceries",
            amount: 50m);

        transaction.UpdateDetails(
            75m,
            "Updated Groceries",
            CategoryEnum.OTHERS,
            TransactionTypeEnum.INCOME);

        Assert.Equal(75m, transaction.Amount);
        Assert.Equal("Updated Groceries", transaction.Description);
        Assert.Equal((int)CategoryEnum.OTHERS, transaction.CategoryId);
        Assert.Equal((int)TransactionTypeEnum.INCOME, transaction.TransactionTypeId);
    }

    [Fact]
    public void UpdateDetails_WithInvalidData_ShouldNotMutateTransaction()
    {
        var transaction = CreateTransaction(
            category: CategoryEnum.FOOD,
            transactionType: TransactionTypeEnum.EXPENSE,
            description: "Groceries",
            amount: 50m);

        Assert.Throws<DomainException>(() =>
            transaction.UpdateDetails(-1m, "Changed", CategoryEnum.OTHERS, TransactionTypeEnum.INCOME));

        Assert.Equal(50m, transaction.Amount);
        Assert.Equal("Groceries", transaction.Description);
        Assert.Equal((int)CategoryEnum.FOOD, transaction.CategoryId);
        Assert.Equal((int)TransactionTypeEnum.EXPENSE, transaction.TransactionTypeId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_WithEmptyDescription_ShouldThrowDomainException(string invalidDescription)
    {
        var transaction = CreateTransaction();

        var exception = Assert.Throws<DomainException>(() =>
            transaction.UpdateDetails(100m, invalidDescription, CategoryEnum.OTHERS, TransactionTypeEnum.INCOME));

        Assert.Equal("Transaction description cannot be empty", exception.Message);
    }

    [Fact]
    public void UpdateDetails_WithInvalidCategory_ShouldThrowDomainException()
    {
        var transaction = CreateTransaction();

        var exception = Assert.Throws<DomainException>(() =>
            transaction.UpdateDetails(100m, "Updated", (CategoryEnum)999, TransactionTypeEnum.INCOME));

        Assert.Equal("Transaction category is invalid", exception.Message);
    }

    [Fact]
    public void UpdateDetails_WithInvalidTransactionType_ShouldThrowDomainException()
    {
        var transaction = CreateTransaction();

        var exception = Assert.Throws<DomainException>(() =>
            transaction.UpdateDetails(100m, "Updated", CategoryEnum.OTHERS, (TransactionTypeEnum)999));

        Assert.Equal("Transaction type is invalid", exception.Message);
    }

    private static Transaction CreateTransaction(
        Guid? accountId = null,
        CategoryEnum category = CategoryEnum.ENTERTAINMENT,
        TransactionTypeEnum transactionType = TransactionTypeEnum.EXPENSE,
        string description = "Test Transaction",
        decimal amount = 100m,
        DateTime? date = null)
    {
        return new Transaction(
            accountId ?? Guid.NewGuid(),
            category,
            transactionType,
            description,
            amount,
            date ?? DateTime.Now);
    }
}
