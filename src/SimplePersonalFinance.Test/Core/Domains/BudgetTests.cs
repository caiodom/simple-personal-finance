using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Exceptions;

namespace SimplePersonalFinance.Test.Core.Domain.Entities;

public class BudgetTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        var userId = Guid.NewGuid();
        var category = CategoryEnum.ENTERTAINMENT;
        var limitAmount = 500m;
        var month = 5;
        var year = 2023;

        var budget = new Budget(userId, category, limitAmount, month, year);

        Assert.Equal(userId, budget.UserId);
        Assert.Equal((int)category, budget.CategoryId);
        Assert.Equal(limitAmount, budget.LimitAmount);
        Assert.Equal(month, budget.Month);
        Assert.Equal(year, budget.Year);
    }

    [Fact]
    public void Constructor_WithEmptyUserId_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Budget(Guid.Empty, CategoryEnum.ENTERTAINMENT, 500m, 5, 2023));

        Assert.Equal("Budget user id cannot be empty", exception.Message);
    }

    [Fact]
    public void Constructor_WithInvalidCategory_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Budget(Guid.NewGuid(), (CategoryEnum)999, 500m, 5, 2023));

        Assert.Equal("Budget category is invalid", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Constructor_WithZeroOrNegativeLimitAmount_ShouldThrowDomainException(decimal invalidAmount)
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Budget(Guid.NewGuid(), CategoryEnum.ENTERTAINMENT, invalidAmount, 5, 2023));

        Assert.Equal("Budget limit amount must be greater than zero", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Constructor_WithInvalidMonth_ShouldThrowDomainException(int invalidMonth)
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Budget(Guid.NewGuid(), CategoryEnum.ENTERTAINMENT, 500m, invalidMonth, 2023));

        Assert.Equal("Budget month must be between 1 and 12", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidYear_ShouldThrowDomainException(int invalidYear)
    {
        var exception = Assert.Throws<DomainException>(() =>
            new Budget(Guid.NewGuid(), CategoryEnum.ENTERTAINMENT, 500m, 5, invalidYear));

        Assert.Equal("Budget year must be greater than zero", exception.Message);
    }

    [Fact]
    public void UpdateBudget_WithValidData_ShouldUpdateProperties()
    {
        var userId = Guid.NewGuid();
        var budget = new Budget(userId, CategoryEnum.ENTERTAINMENT, 500m, 5, 2023);

        budget.UpdateBudget(700m, 6, 2023);

        Assert.Equal(700m, budget.LimitAmount);
        Assert.Equal(6, budget.Month);
        Assert.Equal(2023, budget.Year);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void UpdateBudget_WithZeroOrNegativeLimitAmount_ShouldThrowDomainException(decimal invalidAmount)
    {
        var budget = new Budget(Guid.NewGuid(), CategoryEnum.ENTERTAINMENT, 500m, 5, 2023);

        var exception = Assert.Throws<DomainException>(() =>
            budget.UpdateBudget(invalidAmount, 6, 2023));

        Assert.Equal("Budget limit amount must be greater than zero", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void UpdateBudget_WithInvalidMonth_ShouldThrowDomainException(int invalidMonth)
    {
        var budget = new Budget(Guid.NewGuid(), CategoryEnum.ENTERTAINMENT, 500m, 5, 2023);

        var exception = Assert.Throws<DomainException>(() =>
            budget.UpdateBudget(600m, invalidMonth, 2023));

        Assert.Equal("Budget month must be between 1 and 12", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UpdateBudget_WithInvalidYear_ShouldThrowDomainException(int invalidYear)
    {
        var budget = new Budget(Guid.NewGuid(), CategoryEnum.ENTERTAINMENT, 500m, 5, 2023);

        var exception = Assert.Throws<DomainException>(() =>
            budget.UpdateBudget(600m, 6, invalidYear));

        Assert.Equal("Budget year must be greater than zero", exception.Message);
    }

    [Fact]
    public void UpdateBudget_ShouldNotChangeUserId()
    {
        var userId = Guid.NewGuid();
        var budget = new Budget(userId, CategoryEnum.ENTERTAINMENT, 500m, 5, 2023);
        var originalUserId = budget.UserId;

        budget.UpdateBudget(600m, 6, 2023);

        Assert.Equal(originalUserId, budget.UserId);
    }

    [Fact]
    public void UpdateBudget_ShouldNotChangeCategoryId()
    {
        var budget = new Budget(Guid.NewGuid(), CategoryEnum.ENTERTAINMENT, 500m, 5, 2023);
        var originalCategoryId = budget.CategoryId;

        budget.UpdateBudget(600m, 6, 2023);

        Assert.Equal(originalCategoryId, budget.CategoryId);
    }
}
