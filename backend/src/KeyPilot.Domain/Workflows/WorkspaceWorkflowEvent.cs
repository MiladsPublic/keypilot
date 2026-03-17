using KeyPilot.Domain.Common;

namespace KeyPilot.Domain.Workflows;

public sealed class WorkspaceWorkflowEvent : AuditableEntity
{
    private WorkspaceWorkflowEvent()
    {
    }

    private WorkspaceWorkflowEvent(
        Guid workspaceId,
        string eventType,
        string deduplicationKey,
        DateTime occurredAtUtc,
        Guid? taskId,
        Guid? conditionId,
        DateTime createdAtUtc)
    {
        WorkspaceId = workspaceId;
        EventType = eventType;
        DeduplicationKey = deduplicationKey;
        OccurredAtUtc = occurredAtUtc;
        TaskId = taskId;
        ConditionId = conditionId;
        Status = WorkspaceWorkflowEventStatus.Pending;
        Attempts = 0;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid WorkspaceId { get; private set; }

    public string EventType { get; private set; } = string.Empty;

    public string DeduplicationKey { get; private set; } = string.Empty;

    public DateTime OccurredAtUtc { get; private set; }

    public Guid? TaskId { get; private set; }

    public Guid? ConditionId { get; private set; }

    public WorkspaceWorkflowEventStatus Status { get; private set; }

    public int Attempts { get; private set; }

    public DateTime? ProcessedAtUtc { get; private set; }

    public string? LastError { get; private set; }

    public static WorkspaceWorkflowEvent Create(
        Guid workspaceId,
        string eventType,
        string deduplicationKey,
        DateTime occurredAtUtc,
        DateTime createdAtUtc,
        Guid? taskId = null,
        Guid? conditionId = null)
    {
        return new WorkspaceWorkflowEvent(
            workspaceId,
            eventType,
            deduplicationKey,
            occurredAtUtc,
            taskId,
            conditionId,
            createdAtUtc);
    }

    public void MarkProcessed(DateTime processedAtUtc)
    {
        Status = WorkspaceWorkflowEventStatus.Processed;
        ProcessedAtUtc = processedAtUtc;
        Attempts += 1;
        LastError = null;
    }

    public void MarkAttemptFailed(string error)
    {
        Attempts += 1;
        LastError = error;
    }
}