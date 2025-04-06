using Microsoft.EntityFrameworkCore;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Infrastructure.Data.Extensions;
using System.Reflection;

namespace SimplePersonalFinance.Infrastructure.Data.Context;

public class AppDbContext:DbContext
{

    public DbSet<User> Users { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Account> Accounts { get; set; }

    public DbSet<Category> Categories { get; set; }
    public DbSet<AccountType> AccountTypes { get; set; }
    public DbSet<TransactionType> TransactionTypes { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) :base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.SeedDefaults();
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
