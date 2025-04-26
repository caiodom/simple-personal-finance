namespace SimplePersonalFinance.Core.Domain.Exceptions;

public class ValidationException : DomainException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more validation failures have occurred.")
    {
        Errors = errors;
    }

    public ValidationException(string message)
    : base(message)
    {
        Errors = new Dictionary<string, string[]> { { "General", new[] { message } } };
    }

}

