using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;

namespace SimplePersonalFinance.Core.Interfaces.Domain.Strategies;

public interface IBalanceUpdateStrategy
{
    void UpdateBalance(
        Account account,
        decimal originalValue,
        decimal newValue,
        TransactionTypeEnum originalType,
        TransactionTypeEnum newType);
}
