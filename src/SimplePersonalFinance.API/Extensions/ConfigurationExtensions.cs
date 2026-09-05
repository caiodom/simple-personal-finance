using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
using Serilog;
using SimplePersonalFinance.API.ExceptionHandling;
using SimplePersonalFinance.API.Filters;
using SimplePersonalFinance.API.Middlewares;
using SimplePersonalFinance.API.Services;
using SimplePersonalFinance.API.Services.Interfaces;
using SimplePersonalFinance.Application.Extensions;
using SimplePersonalFinance.Core.Interfaces.Services;
using SimplePersonalFinance.Infrastructure.Extensions;

namespace SimplePersonalFinance.API.Extensions;

public static class ConfigurationExtensions
{
    private const string CorsPolicyName = "CorsPolicy";

    public static WebApplicationBuilder AddBuilderConfigurations(this WebApplicationBuilder builder)
    {
        builder.AddSettingsConfigurations()
            .AddLog();

        return builder;
    }

    public static WebApplicationBuilder AddSettingsConfigurations(this WebApplicationBuilder builder)
    {
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

        return builder;
    }

    public static WebApplicationBuilder AddLog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.WithProperty("ApplicationName", "SimplePersonalFinance")
            .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .Enrich.WithCorrelationId()
            .CreateLogger();

        builder.Host.UseSerilog((context, services, configuration) =>
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());

        return builder;
    }

    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration)
            .AddAuthorization()
            .AddProblemDetails()
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddMiddlewares()
            .AddApplicationConfigurations()
            .AddCorsConfiguration(configuration)
            .AddEndpointsApiExplorer()
            .AddSwaggerConfigurations()
            .AddHealthCheck()
            .AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<AuthUserHandler>();
        services.AddScoped<IAuthUserHandler>(provider => provider.GetRequiredService<AuthUserHandler>());
        services.AddScoped<ICurrentUser>(provider => provider.GetRequiredService<AuthUserHandler>());

        return services;
    }

    public static WebApplication UseConfigurations(this WebApplication app)
    {
        Console.WriteLine(app.Environment.EnvironmentName);

        if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.UseApiMiddlewares();

        app.UseCors(CorsPolicyName);
        app.UseRequestLogging();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseHealthChecks()
            .MapControllers();

        return app;
    }

    private static IServiceCollection AddMiddlewares(this IServiceCollection services)
    {
        services.AddTransient<CorrelationIdMiddleware>();
        services.AddTransient<PerformanceMiddleware>();
        return services;
    }

    private static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .GetChildren()
            .Select(section => section.Value?.Trim().TrimEnd('/'))
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Select(origin => origin!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (allowedOrigins.Any(origin => string.Equals(origin, "*", StringComparison.Ordinal)))
            throw new InvalidOperationException("Wildcard CORS origins are not allowed. Configure explicit Cors:AllowedOrigins values.");

        foreach (var origin in allowedOrigins)
        {
            if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new InvalidOperationException($"Invalid CORS origin '{origin}'. Origins must be absolute HTTP or HTTPS URLs.");
            }
        }

        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName, policy =>
            {
                if (allowedOrigins.Length > 0)
                    policy.WithOrigins(allowedOrigins);

                policy.AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }

    private static IServiceCollection AddHealthCheck(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services;
    }

    private static IServiceCollection AddSwaggerConfigurations(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "SimplePersonalFinance.API", Version = "v1" });

            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("bearer", document)] = []
            });
        });

        return services;
    }

    private static WebApplication UseHealthChecks(this WebApplication app)
    {
        app.UseHealthChecks("/api/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        return app;
    }

    private static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        builder.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault());
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            };
        });

        return builder;
    }

    private static IApplicationBuilder UseApiMiddlewares(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<PerformanceMiddleware>();
        builder.UseMiddleware<CorrelationIdMiddleware>();
        return builder;
    }
}
