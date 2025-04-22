using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Interfaces.Services;
using SimplePersonalFinance.Infrastructure.Data.Extensions;
using System.Reflection;

namespace SimplePersonalFinance.Infrastructure.Data.Context;

public class AppDbContext:DbContext
{

    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Budget> Budgets { get; set; }

    public DbSet<Category> Categories { get; set; }
    public DbSet<AccountType> AccountTypes { get; set; }
    public DbSet<TransactionType> TransactionTypes { get; set; }

    private readonly IDomainEventDispatcher _dispatcher;
    
    public AppDbContext(DbContextOptions<AppDbContext> options,IDomainEventDispatcher dispatcher) :base(options) 
    {
        _dispatcher = dispatcher;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.SeedDefaults();
        
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEntities= ChangeTracker.Entries<AggregateRoot>()
                  .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                    .ToList();

        var domainEvents = domainEntities
                            .SelectMany(x => x.Entity.DomainEvents!)
                            .ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        await _dispatcher.DispatchAsync(domainEvents);

        domainEntities.ForEach(x => x.Entity.ClearDomainEvents());

        return result;
    }
}
