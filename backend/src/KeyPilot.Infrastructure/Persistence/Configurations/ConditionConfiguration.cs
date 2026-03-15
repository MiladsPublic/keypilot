using KeyPilot.Domain.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KeyPilot.Infrastructure.Persistence.Configurations;

public sealed class ConditionConfiguration : IEntityTypeConfiguration<Condition>
{
    public void Configure(EntityTypeBuilder<Condition> builder)
    {
        builder.ToTable("conditions");

        builder.HasKey(condition => condition.Id);

        builder.Property(condition => condition.Id)
            .HasColumnName("id");

        builder.Property(condition => condition.PropertyId)
            .HasColumnName("property_id")
            .IsRequired();

        builder.Property(condition => condition.Type)
            .HasColumnName("type")
            .HasConversion(
                value => value.ToString().ToLowerInvariant(),
                value => Enum.Parse<ConditionType>(value, ignoreCase: true))
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(condition => condition.DueDate)
            .HasColumnName("due_date")
            .IsRequired();

        builder.Property(condition => condition.Status)
            .HasColumnName("status")
            .HasConversion(
                value => value.ToString().ToLowerInvariant(),
                value => Enum.Parse<ConditionStatus>(value, ignoreCase: true))
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(condition => condition.CompletedAtUtc)
            .HasColumnName("completed_at_utc");

        builder.Property(condition => condition.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.HasIndex(condition => condition.PropertyId);
    }
}
