using SimplePersonalFinance.Core.Domain.Entities;

namespace SimplePersonalFinance.Core.Interfaces.Data.Repositories;

public interface IAccountRepository
{
    Task AddAsync(Account account);
    void AddAccountTransaction(Transaction transaction);
    Task<(IReadOnlyList<Account> Items, int TotalItems)> GetAccountsByUserIdAsync(Guid userId, int pageNumber, int pageSize);
    Task<Account?> GetByIdAsync(Guid id);
    Task<Account?> GetAccountWithTransactionsAsync(Guid id);
    Task<Account?> GetAccountWithSpecificTransactionAsync(Guid id, Guid transactionId);
    Task<Account?> GetFullAccountWithTransactionsAsync(Guid id);
}
