using Microsoft.EntityFrameworkCore;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Infrastructure.Data.Context;

namespace SimplePersonalFinance.Infrastructure.Data.Repositories;

public class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await context.Transactions
            .Include(transaction => transaction.Category)
            .Include(transaction => transaction.TransactionType)
            .SingleOrDefaultAsync(
                transaction => transaction.Id == id && transaction.IsActive,
                cancellationToken);

    public async Task<List<Transaction>> GetCategoryExpensesByAccountAndPeriod(
        Guid accountId,
        CategoryEnum category,
        DateTime period,
        CancellationToken cancellationToken)
        => await context.Transactions
            .Where(transaction => transaction.AccountId == accountId &&
                                  transaction.Date.Month == period.Month &&
                                  transaction.Date.Year == period.Year &&
                                  transaction.TransactionTypeId == (int)TransactionTypeEnum.EXPENSE &&
                                  transaction.CategoryId == (int)category &&
                                  transaction.IsActive)
            .ToListAsync(cancellationToken);

    public async Task<(IReadOnlyList<Transaction> Items, int TotalItems)> GetAllByAccountIdAsync(
        Guid accountId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = context.Transactions
            .AsNoTracking()
            .Include(transaction => transaction.TransactionType)
            .Include(transaction => transaction.Category)
            .Where(transaction => transaction.AccountId == accountId && transaction.IsActive);

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(transaction => transaction.Date)
            .ThenBy(transaction => transaction.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalItems);
    }
}
