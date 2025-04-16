using SimplePersonalFinance.Core.Domain.Entities;
using Xunit;

namespace SimplePersonalFinance.Test.Core.Domain.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var name = "John Doe";
        var email = "john.doe@example.com";
        var birthdayDate = new DateTime(1990, 1, 15);
        var passwordHash = "hashedpassword123";
        var role = "User";

        // Act
        var user = new User(name, email, birthdayDate, passwordHash, role);

        // Assert
        Assert.Equal(name, user.Name);
        Assert.Equal(email, user.Email);
        Assert.Equal(birthdayDate, user.BirthdayDate);
        Assert.Equal(passwordHash, user.PasswordHash);
        Assert.Equal(role, user.Role);
    }

    [Fact]
    public void Constructor_ShouldSetPropertiesToPrivateSetters()
    {
        // Arrange
        var name = "John Doe";
        var email = "john.doe@example.com";
        var birthdayDate = new DateTime(1990, 1, 15);
        var passwordHash = "hashedpassword123";
        var role = "User";

        // Act
        var user = new User(name, email, birthdayDate, passwordHash, role);
        var properties = typeof(User).GetProperties();

        // Assert - checking that most properties have private setters
        Assert.Contains(properties, p => p.Name == "Name" && p.SetMethod != null && p.SetMethod.IsPrivate);
        Assert.Contains(properties, p => p.Name == "Email" && p.SetMethod != null && p.SetMethod.IsPrivate);
        Assert.Contains(properties, p => p.Name == "PasswordHash" && p.SetMethod != null && p.SetMethod.IsPrivate);
        Assert.Contains(properties, p => p.Name == "Role" && p.SetMethod != null && p.SetMethod.IsPrivate);
    }

    [Fact]
    public void User_ShouldInheritFromEntity()
    {
        // Arrange & Act
        var user = new User(
            "John Doe",
            "john.doe@example.com",
            new DateTime(1990, 1, 15),
            "hashedpassword123",
            "User");

        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.True(user.IsActive);
        Assert.True(user.CreatedAt > DateTime.MinValue);
    }

    [Fact]
    public void SetAsDeleted_ShouldMarkUserAsInactive()
    {
        // Arrange
        var user = new User(
            "John Doe",
            "john.doe@example.com",
            new DateTime(1990, 1, 15),
            "hashedpassword123",
            "User");

        // Act
        user.SetAsDeleted();

        // Assert
        Assert.False(user.IsActive);
    }

}
