namespace SimplePersonalFinance.Core.Interfaces.Data;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
