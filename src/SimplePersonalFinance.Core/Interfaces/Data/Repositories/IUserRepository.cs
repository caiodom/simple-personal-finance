using SimplePersonalFinance.Core.Domain.Entities;

namespace SimplePersonalFinance.Core.Interfaces.Data.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> CheckEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
