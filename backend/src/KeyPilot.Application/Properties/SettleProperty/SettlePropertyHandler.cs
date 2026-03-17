using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Abstractions.Workflow;
using KeyPilot.Application.Properties.GetPropertyById;
using KeyPilot.Application.Properties.Reminders;
using KeyPilot.Application.Properties.Summary;
using MediatR;

namespace KeyPilot.Application.Properties.SettleProperty;

public sealed class SettlePropertyHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IWorkspaceWorkflowOrchestrator workflowOrchestrator,
    IWorkspaceReminderSyncService workspaceReminderSyncService,
    IWorkspaceSummaryService workspaceSummaryService)
    : IRequestHandler<SettlePropertyCommand, PropertyDto?>
{
    public async Task<PropertyDto?> Handle(SettlePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await dbContext.GetPropertyByIdAsync(request.Id, request.OwnerUserId, cancellationToken);

        if (property is null)
        {
            return null;
        }

        var now = dateTimeProvider.UtcNow;
        property.MarkSettlementComplete(now);
        await workspaceReminderSyncService.SyncAsync(property, now, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        if (property.WorkspaceId.HasValue)
        {
            await workflowOrchestrator.SignalAsync(
                new WorkspaceWorkflowSignal(
                    property.WorkspaceId.Value,
                    EventType: "workspace_settled",
                    OccurredAtUtc: now),
                cancellationToken);
        }

        return workspaceSummaryService.BuildPropertyDto(property, DateOnly.FromDateTime(now));
    }
}
