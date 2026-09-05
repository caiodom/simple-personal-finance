using Microsoft.Extensions.Configuration;
using Moq;
using SimplePersonalFinance.Infrastructure.Services;

namespace SimplePersonalFinance.Test.Infrastructure.Services;

public class AuthServicePasswordTests
{
    private readonly AuthService _authService = new(new Mock<IConfiguration>().Object);

    [Fact]
    public void HashPassword_WithSamePassword_ShouldGenerateDifferentSaltedHashes()
    {
        const string password = "StrongPassword123!";

        var firstHash = _authService.HashPassword(password);
        var secondHash = _authService.HashPassword(password);

        Assert.NotEqual(firstHash, secondHash);
        Assert.StartsWith("PBKDF2-SHA256$600000$", firstHash);
        Assert.StartsWith("PBKDF2-SHA256$600000$", secondHash);
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        const string password = "StrongPassword123!";
        var passwordHash = _authService.HashPassword(password);

        var result = _authService.VerifyPassword(password, passwordHash);

        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_WithWrongPassword_ShouldReturnFalse()
    {
        var passwordHash = _authService.HashPassword("StrongPassword123!");

        var result = _authService.VerifyPassword("WrongPassword123!", passwordHash);

        Assert.False(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-password-hash")]
    [InlineData("PBKDF2-SHA256$invalid$salt$hash")]
    [InlineData("PBKDF2-SHA256$600000$not-base64$not-base64")]
    public void VerifyPassword_WithMalformedHash_ShouldReturnFalse(string passwordHash)
    {
        Assert.False(_authService.VerifyPassword("StrongPassword123!", passwordHash));
    }
}
