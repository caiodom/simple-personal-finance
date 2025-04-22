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
        Assert.Contains("Invalid email format", result.Error);
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

    [Fact]
    public void Create_WithFutureBirthday_ShouldReturnFailure()
    {
        // Arrange
        var name = "John Doe";
        var email = "john.doe@example.com";
        var futureBirthday = DateTime.Now.AddDays(1);
        var passwordHash = "hashedpassword123";
        var role = "User";

        // Act
        var result = User.Create(name, email, passwordHash, role, futureBirthday);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Birthday cannot be in the future", result.Error);
    }

    [Fact]
    public void Create_WithInvalidRole_ShouldReturnFailure()
    {
        // Arrange
        var name = "John Doe";
        var email = "john.doe@example.com";
        var birthdayDate = new DateTime(1990, 1, 15);
        var passwordHash = "hashedpassword123";
        var invalidRole = "InvalidRole";

        // Act
        var result = User.Create(name, email, passwordHash, invalidRole, birthdayDate);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Invalid role", result.Error);
    }


/*
    [Fact]
    public void UpdateName_ShouldChangeNameProperty()
    {
        // Arrange
        var user = User.Create("John Doe", "john.doe@example.com", "hashedpassword123", "User", new DateTime(1990, 1, 15)).Value;
        var newName = "Jane Doe";

        // Act
        user.UpdateName(newName);

        // Assert
        Assert.Equal(newName, user.Name);
    }

    [Fact]
    public void UpdateEmail_WithValidEmail_ShouldChangeEmailProperty()
    {
        // Arrange
        var user = User.Create("John Doe", "john.doe@example.com", "hashedpassword123", "User", new DateTime(1990, 1, 15)).Value;
        var newEmail = "jane.doe@example.com";

        // Act
        var result = user.UpdateEmail(newEmail);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newEmail, user.Email.Value);
    }

    [Fact]
    public void UpdateEmail_WithInvalidEmail_ShouldReturnFailure()
    {
        // Arrange
        var user = User.Create("John Doe", "john.doe@example.com", "hashedpassword123", "User", new DateTime(1990, 1, 15)).Value;
        var invalidEmail = "invalid-email";

        // Act
        var result = user.UpdateEmail(invalidEmail);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Invalid email format", result.Message);
        Assert.Equal("john.doe@example.com", user.Email.Value); // Email should not change
    }

    [Fact]
    public void UpdatePasswordHash_ShouldChangePasswordHashProperty()
    {
        // Arrange
        var user = User.Create("John Doe", "john.doe@example.com", "hashedpassword123", "User", new DateTime(1990, 1, 15)).Value;
        var newPasswordHash = "newhashedpassword456";

        // Act
        user.UpdatePasswordHash(newPasswordHash);

        // Assert
        Assert.Equal(newPasswordHash, user.PasswordHash);
    }

    [Fact]
    public void UpdateRole_WithValidRole_ShouldChangeRoleProperty()
    {
        // Arrange
        var user = User.Create("John Doe", "john.doe@example.com", "hashedpassword123", "User", new DateTime(1990, 1, 15)).Value;
        var newRole = "Admin";

        // Act
        var result = user.UpdateRole(newRole);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newRole, user.Role);
    }

    [Fact]
    public void UpdateRole_WithInvalidRole_ShouldReturnFailure()
    {
        // Arrange
        var user = User.Create("John Doe", "john.doe@example.com", "hashedpassword123", "User", new DateTime(1990, 1, 15)).Value;
        var invalidRole = "InvalidRole";

        // Act
        var result = user.UpdateRole(invalidRole);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Invalid role", result.Message);
        Assert.Equal("User", user.Role); // Role should not change
    }

    */

}
