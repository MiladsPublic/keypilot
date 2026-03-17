using KeyPilot.Domain.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KeyPilot.Infrastructure.Persistence.Configurations;

public sealed class WorkspaceReminderConfiguration : IEntityTypeConfiguration<WorkspaceReminder>
{
    public void Configure(EntityTypeBuilder<WorkspaceReminder> builder)
    {
        builder.ToTable("workspace_reminders");

        builder.HasKey(reminder => reminder.Id);

        builder.Property(reminder => reminder.Id)
            .HasColumnName("id");

        builder.Property(reminder => reminder.PropertyId)
            .HasColumnName("property_id")
            .IsRequired();

        builder.Property(reminder => reminder.Key)
            .HasColumnName("key")
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(reminder => reminder.Title)
            .HasColumnName("title")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(reminder => reminder.ScheduledForUtc)
            .HasColumnName("scheduled_for_utc")
            .IsRequired();

        builder.Property(reminder => reminder.Status)
            .HasColumnName("status")
            .HasConversion(
                value => value.ToString().ToLowerInvariant(),
                value => Enum.Parse<WorkspaceReminderStatus>(value, ignoreCase: true))
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(reminder => reminder.SentAtUtc)
            .HasColumnName("sent_at_utc");

        builder.Property(reminder => reminder.CancelledAtUtc)
            .HasColumnName("cancelled_at_utc");

        builder.Property(reminder => reminder.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.HasIndex(reminder => reminder.PropertyId);
        builder.HasIndex(reminder => reminder.Key).IsUnique();
    }
}