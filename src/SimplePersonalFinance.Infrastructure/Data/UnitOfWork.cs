using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Infrastructure.Data.Context;

namespace SimplePersonalFinance.Infrastructure.Data;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => context.SaveChangesAsync(cancellationToken);
}
