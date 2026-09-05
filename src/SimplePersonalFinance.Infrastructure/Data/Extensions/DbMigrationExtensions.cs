using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SimplePersonalFinance.Infrastructure.Data.Context;

namespace SimplePersonalFinance.Infrastructure.Data.Extensions;

public static class DbMigrationExtensions
{
    public static async Task ApplyMigrationsAsync(
        this IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("DbMigrationExtensions");

        var pendingMigrations = (await dbContext.Database
                .GetPendingMigrationsAsync(cancellationToken))
            .ToArray();

        if (pendingMigrations.Length == 0)
        {
            logger.LogInformation("No pending database migrations found");
            return;
        }

        logger.LogInformation(
            "Applying {PendingMigrationCount} pending database migrations",
            pendingMigrations.Length);

        await dbContext.Database.MigrateAsync(cancellationToken);

        logger.LogInformation("Database migrations applied successfully");
    }
}
