namespace SimplePersonalFinance.Core.Interfaces.Services;

public interface IAuthService
{
    string ComputeSha256Hash(string password);
    string GenerateJwtToken(Guid userId,string email, string role);
}
