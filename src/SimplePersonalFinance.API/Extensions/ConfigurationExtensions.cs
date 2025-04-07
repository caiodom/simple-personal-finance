using Microsoft.OpenApi.Models;
using Serilog;
using SimplePersonalFinance.API.Middlewares;
using SimplePersonalFinance.Application.Extensions;
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
        builder.Host.UseSerilog((context, logger) =>
        {
            logger.ReadFrom.Configuration(context.Configuration);
        });

        return builder;
    }
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration)
                .AddExceptionHandler<ApiExceptionHandler>()
                .AddProblemDetails()
                .AddApplicationConfigurations()
                .AddEndpointsApiExplorer()
                .AddSwaggerConfigurations()
                .AddControllers();

        return services;
    }
    public static IServiceCollection AddSwaggerConfigurations(this IServiceCollection services)
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
    public static WebApplication UseConfigurations(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandler();
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}
