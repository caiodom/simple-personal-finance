using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePersonalFinance.Core.Domain.Entities;

namespace SimplePersonalFinance.Infrastructure.Data.Mapping;

public class AccountMapping: IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(150);

        builder.OwnsOne(a => a.InitialBalance, balance =>
        {
            balance.Property(b => b.Amount)
                .IsRequired()
                .HasColumnName("InitialBalance")
                .HasColumnType("decimal(18,2)");
                
        });

        builder.OwnsOne(a => a.CurrentBalance, balance =>
        {
            balance.Property(b => b.Amount)
                .IsRequired()
                .HasColumnName("CurrentBalance")
                .HasColumnType("decimal(18,2)");
                
        });


        builder.HasOne(a => a.User)
            .WithMany(u => u.Accounts)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.AccountType)
            .WithMany(at => at.Accounts)
            .HasForeignKey(a => a.AccountTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Transactions)
            .WithOne(x=>x.Account)
            .HasForeignKey(c => c.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

