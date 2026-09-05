using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SimplePersonalFinance.Core.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SimplePersonalFinance.Infrastructure.Services;

public class AuthService(IConfiguration configuration) : IAuthService
{
    private const string PasswordHashAlgorithm = "pbkdf2-sha256";
    private const int PasswordIterations = 600_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;

    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            PasswordIterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return $"{PasswordHashAlgorithm}${PasswordIterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
            return false;

        if (TryParsePasswordHash(passwordHash, out var iterations, out var salt, out var expectedHash))
        {
            var actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }

        return VerifyLegacySha256(password, passwordHash);
    }

    public bool NeedsRehash(string passwordHash)
    {
        if (!TryParsePasswordHash(passwordHash, out var iterations, out _, out _))
            return true;

        return iterations < PasswordIterations;
    }

    public string GenerateJwtToken(Guid userId, string email, string role)
    {
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var expirationMinutes = configuration["Jwt:ExpirationMinutes"];
        var key = configuration["Jwt:Key"];

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new("userName", email),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(expirationMinutes)),
            signingCredentials: credentials,
            claims: claims);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static bool TryParsePasswordHash(
        string passwordHash,
        out int iterations,
        out byte[] salt,
        out byte[] hash)
    {
        iterations = 0;
        salt = [];
        hash = [];

        var parts = passwordHash.Split('$');
        if (parts.Length != 4 || !string.Equals(parts[0], PasswordHashAlgorithm, StringComparison.Ordinal))
            return false;

        if (!int.TryParse(parts[1], out iterations) || iterations <= 0)
            return false;

        try
        {
            salt = Convert.FromBase64String(parts[2]);
            hash = Convert.FromBase64String(parts[3]);
        }
        catch (FormatException)
        {
            return false;
        }

        return salt.Length >= SaltSize && hash.Length == HashSize;
    }

    private static bool VerifyLegacySha256(string password, string passwordHash)
    {
        try
        {
            var expectedHash = Convert.FromHexString(passwordHash);
            if (expectedHash.Length != HashSize)
                return false;

            var actualHash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
