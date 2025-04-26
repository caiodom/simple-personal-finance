using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SimplePersonalFinance.Application.Behaviors;
using SimplePersonalFinance.Application.Commands.UserCommands.CreateUser;
using SimplePersonalFinance.Application.Validators;

namespace SimplePersonalFinance.Application.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddApplicationConfigurations(this IServiceCollection services)
    {
        services.AddMediaTR()
                .AddValidations();

        return services;
    }

    public static IServiceCollection AddMediaTR(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
           cfg.RegisterServicesFromAssemblyContaining<CreateUserCommand>();
           cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
           cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
         });

        /* services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();
         services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));*/
        return services;
    }

    public static IServiceCollection AddValidations(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();
        return services;
    }
}
