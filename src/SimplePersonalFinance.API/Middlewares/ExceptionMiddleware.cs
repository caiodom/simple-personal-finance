using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Exceptions;

namespace SimplePersonalFinance.API.Middlewares;

public class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IHostEnvironment environment) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        logger.LogError(
            exception,
            "Exception {ExceptionType} thrown for {RequestPath} with message: {Message}",
            exception.GetType().Name,
            context.Request.Path,
            exception.Message);

        var result = exception switch
        {
            EntityNotFoundException ex => CreateErrorResult(
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                ex.Message,
                new { ex.EntityName, ex.EntityId }),

            BusinessRuleViolationException ex => CreateErrorResult(
                StatusCodes.Status422UnprocessableEntity,
                "Business Rule Violation",
                ex.Message,
                new { ex.RuleName }),

            ValidationException ex => CreateErrorResult(
                StatusCodes.Status400BadRequest,
                "Validation Error",
                "One or more validation errors occurred",
                new { ex.Errors }),

            DomainException ex => CreateErrorResult(
                StatusCodes.Status400BadRequest,
                "Domain Error",
                ex.Message),

            _ => CreateErrorResult(
                StatusCodes.Status500InternalServerError,
                "Server Error",
                environment.IsDevelopment()
                    ? exception.Message
                    : "An unexpected error occurred while processing your request")
        };

        context.Response.StatusCode = (int)result.Extensions["StatusCode"];
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(result);
    }

    private static ResultViewModel CreateErrorResult(int statusCode, string title, string message, object? extensions = null)
    {
        var errorResult= new ResultViewModel(false,message);
        errorResult.AddExtension("StatusCode", statusCode);
        errorResult.AddExtension("Title", title);

        if(extensions is IReadOnlyDictionary<string, string[]> errors)
        {
            errorResult.AddExtension("errors", errors);
            return errorResult;
        }

        if (extensions != null)
        {
            var properties = extensions.GetType().GetProperties();
            foreach (var property in properties)
            {
                errorResult.AddExtension(property.Name, property.GetValue(extensions));
            }

            return errorResult;
        }

        return errorResult;
    }

}
