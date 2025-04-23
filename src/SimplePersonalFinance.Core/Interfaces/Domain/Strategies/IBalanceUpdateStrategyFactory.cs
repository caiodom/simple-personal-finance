using SimplePersonalFinance.Core.Domain.Enums;

namespace SimplePersonalFinance.Core.Interfaces.Domain.Strategies;

public interface IBalanceUpdateStrategyFactory
{
    IBalanceUpdateStrategy Create(TransactionTypeEnum originalType, TransactionTypeEnum newType);
}
