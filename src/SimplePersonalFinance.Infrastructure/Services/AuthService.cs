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
    private const string PasswordHashScheme = "PBKDF2-SHA256";
    private const int PasswordHashIterations = 600_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            PasswordHashIterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return string.Join(
            '$',
            PasswordHashScheme,
            PasswordHashIterations,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash));
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(passwordHash))
            return false;

        var parts = passwordHash.Split('$');
        if (parts.Length != 4 || !string.Equals(parts[0], PasswordHashScheme, StringComparison.Ordinal))
            return false;

        if (!int.TryParse(parts[1], out var iterations) || iterations <= 0)
            return false;

        try
        {
            var salt = Convert.FromBase64String(parts[2]);
            var expectedHash = Convert.FromBase64String(parts[3]);

            if (salt.Length < SaltSize || expectedHash.Length < HashSize)
                return false;

            var actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        catch (FormatException)
        {
            return false;
        }
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
            expires: DateTime.Now.AddMinutes(double.Parse(expirationMinutes)),
            signingCredentials: credentials,
            claims: claims);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
