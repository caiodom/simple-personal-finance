using Microsoft.EntityFrameworkCore;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Infrastructure.Data.Context;

namespace SimplePersonalFinance.Infrastructure.Data.Repositories;

public class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public async Task<Transaction?> GetByIdAsync(Guid id)
        => await context.Transactions
            .Include(transaction => transaction.Category)
            .Include(transaction => transaction.TransactionType)
            .SingleOrDefaultAsync(transaction => transaction.Id == id && transaction.IsActive);

    public async Task<List<Transaction>> GetCategoryExpensesByAccountAndPeriod(
        Guid accountId,
        CategoryEnum category,
        DateTime period)
        => await context.Transactions
            .Where(transaction => transaction.AccountId == accountId &&
                                  transaction.Date.Month == period.Month &&
                                  transaction.Date.Year == period.Year &&
                                  transaction.TransactionTypeId == (int)TransactionTypeEnum.EXPENSE &&
                                  transaction.CategoryId == (int)category &&
                                  transaction.IsActive)
            .ToListAsync();

    public async Task<(IReadOnlyList<Transaction> Items, int TotalItems)> GetAllByAccountIdAsync(
        Guid accountId,
        int pageNumber,
        int pageSize)
    {
        var query = context.Transactions
            .AsNoTracking()
            .Include(transaction => transaction.TransactionType)
            .Include(transaction => transaction.Category)
            .Where(transaction => transaction.AccountId == accountId && transaction.IsActive);

        var totalItems = await query.CountAsync();
        var items = await query
            .OrderByDescending(transaction => transaction.Date)
            .ThenBy(transaction => transaction.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalItems);
    }
}
