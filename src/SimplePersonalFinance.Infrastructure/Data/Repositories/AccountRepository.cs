using Microsoft.EntityFrameworkCore;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Infrastructure.Data.Context;

namespace SimplePersonalFinance.Infrastructure.Data.Repositories;

public class AccountRepository(AppDbContext context) : IAccountRepository
{
    public async Task AddAsync(Account account)
        => await context.Accounts.AddAsync(account);

    public void AddAccountTransaction(Transaction transaction)
    {
        context.Entry(transaction).State = EntityState.Added;
    }

    public async Task<(IReadOnlyList<Account> Items, int TotalItems)> GetAccountsByUserIdAsync(
        Guid userId,
        int pageNumber,
        int pageSize)
    {
        var query = context.Accounts
            .AsNoTracking()
            .Include(account => account.AccountType)
            .Where(account => account.UserId == userId && account.IsActive);

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderByDescending(account => account.CreatedAt)
            .ThenBy(account => account.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalItems);
    }

    public async Task<Account?> GetByIdAsync(Guid id)
        => await context.Accounts
            .Include(account => account.AccountType)
            .SingleOrDefaultAsync(account => account.Id == id && account.IsActive);

    public async Task<Account?> GetFullAccountWithTransactionsAsync(Guid id)
        => await context.Accounts
            .Include(account => account.AccountType)
            .Include(account => account.Transactions)
                .ThenInclude(transaction => transaction.Category)
            .Include(account => account.Transactions)
                .ThenInclude(transaction => transaction.TransactionType)
            .SingleOrDefaultAsync(account => account.Id == id && account.IsActive);

    public async Task<Account?> GetAccountWithTransactionsAsync(Guid id)
        => await context.Accounts
            .Include(account => account.Transactions)
            .SingleOrDefaultAsync(account => account.Id == id && account.IsActive);

    public async Task<Account?> GetAccountWithSpecificTransactionAsync(Guid id, Guid transactionId)
        => await context.Accounts
            .Include(account => account.Transactions.Where(transaction => transaction.Id == transactionId && transaction.IsActive))
            .FirstOrDefaultAsync(account => account.Id == id && account.IsActive);
}
