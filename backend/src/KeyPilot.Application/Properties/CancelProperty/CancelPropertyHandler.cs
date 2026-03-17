using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Abstractions.Workflow;
using KeyPilot.Application.Properties.GetPropertyById;
using KeyPilot.Application.Properties.Reminders;
using KeyPilot.Application.Properties.Summary;
using MediatR;

namespace KeyPilot.Application.Properties.CancelProperty;

public sealed class CancelPropertyHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IWorkspaceWorkflowOutbox workflowOutbox,
    IWorkspaceReminderSyncService workspaceReminderSyncService,
    IWorkspaceSummaryService workspaceSummaryService)
    : IRequestHandler<CancelPropertyCommand, PropertyDto?>
{
    public async Task<PropertyDto?> Handle(CancelPropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await dbContext.GetPropertyByIdAsync(request.Id, request.OwnerUserId, cancellationToken);

        if (property is null)
        {
            return null;
        }

        var now = dateTimeProvider.UtcNow;
        property.MarkCancelled(now);
        await workspaceReminderSyncService.SyncAsync(property, now, cancellationToken);

        if (property.WorkspaceId.HasValue)
        {
            await workflowOutbox.EnqueueWorkspaceSignalAsync(
                    property.WorkspaceId.Value,
                    eventType: "workspace_cancelled",
                    occurredAtUtc: now,
                cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return workspaceSummaryService.BuildPropertyDto(property, DateOnly.FromDateTime(now));
    }
}
