using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SimplePersonalFinance.Application.Commands.CreateUser;
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
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateUserCommand>());
        return services;
    }

    public static IServiceCollection AddValidations(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateUserCommandValidator>();
        return services;
    }
}
