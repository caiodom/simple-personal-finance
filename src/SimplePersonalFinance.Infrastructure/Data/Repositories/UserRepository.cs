using Microsoft.EntityFrameworkCore;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Infrastructure.Data.Context;

namespace SimplePersonalFinance.Infrastructure.Data.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task AddAsync(User user)
                => await  context.Users.AddAsync(user);

    public async Task<User> GetByIdAsync(Guid id)
                => await context.Users.SingleOrDefaultAsync(x => x.Id == id && x.IsActive);
    

    public async Task<bool> CheckEmailAsync(string email)
        => await context.Users.AnyAsync(x => x.Email == email && x.IsActive);


    public async Task<User> GetUserByEmailAndPasswordAsync(string email, string passwordHash)
    {
        return await context.Users
                            .SingleOrDefaultAsync(u => u.Email == email && 
                                                       u.PasswordHash == passwordHash && 
                                                       u.IsActive);
    }

}
