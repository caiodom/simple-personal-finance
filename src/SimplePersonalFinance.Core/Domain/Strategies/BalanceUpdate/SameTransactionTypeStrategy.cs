using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Interfaces.Domain.Strategies;

namespace SimplePersonalFinance.Core.Domain.Strategies.BalanceUpdate;

public class SameTransactionTypeStrategy : IBalanceUpdateStrategy
{
    public void UpdateBalance(
        Account account,
        decimal originalValue,
        decimal newValue,
        TransactionTypeEnum originalType,
        TransactionTypeEnum newType)
    {
        if (originalType == TransactionTypeEnum.INCOME)
            account.UpdateCurrentBalance(newValue - originalValue);
        else if (originalType == TransactionTypeEnum.EXPENSE)
            account.UpdateCurrentBalance(originalValue - newValue);
    }
}
