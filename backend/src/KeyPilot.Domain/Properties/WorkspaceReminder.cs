using KeyPilot.Domain.Common;

namespace KeyPilot.Domain.Properties;

public sealed class WorkspaceReminder : AuditableEntity
{
    private WorkspaceReminder()
    {
    }

    internal WorkspaceReminder(
        Guid propertyId,
        string key,
        string title,
        DateTime scheduledForUtc,
        DateTime createdAtUtc,
        Guid? taskId = null)
    {
        PropertyId = propertyId;
        Key = key;
        Title = title;
        ScheduledForUtc = scheduledForUtc;
        Status = WorkspaceReminderStatus.Pending;
        TaskId = taskId;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid PropertyId { get; private set; }

    public Guid? TaskId { get; private set; }

    public string Key { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public DateTime ScheduledForUtc { get; private set; }

    public WorkspaceReminderStatus Status { get; private set; }

    public DateTime? SentAtUtc { get; private set; }

    public DateTime? CancelledAtUtc { get; private set; }

    public static WorkspaceReminder Create(
        Guid propertyId,
        string key,
        string title,
        DateTime scheduledForUtc,
        DateTime createdAtUtc,
        Guid? taskId = null)
    {
        return new WorkspaceReminder(propertyId, key, title, scheduledForUtc, createdAtUtc, taskId);
    }

    public void MarkSent(DateTime sentAtUtc)
    {
        if (Status != WorkspaceReminderStatus.Pending)
        {
            return;
        }

        Status = WorkspaceReminderStatus.Sent;
        SentAtUtc = sentAtUtc;
    }

    public void MarkCancelled(DateTime cancelledAtUtc)
    {
        if (Status == WorkspaceReminderStatus.Cancelled)
        {
            return;
        }

        Status = WorkspaceReminderStatus.Cancelled;
        CancelledAtUtc = cancelledAtUtc;
    }
}