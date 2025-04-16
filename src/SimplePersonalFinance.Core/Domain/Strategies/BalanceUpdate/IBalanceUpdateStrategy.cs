using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.ValueObjects;

namespace SimplePersonalFinance.Core.Domain.Strategies.BalanceUpdate;

public interface IBalanceUpdateStrategy
{
    void UpdateBalance(Account account, MoneyAmount originalValue, MoneyAmount newValue);
}
