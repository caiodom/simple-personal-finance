
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SimplePersonalFinance.Core.Domain.Exceptions;

namespace SimplePersonalFinance.API.Middlewares;

public class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger, IHostEnvironment environment) : IExceptionHandler
{

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Exception {ExceptionType} thrown for {Method} with message: {Message}",
            exception.GetType().Name,
            httpContext.Request.Path,
            exception.Message);

        var problemDetails = exception switch
        {
            EntityNotFoundException ex => CreateProblemDetails(
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                ex.Message,
                new { EntityName = ex.EntityName, EntityId = ex.EntityId }),

            BusinessRuleViolationException ex => CreateProblemDetails(
                StatusCodes.Status422UnprocessableEntity,
                "Business Rule Violation",
                ex.Message,
                new { RuleName = ex.RuleName }),

            ValidationException ex => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Validation Error",
                "One or more validation errors occurred",
                new { Errors = ex.Errors }),


            Core.Domain.Exceptions.InvalidOperationException ex => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Invalid Operation",
                ex.Message),

            DomainException ex => CreateProblemDetails(
                StatusCodes.Status400BadRequest,
                "Domain Error",
                ex.Message),

            _ => CreateProblemDetails(
                StatusCodes.Status500InternalServerError,
                "Server Error",
                environment.IsDevelopment()
                    ? exception.Message
                    : "An error occurred while processing your request")

        };

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;

    }


    private ProblemDetails CreateProblemDetails(int status, string title, string detail, object? extensions = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = Guid.NewGuid().ToString()
        };

        if (extensions != null)
        {
            var properties= extensions.GetType().GetProperties();
            foreach ( var property in properties)
                problemDetails.Extensions[property.Name] = property.GetValue(extensions);
        }

        return problemDetails;  
    }

}
