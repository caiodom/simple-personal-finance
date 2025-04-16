using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.ValueObjects;

namespace SimplePersonalFinance.Core.Domain.Strategies.BalanceUpdate;

public class SameTransactionTypeStrategy : IBalanceUpdateStrategy
{

    public void UpdateBalance(Account account, MoneyAmount originalValue, MoneyAmount newValue)
    {
        if (newValue.Amount > originalValue.Amount)
        {
            account.UpdateCurrentBalance(newValue.Amount - originalValue.Amount, newValue.IsIncome);
            return;
        }

        if (originalValue.Amount > newValue.Amount)
        {
            account.UpdateCurrentBalance(originalValue.Amount - newValue.Amount, !newValue.IsIncome);
        }

    }
}

