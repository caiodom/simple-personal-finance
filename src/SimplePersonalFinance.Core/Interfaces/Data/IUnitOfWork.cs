using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using System.Data;

namespace SimplePersonalFinance.Core.Interfaces.Data
{
    public interface IUnitOfWork:IDisposable
    {
        IUserRepository Users { get; }
        IBudgetRepository Budgets { get; }
        IAccountRepository Accounts { get; }

        ITransactionReadRepository Transactions { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
    }
}
