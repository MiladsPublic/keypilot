using KeyPilot.Application.Abstractions.Clock;
using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.GetPropertyById;
using KeyPilot.Application.Properties.Lifecycle;
using KeyPilot.Application.Properties.Reminders;
using KeyPilot.Application.Properties.Summary;
using KeyPilot.Application.Properties.TaskGeneration;
using MediatR;

namespace KeyPilot.Application.Properties.UpdateProperty;

public sealed class UpdatePropertyHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IWorkspaceLifecycleService workspaceLifecycleService,
    IWorkspaceReminderSyncService workspaceReminderSyncService,
    ISettlementChecklistGenerator settlementChecklistGenerator,
    IWorkspaceSummaryService workspaceSummaryService)
    : IRequestHandler<UpdatePropertyCommand, PropertyDto?>
{
    public async Task<PropertyDto?> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await dbContext.GetPropertyByIdAsync(request.Id, request.OwnerUserId, cancellationToken);

        if (property is null)
        {
            return null;
        }

        var now = dateTimeProvider.UtcNow;
        var today = DateOnly.FromDateTime(now);

        property.Update(
            request.Address,
            request.AcceptedOfferDate,
            request.SettlementDate,
            request.PurchasePrice,
            request.DepositAmount,
            request.MethodReference);

        workspaceLifecycleService.ApplyDerivedState(property, today);
        settlementChecklistGenerator.EnsureGenerated(property, now);
        await workspaceReminderSyncService.SyncAsync(property, now, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return workspaceSummaryService.BuildPropertyDto(property, today);
    }
}
