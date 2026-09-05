using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePersonalFinance.Core.Domain.Entities;

namespace SimplePersonalFinance.Infrastructure.Data.Mapping;

public class BudgetMapping : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.ToTable("Budgets");
        builder.HasKey(budget => budget.Id);

        builder.Property(budget => budget.LimitAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasIndex(budget => budget.UserId)
            .HasDatabaseName("IX_Budgets_UserId");

        builder.HasIndex(budget => new { budget.UserId, budget.CategoryId })
            .IsUnique()
            .HasFilter("\"IsActive\" = TRUE")
            .HasDatabaseName("UX_Budgets_UserId_CategoryId_Active");

        builder.HasOne(budget => budget.User)
            .WithMany(user => user.Budgets)
            .HasForeignKey(budget => budget.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(budget => budget.Category)
            .WithMany(category => category.Budgets)
            .HasForeignKey(budget => budget.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
