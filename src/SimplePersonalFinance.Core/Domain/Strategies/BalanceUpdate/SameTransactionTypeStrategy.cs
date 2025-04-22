using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.ValueObjects;

namespace SimplePersonalFinance.Core.Domain.Strategies.BalanceUpdate;

public class SameTransactionTypeStrategy : IBalanceUpdateStrategy
{
    public void UpdateBalance(
        Account account,
        Money originalValue,
        Money newValue,
        TransactionTypeEnum originalType,
        TransactionTypeEnum newType)
    {
        if (originalValue == null)
            throw new ArgumentNullException(nameof(originalValue), "Transaction value cannot be null");

        if (newValue == null)
            throw new ArgumentNullException(nameof(newValue), "Transaction value cannot be null");

        if (originalType == TransactionTypeEnum.INCOME)
        {
            if (newValue.Amount > originalValue.Amount)
                account.UpdateCurrentBalance(Money.Create(newValue.Amount - originalValue.Amount).Value);
            else if (originalValue.Amount > newValue.Amount)
                account.UpdateCurrentBalance(Money.Create(originalValue.Amount - newValue.Amount).Value.Scale(-1));
        }
        else
        {
            if (newValue.Amount > originalValue.Amount)
                account.UpdateCurrentBalance(Money.Create(newValue.Amount - originalValue.Amount).Value.Scale(-1));
            else if (originalValue.Amount > newValue.Amount)
                account.UpdateCurrentBalance(Money.Create(originalValue.Amount - newValue.Amount).Value);
        }
    }
}
