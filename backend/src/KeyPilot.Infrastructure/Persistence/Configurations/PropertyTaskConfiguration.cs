using System.Text.RegularExpressions;
using KeyPilot.Domain.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KeyPilot.Infrastructure.Persistence.Configurations;

public sealed class PropertyTaskConfiguration : IEntityTypeConfiguration<PropertyTask>
{
    public void Configure(EntityTypeBuilder<PropertyTask> builder)
    {
        builder.ToTable("tasks");

        builder.HasKey(task => task.Id);

        builder.Property(task => task.Id)
            .HasColumnName("id");

        builder.Property(task => task.PropertyId)
            .HasColumnName("property_id")
            .IsRequired();

        builder.Property(task => task.ConditionId)
            .HasColumnName("condition_id");

        builder.Property(task => task.Title)
            .HasColumnName("title")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(task => task.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(task => task.Importance)
            .HasColumnName("importance")
            .HasConversion(
                value => ToSnakeCase(value.ToString()),
                value => Enum.Parse<TaskImportance>(value.Replace("_", ""), ignoreCase: true))
            .HasMaxLength(32)
            .HasDefaultValueSql("'recommended'")
            .IsRequired();

        builder.Property(task => task.Notes)
            .HasColumnName("notes")
            .HasMaxLength(2000);

        builder.Property(task => task.Stage)
            .HasColumnName("stage")
            .HasConversion(
                value => ToSnakeCase(value.ToString()),
                value => Enum.Parse<TaskStage>(value.Replace("_", ""), ignoreCase: true))
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(task => task.DueDate)
            .HasColumnName("due_date");

        builder.Property(task => task.Status)
            .HasColumnName("status")
            .HasConversion(
                value => ToSnakeCase(value.ToString()),
                value => Enum.Parse<Domain.Properties.TaskStatus>(value.Replace("_", ""), ignoreCase: true))
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(task => task.CompletedAtUtc)
            .HasColumnName("completed_at_utc");

        builder.Property(task => task.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.HasIndex(task => task.PropertyId);
        builder.HasIndex(task => task.ConditionId);
    }

    private static string ToSnakeCase(string input)
        => Regex.Replace(input, "(?<!^)([A-Z])", "_$1").ToLowerInvariant();
}
