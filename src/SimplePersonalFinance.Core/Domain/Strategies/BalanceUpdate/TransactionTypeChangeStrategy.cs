using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.ValueObjects;

namespace SimplePersonalFinance.Core.Domain.Strategies.BalanceUpdate;

public class TransactionTypeChangeStrategy : IBalanceUpdateStrategy
{
    public void UpdateBalance(Account account, MoneyAmount originalValue, MoneyAmount newValue)
    {
        account.UpdateCurrentBalance(originalValue.Amount, !originalValue.IsIncome);
        account.UpdateCurrentBalance(newValue.Amount, newValue.IsIncome);
    }
}
