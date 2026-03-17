using KeyPilot.Application.Abstractions.Persistence;
using KeyPilot.Application.Properties.Lifecycle;
using MediatR;

namespace KeyPilot.Application.Properties.Reminders.ProcessWorkspaceReminderTick;

public sealed class ProcessWorkspaceReminderTickHandler(
    IApplicationDbContext dbContext,
    IWorkspaceLifecycleService workspaceLifecycleService,
    IWorkspaceReminderSyncService workspaceReminderSyncService)
    : IRequestHandler<ProcessWorkspaceReminderTickCommand, bool>
{
    public async Task<bool> Handle(ProcessWorkspaceReminderTickCommand request, CancellationToken cancellationToken)
    {
        var workspace = await dbContext.GetPropertyByWorkspaceIdAsync(request.WorkspaceId, cancellationToken);

        if (workspace is null)
        {
            return false;
        }

        var nowUtc = request.TriggeredAtUtc;

        workspaceLifecycleService.ApplyDerivedState(workspace, DateOnly.FromDateTime(nowUtc));
        await workspaceReminderSyncService.SyncAsync(workspace, nowUtc, cancellationToken);

        foreach (var reminder in workspace.Reminders)
        {
            if (reminder.Status != Domain.Properties.WorkspaceReminderStatus.Pending)
            {
                continue;
            }

            if (reminder.ScheduledForUtc > nowUtc)
            {
                continue;
            }

            reminder.MarkSent(nowUtc);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}