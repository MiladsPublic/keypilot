using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.Lifecycle;

internal sealed class WorkspaceLifecycleService : IWorkspaceLifecycleService
{
    public void ApplyDerivedState(Property workspace, DateOnly today)
    {
        workspace.RecalculateStatus(today);
    }
}