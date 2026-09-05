using SimplePersonalFinance.Core.Domain.Entities;

namespace SimplePersonalFinance.Core.Interfaces.Data.Repositories;

public interface IBudgetRepository
{
    Task<Budget?> GetByUserAndCategoryAsync(Guid userId, int categoryId, CancellationToken cancellationToken);
    Task AddAsync(Budget budget, CancellationToken cancellationToken);
    Task<Budget?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<(IReadOnlyList<Budget> Items, int TotalItems)> GetAllByUserIdAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}
