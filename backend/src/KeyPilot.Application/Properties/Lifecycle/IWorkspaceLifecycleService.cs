using KeyPilot.Domain.Properties;

namespace KeyPilot.Application.Properties.Lifecycle;

public interface IWorkspaceLifecycleService
{
    void ApplyDerivedState(Property workspace, DateOnly today);
}