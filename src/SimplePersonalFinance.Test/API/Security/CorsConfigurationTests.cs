using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimplePersonalFinance.API.Extensions;

namespace SimplePersonalFinance.Test.API.Security;

public class CorsConfigurationTests
{
    [Fact]
    public void AddServices_WithExplicitOrigins_ShouldConfigureOnlyThoseOrigins()
    {
        var configuration = CreateConfiguration(new Dictionary<string, string?>
        {
            ["Cors:AllowedOrigins:0"] = "https://app.example.test/",
            ["Cors:AllowedOrigins:1"] = "http://localhost:3000"
        });
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddServices(configuration);

        using var provider = services.BuildServiceProvider();
        var corsOptions = provider.GetRequiredService<IOptions<CorsOptions>>().Value;
        var policy = corsOptions.GetPolicy("CorsPolicy");

        Assert.NotNull(policy);
        Assert.Equal(2, policy.Origins.Count);
        Assert.Contains("https://app.example.test", policy.Origins);
        Assert.Contains("http://localhost:3000", policy.Origins);
        Assert.DoesNotContain("*", policy.Origins);
    }

    [Fact]
    public void AddServices_WithWildcardOrigin_ShouldFailClosed()
    {
        var configuration = CreateConfiguration(new Dictionary<string, string?>
        {
            ["Cors:AllowedOrigins:0"] = "*"
        });
        var services = new ServiceCollection();
        services.AddLogging();

        var exception = Assert.Throws<InvalidOperationException>(() => services.AddServices(configuration));

        Assert.Contains("Wildcard CORS origins are not allowed", exception.Message, StringComparison.Ordinal);
    }

    private static IConfiguration CreateConfiguration(IReadOnlyDictionary<string, string?> overrides)
    {
        var values = new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=test;Username=test;Password=test",
            ["Jwt:Issuer"] = "SimplePersonalFinance.Tests",
            ["Jwt:Audience"] = "SimplePersonalFinance.Tests",
            ["Jwt:Key"] = "test-only-signing-key-with-at-least-32-characters",
            ["AllowedHosts"] = "*"
        };

        foreach (var (key, value) in overrides)
            values[key] = value;

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }
}
