using KeyPilot.Application.Abstractions.Persistence;
using MediatR;

namespace KeyPilot.Application.Properties.Reminders.ProcessWorkspaceReminderTick;

public sealed class ProcessWorkspaceReminderTickHandler(IApplicationDbContext dbContext)
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