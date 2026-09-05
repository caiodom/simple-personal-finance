namespace SimplePersonalFinance.Core.Interfaces.Services;

public interface IAuthService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
    bool NeedsRehash(string passwordHash);
    string GenerateJwtToken(Guid userId, string email, string role);
}
