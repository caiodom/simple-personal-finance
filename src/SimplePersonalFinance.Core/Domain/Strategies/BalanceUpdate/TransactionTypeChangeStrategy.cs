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
        ArgumentNullException.ThrowIfNull(originalValue, nameof(originalValue));
        ArgumentNullException.ThrowIfNull(newValue, nameof(newValue));

        ReverseOriginalTransactionEffect(account, originalValue, originalType);
        ApplyNewTransactionEffect(account, newValue, newType);
    }

    private void ReverseOriginalTransactionEffect(Account account, Money value, TransactionTypeEnum type)
    {
        if (IsIncome(type))
            account.UpdateCurrentBalance(value.Scale(-1));

        if (IsExpense(type))
            account.UpdateCurrentBalance(value);
    }

    private void ApplyNewTransactionEffect(Account account, Money value, TransactionTypeEnum type)
    {
        if (IsIncome(type))
            account.UpdateCurrentBalance(value);

        if (IsExpense(type))
            account.UpdateCurrentBalance(value.Scale(-1));
    }

    private bool IsIncome(TransactionTypeEnum type) =>
        type == TransactionTypeEnum.INCOME;

    private bool IsExpense(TransactionTypeEnum type) =>
        type == TransactionTypeEnum.EXPENSE;
}
