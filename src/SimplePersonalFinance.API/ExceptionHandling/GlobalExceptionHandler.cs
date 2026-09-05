using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SimplePersonalFinance.Core.Domain.Exceptions;

namespace SimplePersonalFinance.API.ExceptionHandling;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        LogException(httpContext, exception);

        var problemDetails = CreateProblemDetails(httpContext, exception);
        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });

        return true;
    }

    private ProblemDetails CreateProblemDetails(HttpContext httpContext, Exception exception)
    {
        var problemDetails = exception switch
        {
            EntityNotFoundException ex => Create(
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                ex.Message,
                new Dictionary<string, object?>
                {
                    ["entityName"] = ex.EntityName,
                    ["entityId"] = ex.EntityId
                }),

            BusinessRuleViolationException ex => Create(
                StatusCodes.Status422UnprocessableEntity,
                "Business Rule Violation",
                ex.Message,
                new Dictionary<string, object?>
                {
                    ["ruleName"] = ex.RuleName
                }),

            ValidationException ex => Create(
                StatusCodes.Status400BadRequest,
                "Validation Error",
                "One or more validation errors occurred.",
                new Dictionary<string, object?>
                {
                    ["errors"] = ex.Errors
                }),

            DomainException ex => Create(
                StatusCodes.Status400BadRequest,
                "Domain Error",
                ex.Message),

            _ => Create(
                StatusCodes.Status500InternalServerError,
                "Server Error",
                environment.IsDevelopment()
                    ? exception.Message
                    : "An unexpected error occurred while processing your request.")
        };

        problemDetails.Instance = httpContext.Request.Path;
        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        return problemDetails;
    }

    private static ProblemDetails Create(
        int status,
        string title,
        string detail,
        IReadOnlyDictionary<string, object?>? extensions = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        };

        if (extensions is null)
            return problemDetails;

        foreach (var (key, value) in extensions)
            problemDetails.Extensions[key] = value;

        return problemDetails;
    }

    private void LogException(HttpContext httpContext, Exception exception)
    {
        if (exception is DomainException)
        {
            logger.LogWarning(
                exception,
                "Handled {ExceptionType} for {RequestPath}: {Message}",
                exception.GetType().Name,
                httpContext.Request.Path,
                exception.Message);
            return;
        }

        logger.LogError(
            exception,
            "Unhandled {ExceptionType} for {RequestPath}: {Message}",
            exception.GetType().Name,
            httpContext.Request.Path,
            exception.Message);
    }
}
