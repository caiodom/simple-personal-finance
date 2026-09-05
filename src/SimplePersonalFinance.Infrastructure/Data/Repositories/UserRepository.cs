using Microsoft.EntityFrameworkCore;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Infrastructure.Data.Context;

namespace SimplePersonalFinance.Infrastructure.Data.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken cancellationToken)
        => await context.Users.AddAsync(user, cancellationToken);

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await context.Users.SingleOrDefaultAsync(
            user => user.Id == id && user.IsActive,
            cancellationToken);

    public async Task<bool> CheckEmailAsync(string email, CancellationToken cancellationToken)
        => await context.Users.AnyAsync(
            user => user.Email.Value == email && user.IsActive,
            cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        => await context.Users.SingleOrDefaultAsync(
            user => user.Email.Value == email && user.IsActive,
            cancellationToken);
}
