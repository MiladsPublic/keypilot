using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Domain.Properties;
using KeyPilot.Domain.Workflows;

namespace KeyPilot.Application.Abstractions.Workflow;

public sealed class WorkspaceWorkflowOutbox(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider) : IWorkspaceWorkflowOutbox
{
    public async Task EnqueueWorkspaceCreatedAsync(Property workspace, DateTime occurredAtUtc, CancellationToken cancellationToken)
    {
        if (!workspace.WorkspaceId.HasValue)
        {
            return;
        }

        var deduplicationKey = $"workspace:{workspace.WorkspaceId}:created";
        if (await dbContext.WorkflowEventExistsByDeduplicationKeyAsync(deduplicationKey, cancellationToken))
        {
            return;
        }

        dbContext.AddWorkflowEvent(
            WorkspaceWorkflowEvent.Create(
                workspace.WorkspaceId.Value,
                eventType: "workspace_created",
                deduplicationKey: deduplicationKey,
                occurredAtUtc: occurredAtUtc,
                createdAtUtc: dateTimeProvider.UtcNow));
    }

    public async Task EnqueueWorkspaceSignalAsync(
        Guid workspaceId,
        string eventType,
        DateTime occurredAtUtc,
        CancellationToken cancellationToken,
        Guid? taskId = null,
        Guid? conditionId = null)
    {
        var deduplicationKey = $"workspace:{workspaceId}:{eventType}:{taskId}:{conditionId}:{occurredAtUtc.Ticks}";

        if (await dbContext.WorkflowEventExistsByDeduplicationKeyAsync(deduplicationKey, cancellationToken))
        {
            return;
        }

        dbContext.AddWorkflowEvent(
            WorkspaceWorkflowEvent.Create(
                workspaceId,
                eventType,
                deduplicationKey,
                occurredAtUtc,
                dateTimeProvider.UtcNow,
                taskId,
                conditionId));
    }
}