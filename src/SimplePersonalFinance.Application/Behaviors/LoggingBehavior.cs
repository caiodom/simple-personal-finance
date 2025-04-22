using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace SimplePersonalFinance.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName=typeof(TRequest).Name;
        var requestType= request.GetType().Name;
        var properties= new List<string>();

        foreach(PropertyInfo prop in request.GetType().GetProperties())
        {
            string propName= prop.Name.ToLower();

            if (propName.Contains("password") || propName.Contains("secret") || propName.Contains("credential"))
                continue;

            object? value= prop.GetValue(request, null);
            properties.Add($"{prop.Name}: {value}");
        }

        logger.LogInformation(
            "Handling {RequestName} ({RequestType}) with properties: {Properties}",
            requestName,
            requestType,
            string.Join(", ", properties));

        var response = await next();

        logger.LogInformation(
                    "Handled {RequestName} ({RequestType})",
                    requestName,
                    requestType);

        return response;

    }
}
