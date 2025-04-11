using SimplePersonalFinance.Core.Domain.Entities;

namespace SimplePersonalFinance.Core.Interfaces.Data.Repositories;

public interface IAccountRepository
{
    Task AddAsync(Account account);
    void AddAccountTransaction(Transaction transaction);
    Task<List<Account>> GetAccountsByUserIdAsync(Guid userId);
    Task<Account?> GetByIdAsync(Guid id);
    Task<Account?> GetAccountWithTransactionsAsync(Guid id);
    Task<Account?> GetAccountWithSpecificTransactionAsync(Guid id, Guid transactionId);
    Task<Account?> GetFullAccountWithTransactionsAsync(Guid id);
}
