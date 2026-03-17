using KeyPilot.Domain.Common;

namespace KeyPilot.Domain.Workflows;

public sealed class WorkspaceWorkflowInstance : AuditableEntity
{
    private WorkspaceWorkflowInstance()
    {
    }

    private WorkspaceWorkflowInstance(
        Guid workspaceId,
        string workflowId,
        DateTime startedAtUtc,
        DateTime createdAtUtc)
    {
        WorkspaceId = workspaceId;
        WorkflowId = workflowId;
        Status = WorkspaceWorkflowInstanceStatus.Running;
        StartedAtUtc = startedAtUtc;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid WorkspaceId { get; private set; }

    public string WorkflowId { get; private set; } = string.Empty;

    public WorkspaceWorkflowInstanceStatus Status { get; private set; }

    public DateTime StartedAtUtc { get; private set; }

    public DateTime? LastSignaledAtUtc { get; private set; }

    public DateTime? ClosedAtUtc { get; private set; }

    public static WorkspaceWorkflowInstance Start(
        Guid workspaceId,
        string workflowId,
        DateTime startedAtUtc,
        DateTime createdAtUtc)
    {
        return new WorkspaceWorkflowInstance(workspaceId, workflowId, startedAtUtc, createdAtUtc);
    }

    public void MarkSignaled(DateTime signaledAtUtc)
    {
        LastSignaledAtUtc = signaledAtUtc;
    }

    public void MarkClosed(DateTime closedAtUtc)
    {
        Status = WorkspaceWorkflowInstanceStatus.Closed;
        ClosedAtUtc = closedAtUtc;
    }
}