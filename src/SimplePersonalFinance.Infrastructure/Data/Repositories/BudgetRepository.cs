using Microsoft.EntityFrameworkCore;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Infrastructure.Data.Context;

namespace SimplePersonalFinance.Infrastructure.Data.Repositories;

public class BudgetRepository(AppDbContext context) : IBudgetRepository
{
    public async Task AddAsync(Budget budget, CancellationToken cancellationToken)
        => await context.Budgets.AddAsync(budget, cancellationToken);

    public async Task<Budget?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await context.Budgets
            .Include(budget => budget.Category)
            .SingleOrDefaultAsync(budget => budget.Id == id, cancellationToken);

    public async Task<Budget?> GetByUserAndCategoryAsync(
        Guid userId,
        int categoryId,
        CancellationToken cancellationToken)
        => await context.Budgets
            .Include(budget => budget.Category)
            .SingleOrDefaultAsync(
                budget => budget.UserId == userId && budget.CategoryId == categoryId,
                cancellationToken);

    public async Task<(IReadOnlyList<Budget> Items, int TotalItems)> GetAllByUserIdAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = context.Budgets
            .AsNoTracking()
            .Include(budget => budget.Category)
            .Where(budget => budget.UserId == userId && budget.IsActive);

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(budget => budget.Year)
            .ThenByDescending(budget => budget.Month)
            .ThenBy(budget => budget.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalItems);
    }
}
