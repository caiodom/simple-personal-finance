using SimplePersonalFinance.Core.Domain.Entities;

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
        var user = User.Create(name, email, passwordHash, role, birthdayDate).Value;

        // Assert
        Assert.Equal(name, user.Name);
        Assert.Equal(email, user.Email.Value);
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
        var user = User.Create(name, email, passwordHash, role, birthdayDate).Value;
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

        var user = User.Create("John Doe", "john.doe@example.com", "hashedpassword123", "User", new DateTime(1990, 1, 15)).Value;

        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.True(user.IsActive);
        Assert.True(user.CreatedAt > DateTime.MinValue);
    }

    [Fact]
    public void SetAsDeleted_ShouldMarkUserAsInactive()
    {
        // Arrange
        var user = User.Create("John Doe", "john.doe@example.com", "hashedpassword123", "User", new DateTime(1990, 1, 15)).Value;

        // Act
        user.SetAsDeleted();

        // Assert
        Assert.False(user.IsActive);
    }

    [Fact]
    public void Create_WithInvalidEmail_ShouldReturnFailure()
    {
        // Arrange
        var name = "John Doe";
        var invalidEmail = "invalid-email";
        var birthdayDate = new DateTime(1990, 1, 15);
        var passwordHash = "hashedpassword123";
        var role = "User";

        // Act
        var result = User.Create(name, invalidEmail, passwordHash, role, birthdayDate);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Email format is invalid", result.Error);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldReturnFailure()
    {
        // Arrange
        var emptyName = "";
        var email = "john.doe@example.com";
        var birthdayDate = new DateTime(1990, 1, 15);
        var passwordHash = "hashedpassword123";
        var role = "User";

        // Act
        var result = User.Create(emptyName, email, passwordHash, role, birthdayDate);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Name cannot be empty", result.Error);
    }

    [Fact]
    public void Create_WithEmptyPasswordHash_ShouldReturnFailure()
    {
        // Arrange
        var name = "John Doe";
        var email = "john.doe@example.com";
        var birthdayDate = new DateTime(1990, 1, 15);
        var emptyPasswordHash = "";
        var role = "User";

        // Act
        var result = User.Create(name, email, emptyPasswordHash, role, birthdayDate);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Password hash cannot be empty", result.Error);
    }


}
