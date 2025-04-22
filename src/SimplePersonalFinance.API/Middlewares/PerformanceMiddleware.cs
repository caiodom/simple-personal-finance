using System.Diagnostics;
namespace SimplePersonalFinance.API.Middlewares;

public class PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var watch= Stopwatch.StartNew();

		try
		{
			await next(context);
		}
		finally 
		{
			watch.Stop();
			var elapsedMs= watch.ElapsedMilliseconds;


			logger.LogInformation(
				"HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {ElapsedMilliseconds}ms",
				context.Request.Method,
				context.Request.Path,
				context.Response.StatusCode,
				elapsedMs);


		}
    }
}
