using Microsoft.EntityFrameworkCore;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Infrastructure.Data.Context;

namespace SimplePersonalFinance.Infrastructure.Data.Repositories;

public class TransactionReadRepository(AppDbContext context) : ITransactionReadRepository
{
    public Task<Transaction?> GetByIdAsync(Guid id)
    {
        return context.Transactions
            .Include(x => x.Category)
            .Include(x => x.TransactionType)
            .SingleOrDefaultAsync(x => x.Id == id && x.IsActive);
    }

    public Task<List<Transaction>> GetCategoryExpensesByAccountAndPeriod(Guid accountId,CategoryEnum category, DateTime period)
    {
        return context.Transactions
            .Where(x => x.AccountId == accountId && 
                        x.Date.Month == period.Month && 
                        x.Date.Year == period.Year && 
                        x.TransactionTypeId == (int)TransactionTypeEnum.EXPENSE &&
                        x.CategoryId == (int)category &&
                        x.IsActive)
            .ToListAsync();
    }
}
