using SimplePersonalFinance.Core.Domain.Entities;

namespace SimplePersonalFinance.Core.Interfaces.Data.Repositories;

public interface IAccountRepository
{
    Task AddAsync(Account account, CancellationToken cancellationToken);
    void AddAccountTransaction(Transaction transaction);
    Task<(IReadOnlyList<Account> Items, int TotalItems)> GetAccountsByUserIdAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
    Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Account?> GetAccountWithTransactionsAsync(Guid id, CancellationToken cancellationToken);
    Task<Account?> GetAccountWithSpecificTransactionAsync(Guid id, Guid transactionId, CancellationToken cancellationToken);
    Task<Account?> GetFullAccountWithTransactionsAsync(Guid id, CancellationToken cancellationToken);
}
