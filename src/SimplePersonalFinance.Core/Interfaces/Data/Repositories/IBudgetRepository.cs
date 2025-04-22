using SimplePersonalFinance.Core.Domain.Entities;

namespace SimplePersonalFinance.Core.Interfaces.Data.Repositories;

public interface IBudgetRepository
{
    Task<Budget?> GetByUserAndCategoryAsync(Guid userId, int categoryId);
    Task AddAsync(Budget budget);
    Task<Budget?> GetByIdAsync(Guid id);
    IQueryable<Budget> GetAllByUserId(Guid userId);
}
