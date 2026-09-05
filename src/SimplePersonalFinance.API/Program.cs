using SimplePersonalFinance.API.Extensions;
using SimplePersonalFinance.Infrastructure.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddBuilderConfigurations();
builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

if (args.Any(argument => string.Equals(argument, "--migrate", StringComparison.OrdinalIgnoreCase)))
{
    await app.Services.ApplyMigrationsAsync();
    return;
}

app.UseConfigurations();
await app.RunAsync();
