using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Exceptions;
using Xunit;

namespace SimplePersonalFinance.Test.Core.Domain.Entities;

public class BudgetTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var category = CategoryEnum.ENTERTAINMENT;
        var limitAmount = 500m;
        var month = 5;
        var year = 2023;

        // Act
        var budget = new Budget(userId, category, limitAmount, month, year);

        // Assert
        Assert.Equal(userId, budget.UserId);
        Assert.Equal((int)category, budget.CategoryId);
        Assert.Equal(limitAmount, budget.LimitAmount);
        Assert.Equal(month, budget.Month);
        Assert.Equal(year, budget.Year);
    }

    [Fact]
    public void UpdateBudget_WithValidData_ShouldUpdateProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var budget = new Budget(userId, CategoryEnum.ENTERTAINMENT, 500m, 5, 2023);

        var newLimitAmount = 700m;
        var newMonth = 6;
        var newYear = 2023;

        // Act
        budget.UpdateBudget(newLimitAmount, newMonth, newYear);

        // Assert
        Assert.Equal(newLimitAmount, budget.LimitAmount);
        Assert.Equal(newMonth, budget.Month);
        Assert.Equal(newYear, budget.Year);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void UpdateBudget_WithZeroOrNegativeLimitAmount_ShouldThrowDomainException(decimal invalidAmount)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var budget = new Budget(userId, CategoryEnum.ENTERTAINMENT, 500m, 5, 2023);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            budget.UpdateBudget(invalidAmount, 6, 2023));

        Assert.Equal("Budget limit amount must be greater than zero", exception.Message);
    }

    [Fact]
    public void UpdateBudget_ShouldNotChangeUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var budget = new Budget(userId, CategoryEnum.ENTERTAINMENT, 500m, 5, 2023);
        var originalUserId = budget.UserId;

        // Act
        budget.UpdateBudget(600m, 6, 2023);

        // Assert
        Assert.Equal(originalUserId, budget.UserId);
    }

    [Fact]
    public void UpdateBudget_ShouldNotChangeCategoryId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var budget = new Budget(userId, CategoryEnum.ENTERTAINMENT, 500m, 5, 2023);
        var originalCategoryId = budget.CategoryId;

        // Act
        budget.UpdateBudget(600m, 6, 2023);

        // Assert
        Assert.Equal(originalCategoryId, budget.CategoryId);
    }
}
