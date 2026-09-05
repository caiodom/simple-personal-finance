using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePersonalFinance.Core.Domain.Entities;

namespace SimplePersonalFinance.Infrastructure.Data.Mapping;

public class TransactionsMapping : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(transaction => transaction.Id);

        builder.Property(transaction => transaction.Description)
            .IsRequired()
            .HasMaxLength(350);

        builder.Property(transaction => transaction.Amount)
            .HasColumnType("decimal(18,2)");

        builder.HasIndex(transaction => new { transaction.AccountId, transaction.IsActive, transaction.Date })
            .HasDatabaseName("IX_Transactions_AccountId_IsActive_Date");

        builder.HasOne(transaction => transaction.Account)
            .WithMany(account => account.Transactions)
            .HasForeignKey(transaction => transaction.AccountId)
            .IsRequired();

        builder.HasOne(transaction => transaction.Category)
            .WithMany(category => category.Transactions)
            .HasForeignKey(transaction => transaction.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(transaction => transaction.TransactionType)
            .WithMany(type => type.Transactions)
            .HasForeignKey(transaction => transaction.TransactionTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
