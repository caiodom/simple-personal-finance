using FluentValidation;
using FluentValidation.Results;
using MediatR;
using DomainValidationException = SimplePersonalFinance.Core.Domain.Exceptions.ValidationException;

namespace SimplePersonalFinance.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        AggregateAndValidateFailures(validationResults);

        return await next();
    }

    private static void AggregateAndValidateFailures(ValidationResult[] validationResults)
    {
        var failures = validationResults
            .Where(result => !result.IsValid)
            .SelectMany(result => result.Errors)
            .GroupBy(
                error => error.PropertyName,
                error => error.ErrorMessage,
                (propertyName, errorMessages) => new
                {
                    Key = propertyName,
                    Values = errorMessages.Distinct().ToArray()
                })
            .ToDictionary(item => item.Key, item => item.Values);

        if (failures.Count > 0)
            throw new DomainValidationException(failures);
    }
}
