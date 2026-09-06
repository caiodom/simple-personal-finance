using Microsoft.Extensions.Diagnostics.HealthChecks;
using SimplePersonalFinance.API.HealthChecks;
using SimplePersonalFinance.Test.Integration;

namespace SimplePersonalFinance.Test.API.HealthChecks;

[Collection(PostgreSqlIntegrationCollection.CollectionName)]
public sealed class DatabaseHealthCheckTests(PostgreSqlIntegrationFixture fixture)
{
    [Fact]
    public async Task CheckHealthAsync_WhenPostgreSqlIsAvailable_ShouldReturnHealthy()
    {
        await using var context = fixture.CreateDbContext();
        var healthCheck = new DatabaseHealthCheck(context);

        var result = await healthCheck.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None);

        Assert.Equal(HealthStatus.Healthy, result.Status);
    }
}
