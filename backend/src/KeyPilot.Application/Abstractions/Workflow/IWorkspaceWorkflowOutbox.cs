using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Abstractions.Workflow;

public interface IWorkspaceWorkflowOutbox
{
    Task EnqueueWorkspaceCreatedAsync(Property workspace, DateTime occurredAtUtc, CancellationToken cancellationToken);

    Task EnqueueWorkspaceSignalAsync(
        Guid workspaceId,
        string eventType,
        DateTime occurredAtUtc,
        CancellationToken cancellationToken,
        Guid? taskId = null,
        Guid? conditionId = null);
}