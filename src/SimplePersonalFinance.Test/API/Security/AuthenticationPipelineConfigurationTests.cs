using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimplePersonalFinance.API.Extensions;

namespace SimplePersonalFinance.Test.API.Security;

public class AuthenticationPipelineConfigurationTests
{
    [Fact]
    public async Task AddServices_ShouldRegisterBearerAuthenticationAndAuthorization()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=test;Username=test;Password=test",
                ["Jwt:Issuer"] = "SimplePersonalFinance.Tests",
                ["Jwt:Audience"] = "SimplePersonalFinance.Tests",
                ["Jwt:Key"] = "test-only-signing-key-with-at-least-32-characters",
                ["AllowedHosts"] = "https://localhost"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddServices(configuration);

        await using var provider = services.BuildServiceProvider();
        var schemeProvider = provider.GetRequiredService<IAuthenticationSchemeProvider>();
        var defaultAuthenticateScheme = await schemeProvider.GetDefaultAuthenticateSchemeAsync();

        Assert.Equal(JwtBearerDefaults.AuthenticationScheme, defaultAuthenticateScheme?.Name);
        Assert.NotNull(provider.GetService<IAuthorizationService>());
    }
}
