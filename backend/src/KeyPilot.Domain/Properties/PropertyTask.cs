using KeyPilot.Domain.Common;

namespace KeyPilot.Domain.Properties;

public sealed class PropertyTask : AuditableEntity
{
    private PropertyTask()
    {
    }

    internal PropertyTask(
        Guid propertyId,
        Guid? conditionId,
        string title,
        TaskStage stage,
        DateOnly? dueDate,
        DateTime createdAtUtc,
        string? description = null,
        TaskImportance importance = TaskImportance.Recommended)
    {
        PropertyId = propertyId;
        ConditionId = conditionId;
        Title = title;
        Stage = stage;
        DueDate = dueDate;
        Description = description;
        Importance = importance;
        Status = TaskStatus.Pending;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid PropertyId { get; private set; }

    public Guid? ConditionId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public TaskImportance Importance { get; private set; } = TaskImportance.Recommended;

    public string? Notes { get; private set; }

    public TaskStage Stage { get; private set; }

    public DateOnly? DueDate { get; private set; }

    public TaskStatus Status { get; private set; }

    public DateTime? CompletedAtUtc { get; private set; }

    public void MarkCompleted(DateTime completedAtUtc)
    {
        if (Status == TaskStatus.Completed)
        {
            return;
        }

        Status = TaskStatus.Completed;
        CompletedAtUtc = completedAtUtc;
    }

    public void MarkNeedsAttention()
    {
        if (Status == TaskStatus.Completed)
        {
            return;
        }

        Status = TaskStatus.NeedsAttention;
    }
}
