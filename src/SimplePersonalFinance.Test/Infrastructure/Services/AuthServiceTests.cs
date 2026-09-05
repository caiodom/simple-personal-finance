using Microsoft.Extensions.Configuration;
using Moq;
using SimplePersonalFinance.Infrastructure.Services;
using System.Security.Cryptography;
using System.Text;

namespace SimplePersonalFinance.Test.Infrastructure.Services;

public class AuthServiceTests
{
    private readonly AuthService _authService = new(new Mock<IConfiguration>().Object);

    [Fact]
    public void HashPassword_ShouldUseVersionedPbkdf2Format()
    {
        var hash = _authService.HashPassword("Password123!");

        Assert.StartsWith("pbkdf2-sha256$600000$", hash);
        Assert.NotEqual("Password123!", hash);
        Assert.False(_authService.NeedsRehash(hash));
    }

    [Fact]
    public void HashPassword_WithSamePassword_ShouldUseDifferentSalts()
    {
        var first = _authService.HashPassword("Password123!");
        var second = _authService.HashPassword("Password123!");

        Assert.NotEqual(first, second);
        Assert.True(_authService.VerifyPassword("Password123!", first));
        Assert.True(_authService.VerifyPassword("Password123!", second));
    }

    [Fact]
    public void VerifyPassword_WithWrongPassword_ShouldReturnFalse()
    {
        var hash = _authService.HashPassword("Password123!");

        Assert.False(_authService.VerifyPassword("WrongPassword", hash));
    }

    [Fact]
    public void VerifyPassword_WithLegacySha256_ShouldAllowMigration()
    {
        const string password = "Password123!";
        var legacyHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password))).ToLowerInvariant();

        Assert.True(_authService.VerifyPassword(password, legacyHash));
        Assert.True(_authService.NeedsRehash(legacyHash));
    }

    [Fact]
    public void VerifyPassword_WithMalformedHash_ShouldReturnFalse()
    {
        Assert.False(_authService.VerifyPassword("Password123!", "not-a-valid-password-hash"));
        Assert.True(_authService.NeedsRehash("not-a-valid-password-hash"));
    }
}
