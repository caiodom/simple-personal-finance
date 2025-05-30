﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using SimplePersonalFinance.Infrastructure.Data.Context;
using System;

namespace SimplePersonalFinance.Infrastructure.Data.Extensions;

public static class DbMigrationExtensions
{
    /// <summary>
    /// Generate migrations before running this method
    /// </summary>
    public static void ApplyMigration(this IServiceProvider serviceProvider, IWebHostEnvironment env)
    {
        // Create a new scope for resolving services
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
        var logger = scope.ServiceProvider
                  .GetRequiredService<ILoggerFactory>()
                  .CreateLogger("DbMigrationExtensions");

        if (dbContext is null)
            throw new ArgumentNullException(nameof(serviceProvider), "The AppDbContext could not be resolved from the service provider.");

        int maxRetries = 5;
        int delayInSeconds = 5;

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetry(
                maxRetries,
                retryAttempt => TimeSpan.FromSeconds(delayInSeconds),
                (exception, timeSpan, retryCount, context) =>
                {
                    //Console.WriteLine($"Failed to apply migrations in the context {typeof(AppDbContext).Name}, attempt {retryCount}/{maxRetries}. Error: {exception.Message}");
                    logger.LogWarning("Failed to apply migrations in context {ContextName}. Attempt {RetryCount}/{MaxRetries}. Error: {ErrorMessage}",
                                        typeof(AppDbContext).Name,
                                        retryCount,
                                        maxRetries,
                                        exception.Message);
                });

        retryPolicy.Execute(() =>
        {
            if (env.IsDevelopment() || env.IsEnvironment("Docker"))
            {
                // Check for pending migrations
                var pendingMigrations = dbContext.Database.GetPendingMigrations();
                if (pendingMigrations.Any())
                {

                    //Console.WriteLine($"Applying {pendingMigrations.Count()} pending migrations...");
                    logger.LogInformation("Applying {PendingMigrations} pending migrations...",
                        pendingMigrations.Count());
                    dbContext.Database.Migrate();
                }
                else
                {
                    //Console.WriteLine("No pending migrations found.");
                    logger.LogInformation("No pending migrations found.");
                }
            }
        });
    }

}
