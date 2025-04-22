namespace SimplePersonalFinance.Core.Domain.Exceptions;

public class InvalidOperationException:DomainException
{
    public InvalidOperationException(string message) : base(message) { }
}
