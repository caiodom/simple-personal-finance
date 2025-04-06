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
    public string ComputeSha256Hash(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
                builder.Append(bytes[i].ToString("x2"));

            return builder.ToString();
        }
    }

    public string GenerateJwtToken(string email, string role)
    {
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var expirationMinutes = configuration["Jwt:ExpirationMinutes"];
        var key = configuration["Jwt:Key"];

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
            {
                new Claim("userName", email),
                new Claim(ClaimTypes.Role, role)
            };

        var token = new JwtSecurityToken(
              issuer: issuer,
              audience: audience,
              expires: DateTime.Now.AddMinutes(double.Parse(expirationMinutes)),
              signingCredentials: credentials,
              claims: claims);

        var tokenHandler = new JwtSecurityTokenHandler();

        var stringToken = tokenHandler.WriteToken(token);

        return stringToken;
    }
}
