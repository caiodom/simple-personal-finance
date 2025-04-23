using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Domain.ValueObjects;

namespace SimplePersonalFinance.Core.Domain.Factories;

public static class MoneyFactory
{
    public static Money Create(decimal amount)
    {
        var moneyResult = Money.Create(amount);
        if (moneyResult.IsFailure)
            throw new DomainException(moneyResult.Error);

        return moneyResult.Value;
    }
}
