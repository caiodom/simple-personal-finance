
using Serilog.Context;

namespace SimplePersonalFinance.API.Middlewares;

public class CorrelationIdMiddleware:IMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next= next;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var correlationId = GetOrCreateCorrelationId(context);

        context.Response.OnStarting(() =>
        {
            context.Response.Headers.TryAdd(CorrelationIdHeader, correlationId);
            return Task.CompletedTask;
        });

        using (LogContext.PushProperty("CorrelationId", correlationId))
            await _next(context);
    }


    private string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlatonId))
            return correlatonId.ToString();

        return Guid.NewGuid().ToString();
    }
}
