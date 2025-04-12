using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;

namespace SimplePersonalFinance.Core.Interfaces.Data.Repositories;

public interface ITransactionReadRepository
{
    Task<Transaction?> GetByIdAsync(Guid id);
    Task<List<Transaction>> GetCategoryExpensesByAccountAndPeriod(Guid accountId, CategoryEnum category, DateTime period);
}