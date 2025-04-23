using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.ValueObjects;

namespace SimplePersonalFinance.Core.Interfaces.Domain.Strategies;

public interface IBalanceUpdateStrategy
{
    void UpdateBalance(
        Account account,
        Money originalValue,
        Money newValue,
        TransactionTypeEnum originalType,
        TransactionTypeEnum newType);
}
