using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;

namespace SimplePersonalFinance.Core.Interfaces.Data.Repositories;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id);
    Task<List<Transaction>> GetCategoryExpensesByAccountAndPeriod(Guid accountId, CategoryEnum category, DateTime period);
    Task<(IReadOnlyList<Transaction> Items, int TotalItems)> GetAllByAccountIdAsync(
        Guid accountId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
