using System.Reflection;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Infrastructure.Data.Context;
using SimplePersonalFinance.Shared.Contracts;
using ApiConfigurationExtensions = SimplePersonalFinance.API.Extensions.ConfigurationExtensions;
using ApplicationConfigurationExtensions = SimplePersonalFinance.Application.Extensions.ConfigurationExtensions;

namespace SimplePersonalFinance.Test.Architecture;

public sealed class LayerDependencyTests
{
    private static readonly Assembly CoreAssembly = typeof(User).Assembly;
    private static readonly Assembly SharedAssembly = typeof(PaginatedResult<>).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(ApplicationConfigurationExtensions).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(AppDbContext).Assembly;
    private static readonly Assembly ApiAssembly = typeof(ApiConfigurationExtensions).Assembly;

    [Fact]
    public void Core_ShouldNotReferenceOuterLayers()
    {
        AssertDoesNotReference(
            CoreAssembly,
            ApplicationAssembly,
            InfrastructureAssembly,
            ApiAssembly);
    }

    [Fact]
    public void Shared_ShouldRemainIndependentFromApplicationLayers()
    {
        AssertDoesNotReference(
            SharedAssembly,
            CoreAssembly,
            ApplicationAssembly,
            InfrastructureAssembly,
            ApiAssembly);
    }

    [Fact]
    public void Application_ShouldNotReferenceInfrastructureOrApi()
    {
        AssertDoesNotReference(
            ApplicationAssembly,
            InfrastructureAssembly,
            ApiAssembly);
    }

    [Fact]
    public void Infrastructure_ShouldNotReferenceApi()
    {
        AssertDoesNotReference(InfrastructureAssembly, ApiAssembly);
    }

    private static void AssertDoesNotReference(Assembly source, params Assembly[] forbiddenAssemblies)
    {
        var referencedAssemblyNames = source
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => name is not null)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var forbiddenAssembly in forbiddenAssemblies)
        {
            var forbiddenName = forbiddenAssembly.GetName().Name;
            Assert.DoesNotContain(forbiddenName, referencedAssemblyNames);
        }
    }
}
