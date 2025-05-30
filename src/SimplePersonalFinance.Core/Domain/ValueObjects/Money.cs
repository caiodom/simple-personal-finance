﻿using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Domain.Results;
using System.Globalization;

namespace SimplePersonalFinance.Core.Domain.ValueObjects;

public class Money : ValueObject
{
    public decimal Amount { get; }


    private Money(decimal amount)
    {
        Amount = amount;
    }

    public static Result<Money> Create(decimal amount)
    {

        if (amount < 0)
            return Result.Failure<Money>("Amount cannot be negative");


        return Result.Success(new Money(amount));
    }


    public Money Add(Money money)
    {


        return new Money(Amount + money.Amount);
    }

    public Money Subtract(Money money)
    {


        return new Money(Amount - money.Amount);
    }

    public Money Scale(decimal factor)
    {
        return new Money(Amount * factor);
    }

    public bool IsGreaterThan(Money money)
    {
        return Amount > money.Amount;
    }

    public bool IsZero() => Amount == 0;

    public override string ToString() => Amount.ToString("F2", CultureInfo.InvariantCulture);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
    }
}
