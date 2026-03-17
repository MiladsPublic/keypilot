using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Abstractions.Workflow;
using KeyPilot.Application.Properties.Lifecycle;
using KeyPilot.Application.Properties.Common;
using KeyPilot.Application.Properties.Reminders;
using MediatR;

namespace KeyPilot.Application.Conditions.FailCondition;

public sealed class FailConditionHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IWorkspaceWorkflowOrchestrator workflowOrchestrator,
    IWorkspaceLifecycleService workspaceLifecycleService,
    IWorkspaceReminderSyncService workspaceReminderSyncService) : IRequestHandler<FailConditionCommand, ConditionDto?>
{
    public async Task<ConditionDto?> Handle(FailConditionCommand request, CancellationToken cancellationToken)
    {
        var condition = await dbContext.GetConditionByIdAsync(request.Id, request.OwnerUserId, cancellationToken);

        if (condition is null)
        {
            return null;
        }

        var property = await dbContext.GetPropertyByIdAsync(condition.PropertyId, request.OwnerUserId, cancellationToken);

        if (property is null)
        {
            return null;
        }

        var now = dateTimeProvider.UtcNow;
        condition.MarkFailed();
        workspaceLifecycleService.ApplyDerivedState(property, DateOnly.FromDateTime(now));
        await workspaceReminderSyncService.SyncAsync(property, now, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        if (property.WorkspaceId.HasValue)
        {
            await workflowOrchestrator.SignalAsync(
                new WorkspaceWorkflowSignal(
                    property.WorkspaceId.Value,
                    EventType: "condition_failed",
                    OccurredAtUtc: now,
                    ConditionId: condition.Id),
                cancellationToken);
        }

        return ConditionDto.FromCondition(condition);
    }
}
