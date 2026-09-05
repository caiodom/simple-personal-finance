using Microsoft.EntityFrameworkCore;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Infrastructure.Data.Context;

namespace SimplePersonalFinance.Infrastructure.Data.Repositories;

public class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public async Task<Transaction?> GetByIdAsync(Guid id)
    {
        return await context.Transactions
            .Include(x => x.Category)
            .Include(x => x.TransactionType)
            .SingleOrDefaultAsync(x => x.Id == id && x.IsActive);
    }

    public async Task<List<Transaction>> GetCategoryExpensesByAccountAndPeriod(Guid accountId, CategoryEnum category, DateTime period)
    {
        return await context.Transactions
            .Where(x => x.AccountId == accountId &&
                        x.Date.Month == period.Month &&
                        x.Date.Year == period.Year &&
                        x.TransactionTypeId == (int)TransactionTypeEnum.EXPENSE &&
                        x.CategoryId == (int)category &&
                        x.IsActive)
            .ToListAsync();
    }

    public async Task<(IReadOnlyList<Transaction> Items, int TotalItems)> GetAllByAccountIdAsync(
        Guid accountId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        ValidatePagination(pageNumber, pageSize);

        var query = context.Transactions
            .Include(x => x.TransactionType)
            .Include(x => x.Category)
            .Where(x => x.AccountId == accountId && x.IsActive)
            .AsNoTracking();

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalItems);
    }

    private static void ValidatePagination(int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than zero.");

        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
    }
}
