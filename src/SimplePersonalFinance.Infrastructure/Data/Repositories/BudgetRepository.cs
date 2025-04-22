using Microsoft.EntityFrameworkCore;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Infrastructure.Data.Context;

namespace SimplePersonalFinance.Infrastructure.Data.Repositories;

public class BudgetRepository(AppDbContext context):IBudgetRepository
{
    public async Task AddAsync(Budget budget)
         => await context.Budgets.AddAsync(budget);

    public async Task<Budget?> GetByIdAsync(Guid id)
    {
        return await context.Budgets
            .Include(x => x.Category)
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Budget?>GetByUserAndCategoryAsync(Guid userId, int categoryId)
    {
        return await context.Budgets
            .Include(x => x.Category)
            .SingleOrDefaultAsync(x => x.UserId==userId && x.CategoryId == categoryId);
    }

    public IQueryable<Budget> GetAllByUserId(Guid userId)
    {
        return context.Budgets
                        .Include(x => x.Category)
                        .Where(x => x.UserId == userId && x.IsActive)
                        .AsNoTracking();

    }

}
