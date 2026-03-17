using KeyPilot.Domain.Workflows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KeyPilot.Infrastructure.Persistence.Configurations;

public sealed class WorkspaceWorkflowEventConfiguration : IEntityTypeConfiguration<WorkspaceWorkflowEvent>
{
    public void Configure(EntityTypeBuilder<WorkspaceWorkflowEvent> builder)
    {
        builder.ToTable("workspace_workflow_events");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .HasColumnName("id");

        builder.Property(entity => entity.WorkspaceId)
            .HasColumnName("workspace_id")
            .IsRequired();

        builder.Property(entity => entity.EventType)
            .HasColumnName("event_type")
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(entity => entity.DeduplicationKey)
            .HasColumnName("deduplication_key")
            .HasMaxLength(240)
            .IsRequired();

        builder.Property(entity => entity.OccurredAtUtc)
            .HasColumnName("occurred_at_utc")
            .IsRequired();

        builder.Property(entity => entity.TaskId)
            .HasColumnName("task_id");

        builder.Property(entity => entity.ConditionId)
            .HasColumnName("condition_id");

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .HasConversion(
                value => value.ToString().ToLowerInvariant(),
                value => Enum.Parse<WorkspaceWorkflowEventStatus>(value, ignoreCase: true))
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(entity => entity.Attempts)
            .HasColumnName("attempts")
            .IsRequired();

        builder.Property(entity => entity.ProcessedAtUtc)
            .HasColumnName("processed_at_utc");

        builder.Property(entity => entity.LastError)
            .HasColumnName("last_error")
            .HasMaxLength(1000);

        builder.Property(entity => entity.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.HasIndex(entity => entity.DeduplicationKey)
            .IsUnique();

        builder.HasIndex(entity => new { entity.Status, entity.CreatedAtUtc });
        builder.HasIndex(entity => entity.WorkspaceId);
    }
}