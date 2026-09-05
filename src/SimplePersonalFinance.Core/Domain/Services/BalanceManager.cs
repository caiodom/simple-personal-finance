using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Interfaces.Domain.Services;
using SimplePersonalFinance.Core.Interfaces.Domain.Strategies;

namespace SimplePersonalFinance.Core.Domain.Services;

public class BalanceManager : IBalanceManager
{
    public void ApplyNewTransaction(Account account, decimal amount, TransactionTypeEnum type)
    {
        if (type == TransactionTypeEnum.EXPENSE)
            account.UpdateCurrentBalance(-amount);
        else if (type == TransactionTypeEnum.INCOME)
            account.UpdateCurrentBalance(amount);
    }

    public void RevertTransaction(Account account, Transaction transaction)
    {
        var type = (TransactionTypeEnum)transaction.TransactionTypeId;

        if (type == TransactionTypeEnum.INCOME)
            account.UpdateCurrentBalance(-transaction.Amount);
        else if (type == TransactionTypeEnum.EXPENSE)
            account.UpdateCurrentBalance(transaction.Amount);
    }

    public void UpdateBalanceForEdit(
        Account account,
        decimal originalValue,
        decimal newValue,
        TransactionTypeEnum originalType,
        TransactionTypeEnum newType,
        IBalanceUpdateStrategy strategy)
    {
        strategy.UpdateBalance(account, originalValue, newValue, originalType, newType);
    }
}
