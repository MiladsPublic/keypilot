namespace KeyPilot.Workflows;

public sealed record WorkspaceWorkflowSignalInput(
    string EventType,
    DateTime OccurredAtUtc,
    Guid? TaskId = null,
    Guid? ConditionId = null);
