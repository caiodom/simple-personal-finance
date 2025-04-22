﻿namespace SimplePersonalFinance.Core.Domain.Entities.Base;

public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject one, ValueObject two)
    {
        if (ReferenceEquals(one, null))
            return ReferenceEquals(two, null);

        return one.Equals(two);
    }

    public static bool operator !=(ValueObject one, ValueObject two)
    {
        return !(one == two);
    }
}
