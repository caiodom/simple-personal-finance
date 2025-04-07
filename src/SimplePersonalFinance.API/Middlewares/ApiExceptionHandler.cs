
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SimplePersonalFinance.API.Middlewares;

public class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server error"
        };

        logger.LogError(exception, "An error occurred while processing the request.");

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(details, cancellationToken);

        return true;
    }
}
