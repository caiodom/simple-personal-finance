namespace SimplePersonalFinance.Core.Domain.ValueObjects;

public class MoneyAmount
{
    public decimal Amount { get; }
    public bool IsIncome { get; }

    public MoneyAmount(decimal amount, bool isIncome)
    {
        Amount = amount;
        IsIncome = isIncome;
    }
}
