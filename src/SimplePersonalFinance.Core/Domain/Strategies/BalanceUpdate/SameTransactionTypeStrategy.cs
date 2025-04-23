using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.ValueObjects;
using SimplePersonalFinance.Core.Interfaces.Domain.Strategies;

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
        ArgumentNullException.ThrowIfNull(originalValue, nameof(originalValue));
        ArgumentNullException.ThrowIfNull(newValue, nameof(newValue));

        if (IsIncome(originalType))
            UpdateForIncome(account, originalValue, newValue);

        if (IsExpense(originalType))
            UpdateForExpense(account, originalValue, newValue);
    }

    private void UpdateForIncome(Account account, Money originalValue, Money newValue)
    {
        if (IsGreaterThan(newValue, originalValue))
            account.UpdateCurrentBalance(CreateDifference(newValue, originalValue));

        if (IsGreaterThan(originalValue, newValue))
            account.UpdateCurrentBalance(CreateDifference(originalValue, newValue).Scale(-1));
    }

    private void UpdateForExpense(Account account, Money originalValue, Money newValue)
    {
        if (IsGreaterThan(newValue, originalValue))
            account.UpdateCurrentBalance(CreateDifference(newValue, originalValue).Scale(-1));

        if (IsGreaterThan(originalValue, newValue))
            account.UpdateCurrentBalance(CreateDifference(originalValue, newValue));
    }

    private bool IsIncome(TransactionTypeEnum type) =>
        type == TransactionTypeEnum.INCOME;

    private bool IsExpense(TransactionTypeEnum type) =>
        type == TransactionTypeEnum.EXPENSE;

    private bool IsGreaterThan(Money first, Money second) =>
        first.Amount > second.Amount;

    private Money CreateDifference(Money greater, Money lesser) =>
        Money.Create(greater.Amount - lesser.Amount).Value;
}
