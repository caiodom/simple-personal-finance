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
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        ValidatePagination(pageNumber, pageSize);

        var query = context.Accounts
            .Include(x => x.AccountType)
            .Where(x => x.UserId == userId && x.IsActive)
            .AsNoTracking();

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalItems);
    }

    public async Task<Account?> GetByIdAsync(Guid id)
        => await context.Accounts
            .Include(x => x.AccountType)
            .SingleOrDefaultAsync(x => x.Id == id && x.IsActive);

    public async Task<Account?> GetFullAccountWithTransactionsAsync(Guid id)
        => await context.Accounts
            .Include(x => x.AccountType)
            .Include(x => x.Transactions)
                .ThenInclude(x => x.Category)
            .Include(x => x.Transactions)
                .ThenInclude(x => x.TransactionType)
            .SingleOrDefaultAsync(x => x.Id == id && x.IsActive);

    public async Task<Account?> GetAccountWithTransactionsAsync(Guid id)
        => await context.Accounts
            .Include(x => x.Transactions)
            .SingleOrDefaultAsync(x => x.Id == id && x.IsActive);

    public async Task<Account?> GetAccountWithSpecificTransactionAsync(Guid id, Guid transactionId)
    {
        return await context.Accounts
            .Include(x => x.Transactions.Where(x => x.Id == transactionId && x.IsActive))
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
    }

    private static void ValidatePagination(int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than zero.");

        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
    }
}
