using SimplePersonalFinance.Core.Domain.Entities;

namespace SimplePersonalFinance.Core.Interfaces.Data.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
    Task<bool> CheckEmailAsync(string email);
    Task<User?> GetUserByEmailAndPasswordAsync(string email, string passwordHash);
}
