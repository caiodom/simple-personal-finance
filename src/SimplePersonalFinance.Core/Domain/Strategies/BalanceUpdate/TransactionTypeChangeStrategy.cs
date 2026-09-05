using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Interfaces.Domain.Strategies;

namespace SimplePersonalFinance.Core.Domain.Strategies.BalanceUpdate;

public class TransactionTypeChangeStrategy : IBalanceUpdateStrategy
{
    public void UpdateBalance(
        Account account,
        decimal originalValue,
        decimal newValue,
        TransactionTypeEnum originalType,
        TransactionTypeEnum newType)
    {
        ReverseOriginalTransactionEffect(account, originalValue, originalType);
        ApplyNewTransactionEffect(account, newValue, newType);
    }

    private static void ReverseOriginalTransactionEffect(Account account, decimal value, TransactionTypeEnum type)
    {
        if (type == TransactionTypeEnum.INCOME)
            account.UpdateCurrentBalance(-value);
        else if (type == TransactionTypeEnum.EXPENSE)
            account.UpdateCurrentBalance(value);
    }

    private static void ApplyNewTransactionEffect(Account account, decimal value, TransactionTypeEnum type)
    {
        if (type == TransactionTypeEnum.INCOME)
            account.UpdateCurrentBalance(value);
        else if (type == TransactionTypeEnum.EXPENSE)
            account.UpdateCurrentBalance(-value);
    }
}
