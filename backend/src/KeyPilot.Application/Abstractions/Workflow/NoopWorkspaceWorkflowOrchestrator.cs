namespace KeyPilot.Application.Abstractions.Workflow;

public sealed class NoopWorkspaceWorkflowOrchestrator : IWorkspaceWorkflowOrchestrator
{
    public Task StartAsync(WorkspaceWorkflowStartInput input, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task SignalAsync(WorkspaceWorkflowSignal signal, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}