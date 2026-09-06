using Microsoft.Extensions.Diagnostics.HealthChecks;
using SimplePersonalFinance.Infrastructure.Data.Context;

namespace SimplePersonalFinance.API.HealthChecks;

public sealed class DatabaseHealthCheck(AppDbContext context) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext healthCheckContext,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await context.Database.CanConnectAsync(cancellationToken);

            return canConnect
                ? HealthCheckResult.Healthy("PostgreSQL is reachable.")
                : HealthCheckResult.Unhealthy("PostgreSQL is not reachable.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("PostgreSQL connectivity check failed.", exception);
        }
    }
}
