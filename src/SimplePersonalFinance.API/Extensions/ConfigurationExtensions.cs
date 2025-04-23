using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;
using SimplePersonalFinance.API.Middlewares;
using SimplePersonalFinance.API.Services;
using SimplePersonalFinance.API.Services.Interfaces;
using SimplePersonalFinance.Application.Extensions;
using SimplePersonalFinance.Infrastructure.Data.Context;
using SimplePersonalFinance.Infrastructure.Data.Extensions;
using SimplePersonalFinance.Infrastructure.Extensions;

namespace SimplePersonalFinance.API.Extensions;

public static class ConfigurationExtensions
{
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
                .AddExceptionHandler<ApiExceptionHandler>()
                .AddProblemDetails()
                .AddMiddlewares()
                .AddApplicationConfigurations()
                .AddCorsConfiguration(configuration)
                .AddEndpointsApiExplorer()
                .AddSwaggerConfigurations()
                .AddHealthCheck()
                .AddControllers();

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAuthUserHandler, AuthUserHandler>();

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
        app.UseCorrelationId();
        app.UsePerformanceTracking();

        app.UseCors("CorsPolicy");
        app.UseRequestLogging(); 
        app.UseAuthorization();

        app.UseHealthChecks()
            .MapControllers();


        app.Services.ApplyMigration(app.Environment);

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

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                 builder =>
                 {
                     builder
                            .WithOrigins(configuration.GetSection("AllowedHosts").Value)
                            .AllowAnyMethod()
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
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "SimplePersonalFinance.API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer schema."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                     {
                           new OpenApiSecurityScheme
                             {
                                 Reference = new OpenApiReference
                                 {
                                     Type = ReferenceType.SecurityScheme,
                                     Id = "Bearer"
                                 }
                             },
                             new string[] {}
                     }
                 });
        });

        return services;
    }
    private static WebApplication UseHealthChecks(this WebApplication app)
    {
        app.UseHealthChecks("/api/health", new HealthCheckOptions
        {
            ResponseWriter=UIResponseWriter.WriteHealthCheckUIResponse
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
                diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress);
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);

                if (httpContext.User.Identity?.IsAuthenticated == true)
                    diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value);
            };
        });

        return builder;
    }
    private static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorrelationIdMiddleware>();
    }
    private static IApplicationBuilder UsePerformanceTracking(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PerformanceMiddleware>();
    }
}
