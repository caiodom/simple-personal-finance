using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Factories;
using SimplePersonalFinance.Core.Domain.ValueObjects;
using SimplePersonalFinance.Core.Interfaces.Domain.Services;
using SimplePersonalFinance.Core.Interfaces.Domain.Strategies;

namespace SimplePersonalFinance.Core.Domain.Services;

public class BalanceManager : IBalanceManager
{
    public void ApplyNewTransaction(Account account, Money amount, TransactionTypeEnum type)
    {
        if (type == TransactionTypeEnum.EXPENSE)
            account.UpdateCurrentBalance(amount.Scale(-1));
        else if (type == TransactionTypeEnum.INCOME)
            account.UpdateCurrentBalance(amount);
    }

    public void RevertTransaction(Account account, Transaction transaction)
    {
        var amount = MoneyFactory.Create(transaction.Amount);
        var type = (TransactionTypeEnum)transaction.TransactionTypeId;

        // Reverse the effect (if income was added, now subtract; if expense was subtracted, now add)
        if (type == TransactionTypeEnum.INCOME)
            account.UpdateCurrentBalance(amount.Scale(-1));
        else if (type == TransactionTypeEnum.EXPENSE)
            account.UpdateCurrentBalance(amount);
    }

    public void UpdateBalanceForEdit(
        Account account,
        Money originalValue,
        Money newValue,
        TransactionTypeEnum originalType,
        TransactionTypeEnum newType,
        IBalanceUpdateStrategy strategy)
    {
        strategy.UpdateBalance(account, originalValue, newValue, originalType, newType);
    }
}
