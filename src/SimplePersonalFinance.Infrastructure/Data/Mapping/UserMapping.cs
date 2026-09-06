using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimplePersonalFinance.Core.Domain.Entities;

namespace SimplePersonalFinance.Infrastructure.Data.Mapping;

public class UserMapping : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(user => user.BirthdayDate)
            .HasColumnType("date")
            .IsRequired();

        builder.OwnsOne(user => user.Email, email =>
        {
            email.Property(value => value.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(256);

            email.HasIndex(value => value.Value)
                .IsUnique()
                .HasDatabaseName("UX_Users_Email");
        });

        builder.Property(user => user.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);
    }
}
