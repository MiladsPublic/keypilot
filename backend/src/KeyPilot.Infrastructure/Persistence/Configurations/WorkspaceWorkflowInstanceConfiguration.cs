using KeyPilot.Domain.Workflows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KeyPilot.Infrastructure.Persistence.Configurations;

public sealed class WorkspaceWorkflowInstanceConfiguration : IEntityTypeConfiguration<WorkspaceWorkflowInstance>
{
    public void Configure(EntityTypeBuilder<WorkspaceWorkflowInstance> builder)
    {
        builder.ToTable("workspace_workflow_instances");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Id)
            .HasColumnName("id");

        builder.Property(entity => entity.WorkspaceId)
            .HasColumnName("workspace_id")
            .IsRequired();

        builder.Property(entity => entity.WorkflowId)
            .HasColumnName("workflow_id")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(entity => entity.Status)
            .HasColumnName("status")
            .HasConversion(
                value => value.ToString().ToLowerInvariant(),
                value => Enum.Parse<WorkspaceWorkflowInstanceStatus>(value, ignoreCase: true))
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(entity => entity.StartedAtUtc)
            .HasColumnName("started_at_utc")
            .IsRequired();

        builder.Property(entity => entity.LastSignaledAtUtc)
            .HasColumnName("last_signaled_at_utc");

        builder.Property(entity => entity.ClosedAtUtc)
            .HasColumnName("closed_at_utc");

        builder.Property(entity => entity.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.HasIndex(entity => entity.WorkspaceId)
            .IsUnique();
    }
}