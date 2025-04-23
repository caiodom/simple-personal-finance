using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Interfaces.Domain.Strategies;

namespace SimplePersonalFinance.Core.Domain.Strategies.BalanceUpdate;

public class BalanceUpdateStrategyFactory : IBalanceUpdateStrategyFactory
{
    public IBalanceUpdateStrategy Create(TransactionTypeEnum originalType, TransactionTypeEnum newType)
    {
        return originalType != newType
            ? new TransactionTypeChangeStrategy()
            : new SameTransactionTypeStrategy();
    }
}
