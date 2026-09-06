using Microsoft.EntityFrameworkCore;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Infrastructure.Data.Repositories;

namespace SimplePersonalFinance.Test.Integration;

[Collection(PostgreSqlIntegrationCollection.CollectionName)]
public sealed class PostgreSqlPersistenceTests(PostgreSqlIntegrationFixture fixture)
{
    [Fact]
    public async Task Migrations_CreateExpectedSchemaAndSeedLookupData()
    {
        await using var context = fixture.CreateDbContext();

        Assert.True(await context.Database.CanConnectAsync());
        Assert.NotEmpty(await context.Database.GetAppliedMigrationsAsync());
        Assert.True(await context.AccountTypes.AsNoTracking().AnyAsync());
        Assert.True(await context.Categories.AsNoTracking().AnyAsync());
        Assert.True(await context.TransactionTypes.AsNoTracking().AnyAsync());
    }

    [Fact]
    public async Task UserRepository_PersistsAndQueriesUserByEmail()
    {
        var email = $"integration-{Guid.NewGuid():N}@example.com";
        var user = User.Create(
            "Integration User",
            email,
            "integration-password-hash",
            "User",
            new DateTime(1990, 1, 1)).Value;

        await using var context = fixture.CreateDbContext();
        var repository = new UserRepository(context);

        await repository.AddAsync(user, CancellationToken.None);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var persisted = await repository.GetByEmailAsync(email, CancellationToken.None);

        Assert.NotNull(persisted);
        Assert.Equal(user.Id, persisted.Id);
        Assert.Equal(email, persisted.Email.Value);
        Assert.True(await repository.CheckEmailAsync(email, CancellationToken.None));
    }

    [Fact]
    public async Task Database_RejectsDuplicateUserEmail()
    {
        var email = $"duplicate-{Guid.NewGuid():N}@example.com";
        var first = User.Create(
            "First User",
            email,
            "first-password-hash",
            "User",
            new DateTime(1990, 1, 1)).Value;
        var second = User.Create(
            "Second User",
            email,
            "second-password-hash",
            "User",
            new DateTime(1991, 1, 1)).Value;

        await using var firstContext = fixture.CreateDbContext();
        await firstContext.Users.AddAsync(first);
        await firstContext.SaveChangesAsync();

        await using var secondContext = fixture.CreateDbContext();
        await secondContext.Users.AddAsync(second);

        await Assert.ThrowsAsync<DbUpdateException>(() => secondContext.SaveChangesAsync());
    }
}
