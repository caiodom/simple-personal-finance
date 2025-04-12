using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;
using SimplePersonalFinance.Infrastructure.Data;
using SimplePersonalFinance.Infrastructure.Data.Context;
using SimplePersonalFinance.Infrastructure.Data.Repositories;
using SimplePersonalFinance.Infrastructure.Services;
using System.Text;

namespace SimplePersonalFinance.Infrastructure.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext(configuration)
                    .AddAuthentication(configuration)
                    .AddPersistence();

            return services;
        }
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddScoped<IBudgetRepository,BudgetRepository>();
            services.AddScoped<IUserRepository,UserRepository>();
            services.AddScoped<IAccountRepository,AccountRepository>();
            services.AddScoped<ITransactionReadRepository, TransactionReadRepository>();

            services.AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options=>
                            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                            b=>b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));  

            return services;
        }
        public static IServiceCollection AddAuthentication(this IServiceCollection services,IConfiguration configuration)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                    };
                });


            services.AddScoped<IAuthService, AuthService>();

            return services;
        }


    }
}
