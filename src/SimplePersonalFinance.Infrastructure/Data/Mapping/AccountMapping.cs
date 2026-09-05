using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePersonalFinance.Core.Domain.Entities;

namespace SimplePersonalFinance.Infrastructure.Data.Mapping;

public class AccountMapping : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(account => account.Id);

        builder.Property(account => account.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(account => account.InitialBalance)
            .IsRequired()
            .HasColumnName("InitialBalance")
            .HasColumnType("decimal(18,2)");

        builder.Property(account => account.CurrentBalance)
            .IsRequired()
            .HasColumnName("CurrentBalance")
            .HasColumnType("decimal(18,2)");

        builder.HasOne(account => account.User)
            .WithMany(user => user.Accounts)
            .HasForeignKey(account => account.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(account => account.AccountType)
            .WithMany(accountType => accountType.Accounts)
            .HasForeignKey(account => account.AccountTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(account => account.Transactions)
            .WithOne(transaction => transaction.Account)
            .HasForeignKey(transaction => transaction.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
