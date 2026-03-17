namespace KeyPilot.Application.Abstractions.Workflow;

public interface IWorkspaceWorkflowOrchestrator
{
    Task StartAsync(WorkspaceWorkflowStartInput input, CancellationToken cancellationToken);

    Task SignalAsync(WorkspaceWorkflowSignal signal, CancellationToken cancellationToken);
}