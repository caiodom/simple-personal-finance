using SimplePersonalFinance.Core.Domain.Entities;

namespace SimplePersonalFinance.Core.Interfaces.Data.Repositories;

public interface ITransactionReadRepository
{
    Task<Transaction?> GetByIdAsync(Guid id);
}
