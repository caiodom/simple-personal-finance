using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.ValueObjects;

namespace SimplePersonalFinance.Core.Domain.Strategies.BalanceUpdate;

public class TransactionTypeChangeStrategy : IBalanceUpdateStrategy
{
    public void UpdateBalance(
        Account account,
        Money originalValue,
        Money newValue,
        TransactionTypeEnum originalType,
        TransactionTypeEnum newType)
    {
        if (originalValue == null)
            throw new ArgumentNullException(nameof(originalValue), "Original transaction value cannot be null");
        if (newValue == null)
            throw new ArgumentNullException(nameof(newValue), "New transaction value cannot be null");

        if (originalType == TransactionTypeEnum.INCOME)
            account.UpdateCurrentBalance(originalValue.Scale(-1)); 
        else
            account.UpdateCurrentBalance(originalValue);

        if (newType == TransactionTypeEnum.INCOME)
            account.UpdateCurrentBalance(newValue);
        else
            account.UpdateCurrentBalance(newValue.Scale(-1));
    }
}
