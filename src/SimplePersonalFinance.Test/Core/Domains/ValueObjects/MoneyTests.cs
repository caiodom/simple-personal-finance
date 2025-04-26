using SimplePersonalFinance.Core.Domain.ValueObjects;

namespace SimplePersonalFinance.Test.Core.Domains.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_WithValidAmount_ShouldReturnSuccess()
    {
        // Arrange
        decimal amount = 100.50m;

        // Act
        var result = Money.Create(amount);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(amount, result.Value.Amount);
    }

    [Fact]
    public void Create_WithValidAmountAndCurrency_ShouldReturnSuccess()
    {
        // Arrange
        decimal amount = 100.50m;

        // Act
        var result = Money.Create(amount);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(amount, result.Value.Amount);
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldReturnFailure()
    {
        // Arrange
        decimal amount = -100.50m;

        // Act
        var result = Money.Create(amount);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Amount cannot be negative", result.Error);
    }



    [Fact]
    public void Add_ShouldAddAmounts()
    {
        // Arrange
        var money1 = Money.Create(100m).Value;
        var money2 = Money.Create(50m).Value;

        // Act
        var result = money1.Add(money2);

        // Assert
        Assert.Equal(150m, result.Amount);
    }






    [Fact]
    public void Scale_ShouldMultiplyAmount()
    {
        // Arrange
        var money = Money.Create(100m).Value;
        decimal factor = 1.5m;

        // Act
        var result = money.Scale(factor);

        // Assert
        Assert.Equal(150m, result.Amount);
    }

    [Fact]
    public void IsGreaterThan_WhenGreater_ShouldReturnTrue()
    {
        // Arrange
        var money1 = Money.Create(100m).Value;
        var money2 = Money.Create(50m).Value;

        // Act & Assert
        Assert.True(money1.IsGreaterThan(money2));
    }

    [Fact]
    public void IsGreaterThan_WhenLess_ShouldReturnFalse()
    {
        // Arrange
        var money1 = Money.Create(50m).Value;
        var money2 = Money.Create(100m).Value;

        // Act & Assert
        Assert.False(money1.IsGreaterThan(money2));
    }



    [Fact]
    public void IsZero_WhenZero_ShouldReturnTrue()
    {
        // Arrange
        var money = Money.Create(0m).Value;

        // Act & Assert
        Assert.True(money.IsZero());
    }

    [Fact]
    public void IsZero_WhenNonZero_ShouldReturnFalse()
    {
        // Arrange
        var money = Money.Create(10m).Value;

        // Act & Assert
        Assert.False(money.IsZero());
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        // Arrange
        var money = Money.Create(123.45m).Value;

        // Act
        string result = money.ToString();

        // Assert
        Assert.Equal("123.45", result);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var money1 = Money.Create(100m).Value;
        var money2 = Money.Create(100m).Value;

        // Act & Assert
        Assert.Equal(money1, money2);
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var money1 = Money.Create(100m).Value;
        var money2 = Money.Create(200m).Value;

        // Act & Assert
        Assert.NotEqual(money1, money2);
    }


}
