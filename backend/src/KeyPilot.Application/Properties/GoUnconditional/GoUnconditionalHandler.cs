using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Abstractions.Workflow;
using KeyPilot.Application.Properties.GetPropertyById;
using KeyPilot.Application.Properties.Reminders;
using KeyPilot.Application.Properties.Summary;
using KeyPilot.Application.Properties.TaskGeneration;
using KeyPilot.Domain.Properties;
using MediatR;

namespace KeyPilot.Application.Properties.GoUnconditional;

public sealed class GoUnconditionalHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IWorkspaceWorkflowOutbox workflowOutbox,
    IWorkspaceReminderSyncService workspaceReminderSyncService,
    IWorkspaceSummaryService workspaceSummaryService,
    ISettlementChecklistGenerator settlementChecklistGenerator)
    : IRequestHandler<GoUnconditionalCommand, PropertyDto?>
{
    public async Task<PropertyDto?> Handle(GoUnconditionalCommand request, CancellationToken cancellationToken)
    {
        var property = await dbContext.GetPropertyByIdAsync(request.Id, request.OwnerUserId, cancellationToken);

        if (property is null)
        {
            return null;
        }

        var now = dateTimeProvider.UtcNow;
        var today = DateOnly.FromDateTime(now);

        property.GoUnconditional(today);

        settlementChecklistGenerator.EnsureGenerated(property, now);
        await workspaceReminderSyncService.SyncAsync(property, now, cancellationToken);

        if (property.WorkspaceId.HasValue)
        {
            await workflowOutbox.EnqueueWorkspaceSignalAsync(
                property.WorkspaceId.Value,
                eventType: "went_unconditional",
                occurredAtUtc: now,
                cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return workspaceSummaryService.BuildPropertyDto(property, today);
    }
}
