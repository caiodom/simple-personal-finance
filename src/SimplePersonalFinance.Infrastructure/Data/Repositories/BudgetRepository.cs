using Microsoft.EntityFrameworkCore;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Infrastructure.Data.Context;

namespace SimplePersonalFinance.Infrastructure.Data.Repositories;

public class BudgetRepository(AppDbContext context) : IBudgetRepository
{
    public async Task AddAsync(Budget budget)
        => await context.Budgets.AddAsync(budget);

    public async Task<Budget?> GetByIdAsync(Guid id)
    {
        return await context.Budgets
            .Include(x => x.Category)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Budget?> GetByUserAndCategoryAsync(Guid userId, int categoryId)
    {
        return await context.Budgets
            .Include(x => x.Category)
            .SingleOrDefaultAsync(x => x.UserId == userId && x.CategoryId == categoryId);
    }

    public async Task<(IReadOnlyList<Budget> Items, int TotalItems)> GetAllByUserIdAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        ValidatePagination(pageNumber, pageSize);

        var query = context.Budgets
            .Include(x => x.Category)
            .Where(x => x.UserId == userId && x.IsActive)
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
