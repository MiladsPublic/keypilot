using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.Reminders;

public interface IWorkspaceReminderSyncService
{
    Task SyncAsync(Property workspace, DateTime nowUtc, CancellationToken cancellationToken);
}