using FluentValidation;
using FluentValidation.Results;
using MediatR;
using SimplePersonalFinance.Core.Domain.Exceptions;
using System.Threading.Tasks;

namespace SimplePersonalFinance.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        AggregateAndValidateFailures(validationResults);

        return await next();
    }

    private void AggregateAndValidateFailures(ValidationResult[]? validationResults)
    {
        var failures= validationResults
                        .Where(r=>!r.IsValid)
                        .SelectMany(r => r.Errors)
                        .GroupBy(
                            e => e.PropertyName,
                            e => e.ErrorMessage,
                            (propertyName,errorMessages)=> new
                            {
                                Key=propertyName,
                                Values= errorMessages.Distinct().ToArray()
                            })
                        .ToDictionary(x=>x.Key,x=>x.Values);

        if(failures.Count > 0)
            throw new Core.Domain.Exceptions.ValidationException(failures);
    }


}
